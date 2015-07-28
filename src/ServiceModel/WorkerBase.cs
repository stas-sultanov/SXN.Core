using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	/// <summary>
	/// Provides a base class for the workers.
	/// </summary>
	public abstract class WorkerBase : DisposableBase, IWorker
	{
		#region Constant and Static Fields

		/// <summary>
		/// A list of the transitions of the state of the entity that are specific for the current worker.
		/// </summary>
		private static readonly IEnumerable<EntityStateTransition> workerWorkflow = new[]
		{
			new EntityStateTransition(EntityState.None, EntityState.Creating),
			new EntityStateTransition(EntityState.Creating, EntityState.Inactive),
			new EntityStateTransition(EntityState.Inactive, EntityState.Activating),
			new EntityStateTransition(EntityState.Activating, EntityState.Inactive),
			new EntityStateTransition(EntityState.Activating, EntityState.Active),
			new EntityStateTransition(EntityState.Active, EntityState.Deactivating),
			new EntityStateTransition(EntityState.Deactivating, EntityState.Inactive),
			new EntityStateTransition(EntityState.Inactive, EntityState.Retiring),
			new EntityStateTransition(EntityState.Retiring, EntityState.Retired)
		};

		#endregion

		#region Fields

		/// <summary>
		/// Occurs when diagnostic event within the instance has occurred.
		/// </summary>
		private readonly EventHandler<DiagnosticsEventArgs> diagnosticsEventHandler;

		/// <summary>
		/// The workflow of the sate of the instance.
		/// </summary>
		private readonly IEnumerable<EntityStateTransition> stateWorkflow;

		/// <summary>
		/// The stopwatch that measures the activity time.
		/// </summary>
		private readonly Stopwatch stopwatch;

		/// <summary>
		/// The UTC time when worker was started.
		/// </summary>
		private DateTime startTime;

		/// <summary>
		/// The state of the instance.
		/// </summary>
		private EntityState state;

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the state of the current instance is changed.
		/// </summary>
		public event EventHandler<ValueChangedEventArgs<EntityState>> StateChanged;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="WorkerBase"/> class.
		/// </summary>
		/// <param name="diagnosticsEventHandler">A delegate to the method that will handle the diagnostics events.</param>
		/// <param name="name">The name of the instance.</param>
		/// <param name="workflowExtension">The extension of the workflow.</param>
		/// <remarks>Constructor of the final class must set <see cref="State"/> to the <see cref="EntityState.Inactive"/> state.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="diagnosticsEventHandler"/> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected WorkerBase(EventHandler<DiagnosticsEventArgs> diagnosticsEventHandler, String name = null, IEnumerable<EntityStateTransition> workflowExtension = null)
		{
			if (diagnosticsEventHandler == null)
			{
				throw new ArgumentNullException(nameof(diagnosticsEventHandler));
			}

			// Set diagnostics event handler
			this.diagnosticsEventHandler = diagnosticsEventHandler;

			// Set name
			Id = name ?? GetType().Name;

			// Set workflow
			stateWorkflow = workflowExtension == null ? workerWorkflow : workerWorkflow.Concat(workflowExtension);

			// Signup on stat changing event
			StateChanged += OnStateChanged;

			// Set state to creating
			State = EntityState.Creating;

			// Initialize stopwatch
			stopwatch = new Stopwatch();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The time elapsed since start.
		/// </summary>
		protected TimeSpan ElapsedTime => stopwatch.Elapsed;

		/// <summary>
		/// The identifier of the worker.
		/// </summary>
		public String Id
		{
			get;
		}

		/// <summary>
		/// Gets a value which indicates whether current instance is active.
		/// </summary>
		public Boolean IsActive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return State == EntityState.Active;
			}
		}

		/// <summary>
		/// The state of the instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">State can not be changed from the current state to the target state.</exception>
		public EntityState State
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return state;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected set
			{
				// Change state
				var previusState = stateWorkflow.ChangeTo(ref state, value);

				// Check if there are subscribers on StateChanged event and rise event
				StateChanged?.Invoke(this, new ValueChangedEventArgs<EntityState>(previusState, state));
			}
		}

		/// <summary>
		/// The server time, expressed as the Coordinated Universal Time (UTC).
		/// </summary>
		protected DateTime UtcNow => startTime + stopwatch.Elapsed;

		#endregion

		#region Overrides of DisposableBase

		/// <summary>
		/// Releases managed resources held by the instance.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override sealed void ReleaseResources()
		{
			// Check if trying to release active object
			if (IsActive)
			{
				// Deactivate it
				DeactivateAsync(CancellationToken.None).Wait();
			}

			// Set state to retiring
			State = EntityState.Retiring;

			// Release managed resources
			ReleaseManagedResources();

			// Set state to retired
			State = EntityState.Retired;
		}

		#endregion

		#region Methods of IWorker

		/// <summary>
		/// Initiates an asynchronous operation to activate worker.
		/// </summary>
		/// <remarks>Requires worker to be in <see cref="EntityState.Inactive"/> state.</remarks>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> object of type <see cref="Boolean"/>> that represents the asynchronous operation.
		/// <see cref="Task{TResult}.Result"/> equals <c>true</c> if operation has completed successfully, <c>false</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async Task<Boolean> ActivateAsync(CancellationToken cancellationToken)
		{
			// Check whether operation is performed on inactive object
			if (State != EntityState.Inactive)
			{
				TraceEvent(EventLevel.Warning, $"Invalid attempt to change state from {State} to {EntityState.Inactive}.");

				return false;
			}

			// Set transitioning state
			State = EntityState.Activating;

			// Execute activate operations
			var isActivationOk = await OnActivatingAsync(cancellationToken);

			if (isActivationOk)
			{
				// Set solid state
				State = EntityState.Active;

				// Set start time
				startTime = DateTime.UtcNow;

				// Start stopwatch
				stopwatch.Start();
			}
			else
			{
				// Set solid state
				State = EntityState.Inactive;
			}

			return isActivationOk;
		}

		/// <summary>
		/// Initiates an asynchronous operation to deactivate worker.
		/// </summary>
		/// <remarks>Requires worker to be in <see cref="EntityState.Active"/> state.</remarks>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async Task DeactivateAsync(CancellationToken cancellationToken)
		{
			// Check whether operation is performed on active object
			if (State != EntityState.Active)
			{
				TraceEvent(EventLevel.Warning, $"Invalid attempt to change state from {State} to {EntityState.Inactive}.");

				return;
			}

			// Set transitioning state
			State = EntityState.Deactivating;

			// Reset start time
			startTime = DateTime.MinValue;

			// Reset stopwatch
			stopwatch.Reset();

			// Execute deactivate operations
			await OnDeactivatingAsync(cancellationToken);

			// Set solid state
			State = EntityState.Inactive;
		}

		/// <summary>
		/// Initiates an asynchronous operation to perform work.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		public abstract Task RunAsync(CancellationToken cancellationToken);

		#endregion

		#region Private methods

		/// <summary>
		/// Executes on <see cref="StateChanged"/> event.
		/// </summary>
		/// <param name="sender">The reference to the object that raised the event.</param>
		/// <param name="args">The event data.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void OnStateChanged(Object sender, ValueChangedEventArgs<EntityState> args)
		{
			TraceEvent(EventLevel.Informational, $"State changed from {args.PreviousValue} to {args.CurrentValue}.");
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initiates an asynchronous operation to perform operations required to activate worker.
		/// </summary>
		/// <remarks>Executes when <see cref="ActivateAsync"/> method is called</remarks>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> object of type <see cref="Boolean"/>> that represents the asynchronous operation.
		/// <see cref="Task{TResult}.Result"/> equals <c>true</c> if operation has completed successfully, <c>false</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract Task<Boolean> OnActivatingAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Initiates an asynchronous operation to perform operations required to deactivate worker.
		/// </summary>
		/// <remarks>Executes when <see cref="DeactivateAsync"/> method is called</remarks>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract Task OnDeactivatingAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Releases managed resources held by class.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void ReleaseManagedResources();

		/// <summary>
		/// Raises the diagnostics event handler.
		/// </summary>
		/// <param name="level">The level of the event.</param>
		/// <param name="message">The message of the event.</param>
		/// <param name="source">The source of the event.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TraceEvent(EventLevel level, String message, [CallerMemberName] String source = @"")
		{
			// Rise event
			diagnosticsEventHandler(this, new DiagnosticsEventArgs(level, message, source));
		}

		#endregion
	}
}