using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
	/// <summary>
	/// Represents a 128 bit value.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(Value128Converter))]
	public struct Value128 : IComparable<Value128>, IEquatable<Value128>, ISerializable
	{
		#region Constant and Static Fields

		/// <summary>
		/// A read-only instance of the <see cref="Value128" /> structure which represents 1 number.
		/// </summary>
		public static readonly Value128 One = new Value128(0, 1);

		/// <summary>
		/// A read-only instance of the <see cref="Value128" /> structure whose value is all zeros.
		/// </summary>
		public static readonly Value128 Zero = new Value128(0, 0);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="Value128" /> structure.
		/// </summary>
		/// <param name="higherHalf">A highest 64 bit of 128 bit value.</param>
		/// <param name="lowerHalf">A lowest 64 bit of 128 bit value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Value128(UInt64 higherHalf, UInt64 lowerHalf)
			: this()
		{
			HigherHalf = higherHalf;

			LowerHalf = lowerHalf;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Value128" /> structure.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Value128(SerializationInfo info, StreamingContext context)
		{
			HigherHalf = info.GetUInt64(@"H");

			LowerHalf = info.GetUInt64(@"L");
		}

		#endregion

		#region Overrides of ValueType

		/// <summary>
		/// Returns a value that indicates whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><c>true</c> if <paramref name="obj" /> is a <see cref="Value128" /> that has the same value as this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Equals(Object obj)
		{
			if (obj is Value128)
			{
				return Equals((Value128) obj);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override Int32 GetHashCode()
		{
			return HigherHalf.GetHashCode() ^ LowerHalf.GetHashCode();
		}

		/// <summary>
		/// Converts instance of <see cref="Value128" /> to the instance of <see cref="String" /> using <see cref="Base64Encoding.Lex" /> encoding.
		/// </summary>
		/// <returns>An instance of <see cref="String" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override String ToString()
		{
			return Value128Converter.TryToBase64String(this, Base64Encoding.Lex).Result;
		}

		#endregion

		#region Methods of IComparable<Value128>

		/// <summary>
		/// Compares this instance to a specified <see cref="Value128" /> object and returns an indication of their relative values.
		/// </summary>
		/// <param name="other">An object to compare to this instance.</param>
		/// <returns>A signed number indicating the relative values of this instance and <paramref name="other" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Int32 CompareTo(Value128 other)
		{
			var difference = HigherHalf.CompareTo(other.HigherHalf);

			return difference == 0 ? LowerHalf.CompareTo(other.LowerHalf) : difference;
		}

		#endregion

		#region Methods of IEquatable<Value128>

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="Value128" /> represent the same value.
		/// </summary>
		/// <param name="other">An object to compare to this instance.</param>
		/// <returns><c>true</c> if <paramref name="other" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Boolean Equals(Value128 other)
		{
			return (HigherHalf == other.HigherHalf) && (LowerHalf == other.LowerHalf);
		}

		#endregion

		#region Methods of ISerializable

		/// <summary>
		/// Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo" /> to populate with data. </param>
		/// <param name="context">The destination <see cref="StreamingContext" /> for this serialization. </param>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(@"H", HigherHalf);

			info.AddValue(@"L", LowerHalf);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Adds two specified <see cref="Value128" /> values.
		/// </summary>
		/// <param name="first">The first value to add.</param>
		/// <param name="second">The second value to add.</param>
		/// <returns>The sum of <paramref name="first" /> and <paramref name="second" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Value128 operator +(Value128 first, Value128 second)
		{
			var diff = (((first.LowerHalf & second.LowerHalf) & 1) + (first.LowerHalf >> 1) + (second.LowerHalf >> 1)) >> 63;

			var higherHalf = first.HigherHalf + second.HigherHalf + diff;

			var lowerHalf = first.LowerHalf + second.LowerHalf;

			return new Value128(higherHalf, lowerHalf);
		}

		/// <summary>
		/// Indicates whether the values of two specified <see cref="Value128" /> objects are equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator ==(Value128 first, Value128 second)
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
		public static Boolean operator >(Value128 first, Value128 second)
		{
			if (first.HigherHalf == second.HigherHalf)
			{
				return first.LowerHalf > second.LowerHalf;
			}

			return first.HigherHalf > second.HigherHalf;
		}

		/// <summary>
		/// Indicates whether the values of two specified <see cref="Value128" /> objects are not equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are not equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator !=(Value128 first, Value128 second)
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
		public static Boolean operator <(Value128 first, Value128 second)
		{
			if (first.HigherHalf == second.HigherHalf)
			{
				return first.LowerHalf < second.LowerHalf;
			}

			return first.HigherHalf < second.HigherHalf;
		}

		/// <summary>
		/// Subtracts one specified <see cref="Value128" /> value from another.
		/// </summary>
		/// <param name="first">The minuend.</param>
		/// <param name="second">The subtrahend.</param>
		/// <returns>The result of subtracting <paramref name="second" /> from <paramref name="first" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Value128 operator -(Value128 first, Value128 second)
		{
			var lowerHalf = first.LowerHalf - second.LowerHalf;

			var diff = (((lowerHalf & second.LowerHalf) & 1) + (second.LowerHalf >> 1) + (lowerHalf >> 1)) >> 63;

			var higherHalf = first.HigherHalf - (second.HigherHalf + diff);

			return new Value128(higherHalf, lowerHalf);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds two specified <see cref="Value128" /> values.
		/// </summary>
		/// <param name="first">The first value to add.</param>
		/// <param name="second">The second value to add.</param>
		/// <returns>The sum of <paramref name="first" /> and <paramref name="second" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Value128 Add(Value128 first, Value128 second)
		{
			return first + second;
		}

		/// <summary>
		/// Subtracts one specified <see cref="Value128" /> value from another.
		/// </summary>
		/// <param name="first">The minuend.</param>
		/// <param name="second">The subtrahend.</param>
		/// <returns>The result of subtracting <paramref name="second" /> from <paramref name="first" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Value128 Subtract(Value128 first, Value128 second)
		{
			return first - second;
		}

		#endregion

		#region Propreties

		/// <summary>
		/// The higher half of the value.
		/// </summary>
		public UInt64 HigherHalf
		{
			get;
		}

		/// <summary>
		/// The lower half of the value.
		/// </summary>
		public UInt64 LowerHalf
		{
			get;
		}

		#endregion

		#region Explicit cast operators

		/// <summary>
		/// Converts instance of <see cref="Value128" /> to the instance of <see cref="String" /> using Base64 encoding with padding.
		/// </summary>
		/// <param name="value">An instance of <see cref="Value128" /> to convert.</param>
		/// <returns>An instance of <see cref="String" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator String(Value128 value)
		{
			return value.ToString();
		}

		/// <summary>
		/// Converts an instance of <see cref="Value128" /> structure to the instance of <see cref="Array" /> class.
		/// </summary>
		/// <param name="value">An instance of <see cref="Value128" /> to convert.</param>
		/// <returns>An <see cref="Array" /> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Byte[](Value128 value)
		{
			return Value128Converter.ToByteArray(value);
		}

		/// <summary>
		/// Converts an instance of <see cref="Value128" /> structure to the instance of <see cref="IPAddress" /> class.
		/// </summary>
		/// <param name="value">An instance of <see cref="Value128" /> to convert.</param>
		/// <returns>An <see cref="IPAddress" /> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator IPAddress(Value128 value)
		{
			return Value128Converter.ToIPAddress(value);
		}

		/// <summary>
		/// Converts an instance of <see cref="Guid" /> structure to the instance of <see cref="Value128" /> structure.
		/// </summary>
		/// <param name="value">An instance of <see cref="Guid" /> to convert.</param>
		/// <returns>An <see cref="Value128" /> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Value128(Guid value)
		{
			return Value128Converter.FromGuid(value);
		}

		/// <summary>
		/// Converts an instance of <see cref="Value128" /> structure to the instance of <see cref="Guid" /> structure.
		/// </summary>
		/// <param name="value">An instance of <see cref="Value128" /> to convert.</param>
		/// <returns>An <see cref="Guid" /> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator Guid(Value128 value)
		{
			return Value128Converter.ToGuid(value);
		}

		#endregion
	}
}