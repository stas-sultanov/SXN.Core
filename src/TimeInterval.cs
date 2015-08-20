using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Represents a time interval.
	/// </summary>
	public struct TimeInterval : IEquatable<TimeInterval>
	{
		#region Constant and Static Fields

		/// <summary>
		/// The day.
		/// </summary>
		public static readonly TimeInterval Day = new TimeInterval(TimeUnit.Day);

		/// <summary>
		/// The hour.
		/// </summary>
		public static readonly TimeInterval Hour = new TimeInterval(TimeUnit.Hour);

		/// <summary>
		/// The millisecond.
		/// </summary>
		public static readonly TimeInterval Millisecond = new TimeInterval(TimeUnit.Millisecond);

		/// <summary>
		/// The minute.
		/// </summary>
		public static readonly TimeInterval Minute = new TimeInterval(TimeUnit.Minute);

		/// <summary>
		/// The second.
		/// </summary>
		public static readonly TimeInterval Second = new TimeInterval(TimeUnit.Second);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="TimeInterval" /> structure.
		/// </summary>
		/// <param name="unit">The time unit.</param>
		/// <param name="length">The length.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TimeInterval(TimeUnit unit, Int64 length = 1)
		{
			Unit = unit;

			Length = length;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The length of the interval.
		/// </summary>
		public Int64 Length
		{
			get;
		}

		/// <summary>
		/// The time unit.
		/// </summary>
		public TimeUnit Unit
		{
			get;
		}

		#endregion

		#region Overrides of ValueType

		/// <summary>
		/// Returns a value that indicates whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><c>true</c> if <paramref name="obj" /> is a <see cref="TimeInterval" /> that has the same value as this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Equals(Object obj)
		{
			if (obj is TimeInterval)
			{
				return Equals((TimeInterval) obj);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override Int32 GetHashCode()
		{
			return Unit.GetHashCode() ^ Length.GetHashCode();
		}

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String" /> containing a fully qualified type name.
		/// </returns>
		public override String ToString()
		{
			return $"{Unit}:{Length}";
		}

		#endregion

		#region Methods of IEquatable<TimeInterval>

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="TimeInterval" /> represent the same value.
		/// </summary>
		/// <param name="other">An object to compare to this instance.</param>
		/// <returns><c>true</c> if <paramref name="other" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Boolean Equals(TimeInterval other)
		{
			return (Unit == other.Unit) && (Length == other.Length);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds <paramref name="length" /> to the <paramref name="interval" />.
		/// </summary>
		/// <param name="interval">The first value to add.</param>
		/// <param name="length">The second value to add.</param>
		/// <returns>The sum of <paramref name="interval" /> and <paramref name="length" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeInterval Add(TimeInterval interval, Int64 length)
		{
			return interval + length;
		}

		/// <summary>
		/// Adds <paramref name="length" /> to the <paramref name="interval" />.
		/// </summary>
		/// <param name="interval">The first value to add.</param>
		/// <param name="length">The second value to add.</param>
		/// <returns>The sum of <paramref name="interval" /> and <paramref name="length" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeInterval operator +(TimeInterval interval, Int64 length)
		{
			return new TimeInterval(interval.Unit, interval.Length + length);
		}

		/// <summary>
		/// Indicates whether the values of two specified <see cref="TimeInterval" /> objects are equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator ==(TimeInterval first, TimeInterval second)
		{
			return first.Equals(second);
		}

		/// <summary>
		/// Converts an instance of <see cref="TimeInterval" /> structure to the instance of <see cref="TimeSpan" /> structure.
		/// </summary>
		/// <param name="value">An instance of <see cref="TimeInterval" /> to convert.</param>
		/// <returns>A <see cref="TimeSpan" /> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator TimeSpan(TimeInterval value)
		{
			return new TimeSpan(value);
		}

		/// <summary>
		/// Converts an instance of <see cref="TimeInterval" /> structure to the instance of <see cref="Int64" /> structure.
		/// </summary>
		/// <param name="value">An instance of <see cref="TimeInterval" /> to convert.</param>
		/// <returns>The <see cref="Int64" /> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Int64(TimeInterval value)
		{
			return value.Length * (Int64) value.Unit;
		}

		/// <summary>
		/// Indicates whether the values of two specified <see cref="TimeInterval" /> objects are not equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are not equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator !=(TimeInterval first, TimeInterval second)
		{
			return !first.Equals(second);
		}

		/// <summary>
		/// Subtracts <paramref name="length" /> from the <paramref name="interval" />.
		/// </summary>
		/// <param name="interval">The minuend.</param>
		/// <param name="length">The subtrahend.</param>
		/// <returns>The result of subtracting <paramref name="length" /> from <paramref name="interval" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeInterval operator -(TimeInterval interval, Int64 length)
		{
			return new TimeInterval(interval.Unit, interval.Length - length);
		}

		/// <summary>
		/// Subtracts <paramref name="length" /> from the <paramref name="interval" />.
		/// </summary>
		/// <param name="interval">The minuend.</param>
		/// <param name="length">The subtrahend.</param>
		/// <returns>The result of subtracting <paramref name="length" /> from <paramref name="interval" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TimeInterval Subtract(TimeInterval interval, Int64 length)
		{
			return interval - length;
		}

		#endregion
	}
}