using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
	/// <summary>
	/// Provides sequential execution of tasks.
	/// </summary>
	public sealed class SequentialExecutor : IDisposable
	{
		#region Fields

		/// <summary>
		/// A queue of tasks to execute sequentially.
		/// </summary>
		private readonly ConcurrentQueue<Func<Task>> queue;

		/// <summary>
		/// A reference to the current task.
		/// </summary>
		private Task currentTask;

		#endregion

		#region Events

		/// <summary>
		/// Occurs when exception has been thrown by currently executing task.
		/// </summary>
		public event UnhandledExceptionEventHandler OnException;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="SequentialExecutor"/> class.
		/// </summary>
		public SequentialExecutor()
		{
			// Initialize queue
			queue = new ConcurrentQueue<Func<Task>>();

			// Initialize current task
			currentTask = Task.FromResult(0);
		}

		#endregion

		#region Methods of IDisposable

		/// <summary>
		/// Releases resources.
		/// </summary>
		public void Dispose()
		{
			// Wait for task to complete
			currentTask.Wait();

			// Dispose task
			currentTask.Dispose();
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Initiates an asynchronous operation to execute a sequence of functions from the work queue.
		/// </summary>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private async Task RunAsync()
		{
			Func<Task> work;

			while (queue.TryDequeue(out work))
			{
				try
				{
					// Execute work
					await work();
				}
				catch (Exception e)
				{
					// Check if there are subscribers and rise event
					OnException?.Invoke(this, new UnhandledExceptionEventArgs(e, false));
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Enqueues the <paramref name="function"/> to run sequentially.
		/// </summary>
		/// <param name="function">The work to execute sequentially.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Enqueue(Func<Task> function)
		{
			// Enqueue function into the execution queue
			queue.Enqueue(function);

			// Check if task is currently executing
			if (!currentTask.IsCompleted)
			{
				return;
			}

			// Execute new task
			var newTask = RunAsync();

			// Exchange ref
			var oldTask = Interlocked.Exchange(ref currentTask, newTask);

			// Dispose old task
			if (oldTask.IsCompleted)
			{
				oldTask.Dispose();
			}
		}

		/// <summary>
		/// Gets an awaiter used to await current instance.
		/// </summary>
		/// <returns>An instance of <see cref="TaskAwaiter"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TaskAwaiter GetAwaiter()
		{
			return currentTask.GetAwaiter();
		}

		/// <summary>
		/// Waits for the <see cref="SequentialExecutor"/> to complete execution.
		/// </summary>
		/// <exception cref="T:System.AggregateException"> The <see cref="Task"/> was canceled -or- an exception was thrown during the execution of the <see cref="Task"/>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Wait()
		{
			currentTask.Wait();
		}

		#endregion
	}
}