namespace System
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="TimeUnit" /> enumeration.
	/// </summary>
	public static class TimeUnitEx
	{
		#region Constant and Static Fields

		private static readonly TryResult<TimeUnit> dayResult = TryResult<TimeUnit>.CreateSuccess(TimeUnit.Day);

		private static readonly TryResult<TimeUnit> falseResult = TryResult<TimeUnit>.CreateFail();

		private static readonly TryResult<TimeUnit> hourResult = TryResult<TimeUnit>.CreateSuccess(TimeUnit.Hour);

		private static readonly TryResult<TimeUnit> millisecondResult = TryResult<TimeUnit>.CreateSuccess(TimeUnit.Millisecond);

		private static readonly TryResult<TimeUnit> minuteResult = TryResult<TimeUnit>.CreateSuccess(TimeUnit.Minute);

		private static readonly TryResult<TimeUnit> secondResult = TryResult<TimeUnit>.CreateSuccess(TimeUnit.Second);

		#endregion

		#region Methods

		/// <summary>
		/// Gets the next time unit.
		/// </summary>
		/// <param name="timeUnit">The base time unit.</param>
		/// <returns>The next time unit.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeUnit" /> is out of the valid range.</exception>
		public static TimeUnit GetNext(this TimeUnit timeUnit)
		{
			var result = timeUnit.TryGetNext();

			if (result.Success)
			{
				return result.Result;
			}

			throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, @"Is out of the valid range.");
		}

		/// <summary>
		/// Gets the previous time unit.
		/// </summary>
		/// <param name="timeUnit">The base time unit.</param>
		/// <returns>The previous time unit.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="timeUnit" /> is out of the valid range.</exception>
		public static TimeUnit GetPrevious(this TimeUnit timeUnit)
		{
			var result = timeUnit.TryGetPrevious();

			if (result.Success)
			{
				return result.Result;
			}

			throw new ArgumentOutOfRangeException(nameof(timeUnit), timeUnit, @"Is out of the valid range.");
		}

		/// <summary>
		/// Tries to get the next time unit.
		/// </summary>
		/// <param name="timeUnit">The base time unit.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}" /> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success" /> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result" /> contains the next time unit if operation was successful, <see cref="TimeUnit.None" /> otherwise.
		/// </returns>
		public static TryResult<TimeUnit> TryGetNext(this TimeUnit timeUnit)
		{
			switch (timeUnit)
			{
				case TimeUnit.Millisecond:
				{
					return secondResult;
				}
				case TimeUnit.Second:
				{
					return minuteResult;
				}
				case TimeUnit.Minute:
				{
					return hourResult;
				}
				case TimeUnit.Hour:
				{
					return dayResult;
				}
				default:
				{
					return falseResult;
				}
			}
		}

		/// <summary>
		/// Tries to get previous time unit.
		/// </summary>
		/// <param name="timeUnit">The base time unit.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}" /> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success" /> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result" /> contains the previous time unit if operation was successful, <see cref="TimeUnit.None" /> otherwise.
		/// </returns>
		public static TryResult<TimeUnit> TryGetPrevious(this TimeUnit timeUnit)
		{
			switch (timeUnit)
			{
				case TimeUnit.Day:
				{
					return hourResult;
				}
				case TimeUnit.Hour:
				{
					return minuteResult;
				}
				case TimeUnit.Minute:
				{
					return secondResult;
				}
				case TimeUnit.Second:
				{
					return millisecondResult;
				}
				default:
				{
					return falseResult;
				}
			}
		}

		#endregion
	}
}