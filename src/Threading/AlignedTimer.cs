using System.Runtime.CompilerServices;

namespace System.Threading
{
	/// <summary>
	/// Represents a timer which generates recurring events aligned to the <see cref="TimeUnit"/>.
	/// </summary>
	public sealed class AlignedTimer : IDisposable
	{
		#region Fields

		private readonly AlignedTimerCallback callback;

		private readonly Timer timer;

		private Boolean enabled;

		private DateTime nextInvocationTime;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="AlignedTimer"/> class.
		/// </summary>
		/// <param name="callback">A <see cref="AlignedTimerCallback"/> delegate to the method to execute.</param>
		/// <param name="isBlocking">The boolean value that indicates if next schedule is blocked until previous execution has completed.</param>
		/// <param name="interval">The time interval between invocations of <paramref name="callback"/>.</param>
		/// <param name="shift">The time shift of the invocation time.</param>
		/// <exception cref="ArgumentNullException"><paramref name="callback"/> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public AlignedTimer(AlignedTimerCallback callback, Boolean isBlocking, TimeUnit interval, TimeSpan shift)
		{
			if (callback == null)
			{
				throw new ArgumentNullException(nameof(callback));
			}

			this.callback = callback;

			IsBlocking = isBlocking;

			Interval = interval;

			Shift = shift;

			// Initialize timer
			timer = new Timer(TimerCallback);

			// Calculate current aligned time
			nextInvocationTime = DateTime.UtcNow.Floor(Interval);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the <see cref="Boolean"/> value which indicates whether current timer is active.
		/// </summary>
		public Boolean Enabled
		{
			get
			{
				return enabled;
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				enabled = value;

				if (enabled)
				{
					// Schedule next execution
					ScheduleNextInvocation();
				}
				else
				{
					// Stop timer
					timer.Change(Timeout.Infinite, Timeout.Infinite);
				}
			}
		}

		/// <summary>
		/// The boolean value that indicates if next schedule is blocked until previous execution has completed.
		/// </summary>
		public Boolean IsBlocking
		{
			get;
		}

		/// <summary>
		/// The time interval between invocations of the callback.
		/// </summary>
		public TimeUnit Interval
		{
			get;
		}

		/// <summary>
		/// The time shift of the invocation time.
		/// </summary>
		public TimeSpan Shift
		{
			get;
		}

		#endregion

		#region Methods of IDisposable

		/// <summary>
		/// Releases all resources used by <see cref="AlignedTimer"/>.
		/// </summary>
		public void Dispose()
		{
			timer.Dispose();
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Schedules next invocation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ScheduleNextInvocation()
		{
			do
			{
				// Increment time
				nextInvocationTime = nextInvocationTime.Add(Interval);
			}
			while (nextInvocationTime <= DateTime.UtcNow);

			// Calculate due time
			var dueTime = nextInvocationTime - DateTime.UtcNow + Shift;

			// Change timer
			timer.Change(dueTime, Timeout.InfiniteTimeSpan);
		}

		/// <summary>
		/// Handles the calls from the <see cref="timer"/>.
		/// </summary>
		/// <param name="state">An object containing application-specific information relevant to the method invoked by this method, or null.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void TimerCallback(Object state)
		{
			// Set fire time
			var fireTime = nextInvocationTime;

			if (IsBlocking)
			{
				// Execute action
				callback(fireTime);

				if (enabled)
				{
					// Schedule next execution
					ScheduleNextInvocation();
				}
			}
			else
			{
				if (enabled)
				{
					// Schedule next execution
					ScheduleNextInvocation();
				}

				// Execute action
				callback(fireTime);
			}
		}

		#endregion
	}
}