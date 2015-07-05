using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="DateTime" /> structure.
	/// </summary>
	public static class DateTimeEx
	{
		#region Constant and Static Fields

		/// <summary>
		/// UNIX start time.
		/// </summary>
		private static readonly DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		#endregion

		#region Methods

		/// <summary>
		/// Adds <paramref name="timeInterval" /> to the <paramref name="value" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeInterval">The time interval.</param>
		/// <returns>The new instance of <see cref="DateTime" /> with <paramref name="timeInterval" /> added.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Add(this DateTime value, TimeInterval timeInterval)
		{
			return value.AddTicks(timeInterval);
		}

		/// <summary>
		/// Adds <paramref name="timeUnit" /> to the <paramref name="value" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeUnit">A time interval.</param>
		/// <returns>The new instance of <see cref="DateTime" /> with <paramref name="timeUnit" /> added.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Add(this DateTime value, TimeUnit timeUnit)
		{
			return value.AddTicks((Int64) timeUnit);
		}

		/// <summary>
		/// Returns the smallest <see cref="DateTime" /> value that is greater than or equal to the specified <paramref name="value" /> within the specified <paramref name="timeUnit" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeUnit">A unit of time.</param>
		/// <returns>The smallest <see cref="DateTime" /> value that is greater than or equal to the specified <paramref name="value" /> within the specified <paramref name="timeUnit" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Ceiling(this DateTime value, TimeUnit timeUnit)
		{
			var ticks = value.Ticks;

			var ticksPerTimeUnit = (Int64) timeUnit;

			Int64 remainder;

			Math.DivRem(ticks, ticksPerTimeUnit, out remainder);

			return new DateTime(ticks + ticksPerTimeUnit - remainder);
		}

		/// <summary>
		/// Returns the largest <see cref="DateTime" /> that less than or equal to the specified <paramref name="value" /> within the specified <paramref name="timeUnit" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeUnit">A unit of time.</param>
		/// <returns>The largest <see cref="DateTime" /> that less than or equal to the specified <paramref name="value" /> within the specified <paramref name="timeUnit" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Floor(this DateTime value, TimeUnit timeUnit)
		{
			var ticks = value.Ticks;

			var ticksPerTimeUnit = (Int64) timeUnit;

			Int64 remainder;

			Math.DivRem(ticks, ticksPerTimeUnit, out remainder);

			return new DateTime(ticks - remainder);
		}

		/// <summary>
		/// Gets a <see cref="Boolean" /> value which specifies whether <paramref name="value" /> specifies last time interval.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeUnit">A unit of time.</param>
		/// <returns><c>true</c> if time unit is last, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean IsLastTimeInterval(this DateTime value, TimeUnit timeUnit)
		{
			switch (timeUnit)
			{
				case TimeUnit.Microsecond:
				{
					return value.Ticks % 10 == 9;
				}
				case TimeUnit.Millisecond:
				{
					return value.Millisecond == 999;
				}
				case TimeUnit.Second:
				{
					return value.Second == 59;
				}
				case TimeUnit.Minute:
				{
					return value.Minute == 59;
				}
				case TimeUnit.Hour:
				{
					return value.Hour == 23;
				}
				case TimeUnit.Day:
				{
					return DateTime.DaysInMonth(value.Year, value.Month) == value.Day;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets a <see cref="Boolean" /> value which indicates whether <paramref name="value" /> is multiple of the <paramref name="timeUnit" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeUnit">The time unit.</param>
		/// <returns><c>true</c> if <paramref name="value" /> is multiple of the <paramref name="timeUnit" />, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean IsMultipleOf(this DateTime value, TimeUnit timeUnit)
		{
			return value.Ticks % (Int64) timeUnit == 0;
		}

		/// <summary>
		/// Subtracts <paramref name="timeInterval" /> from the <paramref name="value" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeInterval">The time interval.</param>
		/// <returns>The new instance of <see cref="DateTime" /> with <paramref name="timeInterval" /> subtracted.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Subtract(this DateTime value, TimeInterval timeInterval)
		{
			return value.AddTicks(-timeInterval);
		}

		/// <summary>
		/// Subtracts <paramref name="timeUnit" /> from the <paramref name="value" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" />.</param>
		/// <param name="timeUnit">A time interval.</param>
		/// <returns>The new instance of <see cref="DateTime" /> with <paramref name="timeUnit" /> subtracted.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime Subtract(this DateTime value, TimeUnit timeUnit)
		{
			return value.AddTicks(-(Int64) timeUnit);
		}

		/// <summary>
		/// Converts an instance of <see cref="DateTime" /> structure to the UNIX time format.
		/// </summary>
		/// <param name="value">The instance of <see cref="DateTime" /> structure to be converted.</param>
		/// <returns>The UNIX representation of the time specified by the <paramref name="value" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int64 ToUnixTime(this DateTime value)
		{
			return (value - unixStart).Ticks / TimeSpan.TicksPerSecond;
		}

		#endregion
	}
}