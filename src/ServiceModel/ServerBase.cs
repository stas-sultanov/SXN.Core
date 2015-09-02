using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	/// <summary>
	/// Provides a base class for the servers.
	/// </summary>
	public abstract class ServerBase : WorkerBase
	{
		#region Constant and Static Fields

		/// <summary>
		/// The default result of the <see cref="TryAwaitRequestAsync" /> operation.
		/// </summary>
		protected static readonly TryResult<IServerRequestHandler> TryAwaitRequestFailResult = new TryResult<IServerRequestHandler>();

		#endregion

		#region Fields

		/// <summary>
		/// The collection of active tasks.
		/// </summary>
		private readonly LinkedList<KeyValuePair<Task<Boolean>, IServerRequestHandler>> activeTasks;

		/// <summary>
		/// The instance of <see cref="PerformanceCounter" /> which measures the count of active tasks.
		/// </summary>
		private readonly PerformanceCounter activeTasksCounter;

		/// <summary>
		/// The instance of <see cref="PerformanceCounter" /> which measures the count of bad requests per second.
		/// </summary>
		private readonly PerformanceCounter badRequestsPerSecondCounter;

		/// <summary>
		/// The instance of <see cref="PerformanceCounter" /> which measures the average time base to process the requests.
		/// </summary>
		private readonly PerformanceCounter requestProcessingAverageBaseCounter;

		/// <summary>
		/// The instance of <see cref="PerformanceCounter" /> which measures the average time to process the requests.
		/// </summary>
		private readonly PerformanceCounter requestProcessingAverageTimeCounter;

		/// <summary>
		/// The instance of <see cref="PerformanceCounter" /> which measures the count of requests per second.
		/// </summary>
		private readonly PerformanceCounter requestsPerSecondCounter;

		#endregion

		#region Constructors

		/// <summary>
		/// Initialize a new instance of <see cref="ServerBase" /> class.
		/// </summary>
		/// <param name="diagnosticsEventHandler">A delegate to the method that will handle the diagnostics events.</param>
		/// <param name="settings">A server configuration settings.</param>
		/// <remarks>Constructor of the final class must set <see cref="WorkerBase.State" /> to the <see cref="EntityState.Inactive" /> state.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="diagnosticsEventHandler" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException"><paramref name="settings" /> is <c>null</c> or is not valid.</exception>
		/// <exception cref="KeyNotFoundException"><paramref name="settings" /> does not contains the required performance counter configuration.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal ServerBase(EventHandler<DiagnosticsEventArgs> diagnosticsEventHandler, ServerSettings settings)
			: base(diagnosticsEventHandler, settings?.Name)
		{
			// Check settings
			if (settings == null)
			{
				TraceEvent(EventLevel.Critical, @"Settings are invalid.");

				throw new ArgumentException(@"Settings are invalid.", nameof(settings));
			}

			// Initialize active tasks
			activeTasks = new LinkedList<KeyValuePair<Task<Boolean>, IServerRequestHandler>>();

			// Initialize performance counters
			var counters = new Dictionary<String, PerformanceCounter>();

			foreach (var counterConfiguration in settings.PerformanceCounters)
			{
				// Create instance of the counter
				var counter = new PerformanceCounter(counterConfiguration.Value.Category, counterConfiguration.Value.Name, false);

				// Add to the dictionary
				counters.Add(counterConfiguration.Key, counter);
			}

			PerformanceCounters = new ReadOnlyDictionary<String, PerformanceCounter>(counters);

			// Initialize performance counter
			activeTasksCounter = PerformanceCounters[@"ActiveTasks"];

			// Initialize performance counter
			badRequestsPerSecondCounter = PerformanceCounters[@"BadRequestsPerSecond"];

			// Initialize performance counter
			requestsPerSecondCounter = PerformanceCounters[@"RequestsPerSecond"];

			// Initialize performance counter
			requestProcessingAverageBaseCounter = PerformanceCounters[@"RequestProcessingAverageBase"];

			// Initialize performance counter
			requestProcessingAverageTimeCounter = PerformanceCounters[@"RequestProcessingAverageTime"];
		}

		#endregion

		#region Properties

		/// <summary>
		/// The dictionary of performance counters.
		/// </summary>
		protected IReadOnlyDictionary<String, PerformanceCounter> PerformanceCounters
		{
			get;
		}

		#endregion

		#region Overrides of WorkerBase

		/// <summary>
		/// Releases managed resources held by current instance.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void ReleaseManagedResources()
		{
			// Dispose performance counters
			foreach (var performanceCounter in PerformanceCounters)
			{
				performanceCounter.Value.Dispose();
			}
		}

		/// <summary>
		/// Initiates an asynchronous operation to process the requests.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for a task to complete.</param>
		/// <returns>A <see cref="Task" /> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async Task RunAsync(CancellationToken cancellationToken)
		{
			// Trace diagnostics event
			TraceEvent(EventLevel.Informational, @"Main cycle started.");

			// While service is active
			while (IsActive)
			{
				// Try await for the request
				var awaitResult = await TryAwaitRequestAsync();

				// Check if await operation has succeeded
				if (!awaitResult.Success)
				{
					continue;
				}

				// Get handler
				var handler = awaitResult.Result;

				// Start new task
				var task = awaitResult.Result.TryProcessAsync();

				// Add task to the collection of active tasks
				activeTasks.AddLast(new KeyValuePair<Task<Boolean>, IServerRequestHandler>(task, handler));

				// Increment requests counter
				requestsPerSecondCounter.Increment();

				// Update tasks counter
				activeTasksCounter.Increment();

				// Finalize completed requests
				activeTasks.Remove(FinalizeRequestIfCompleted);
			}

			// Trace diagnostics event
			TraceEvent(EventLevel.Informational, @"Main cycle stopped.");

			// Check if there are unfinished tasks
			if (activeTasks.Count == 0)
			{
				return;
			}

			// Wait for all active tasks to complete
			await Task.WhenAll(activeTasks.Select(pair => pair.Key));

			// Finalize all completed requests
			activeTasks.Remove(FinalizeRequestIfCompleted);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Performs finalization routines for the task and handler, if tasks is completed.
		/// </summary>
		/// <param name="pair">A task and handler pair to finalize.</param>
		/// <returns><c>true</c> if task is completed, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Boolean FinalizeRequestIfCompleted(KeyValuePair<Task<Boolean>, IServerRequestHandler> pair)
		{
			// Check task status
			switch (pair.Key.Status)
			{
				case TaskStatus.RanToCompletion:
				{
					// Check if task has completed with false result
					if (!pair.Key.Result)
					{
						// Increment bad requests counter
						badRequestsPerSecondCounter.Increment();
					}

					break;
				}
				case TaskStatus.Canceled:
				{
					break;
				}
				case TaskStatus.Faulted:
				{
					// Increment bad requests counter
					badRequestsPerSecondCounter.Increment();

					// Trace error event
					TraceEvent(EventLevel.Error, pair.Key.Exception?.ToString() ?? @"Unknown Error");

					break;
				}
				default:
				{
					return false;
				}
			}

			// Dispose task
			pair.Key.Dispose();

			// Dispose handler
			pair.Value.Dispose();

			// Update base counter
			requestProcessingAverageBaseCounter.Increment();

			// Update time counter
			requestProcessingAverageTimeCounter.IncrementBy((UtcNow - pair.Value.AcceptTime).Ticks);

			// Update tasks counter
			activeTasksCounter.Decrement();

			return true;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initiates an asynchronous operation to try await the request.
		/// </summary>
		/// <returns>
		/// A <see cref="Task{TResult}" /> object of type <see cref="TryResult{T}" /> that represents the asynchronous operation.
		/// <see cref="Task{TResult}.Result" /> refers to the instance of <see cref="IServerRequestHandler" /> if operation was successful, <c>null</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract Task<TryResult<IServerRequestHandler>> TryAwaitRequestAsync();

		#endregion
	}
}