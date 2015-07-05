using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Represents a range.
	/// </summary>
	/// <typeparam name="T">The type of the range boundaries.</typeparam>
	public struct Range<T> : IComparable<Range<T>>, IComparable<T>, IEquatable<Range<T>>
		where T : IEquatable<T>, IComparable<T>
	{
		#region Constructors

		/// <summary>
		/// Initialize a new instance of <see cref="Range{T}" /> class.
		/// </summary>
		/// <param name="begin">A range begin.</param>
		/// <param name="end">A range end.</param>
		public Range(T begin, T end)
		{
			Begin = begin;

			End = end;
		}

		#endregion

		#region Overrides of ValueType

		/// <summary>
		/// Returns a value that indicates whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><c>true</c> if <paramref name="obj" /> is a <see cref="Range{T}" /> that has the same value as this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Equals(Object obj)
		{
			if (obj is Range<T>)
			{
				return Equals((Range<T>) obj);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override Int32 GetHashCode()
		{
			return Begin.GetHashCode() ^ End.GetHashCode();
		}

		#endregion

		#region Methods of IComparable<Range<T>>

		/// <summary>
		/// Compares this object with another <see cref="Range{T}" /> object.
		/// </summary>
		/// <param name="other">An <see cref="Range{T}" /> object to compare with this object.</param>
		/// <returns>A -1 if this range is less then other, +1 if this range is greater than other, 0 if ranges overlap.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Int32 CompareTo(Range<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			if (End.CompareTo(other.Begin) < 0)
			{
				// does not overlap and less then other
				return -1;
			}

			if (Begin.CompareTo(other.End) > 0)
			{
				// does not overlap and greater then other
				return +1;
			}

			// does overlap
			return 0;
		}

		#endregion

		#region Methods of IComparable<T>

		/// <summary>
		/// Compares this instance with another object.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A -1 if this range is less then other, +1 if this range is greater than other, 0 if ranges overlap.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Int32 CompareTo(T other)
		{
			if (End.CompareTo(other) < 0)
			{
				return -1;
			}

			if (Begin.CompareTo(other) > 0)
			{
				return +1;
			}

			return 0;
		}

		#endregion

		#region Methods of IEquatable<Range<T>>

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="Range{T}" /> represent the same value.
		/// </summary>
		/// <param name="other">An object to compare to this instance.</param>
		/// <returns><c>true</c> if <paramref name="other" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Boolean Equals(Range<T> other)
		{
			return Begin.Equals(other.Begin) && End.Equals(other.End);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Indicates whether the values of two specified <see cref="Range{T}" /> objects are equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator ==(Range<T> first, Range<T> second)
		{
			return first.Equals(second);
		}

		/// <summary>
		/// Defines whether <paramref name="first" /> is greater than <paramref name="second" />.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> is greater than <paramref name="second" />, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator >(Range<T> first, Range<T> second)
		{
			return first.CompareTo(second) > 0;
		}

		/// <summary>
		/// Indicates whether the values of two specified <see cref="Value128" /> objects are not equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are not equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator !=(Range<T> first, Range<T> second)
		{
			return !first.Equals(second);
		}

		/// <summary>
		/// Defines whether <paramref name="first" /> is less than <paramref name="second" />.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> is less than <paramref name="second" />, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator <(Range<T> first, Range<T> second)
		{
			return first.CompareTo(second) < 0;
		}

		#endregion

		#region Propreties

		/// <summary>
		/// Gets the begin of the range.
		/// </summary>
		public T Begin
		{
			get;
		}

		/// <summary>
		/// Gets the end of the range.
		/// </summary>
		public T End
		{
			get;
		}

		#endregion
	}
}