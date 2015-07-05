using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="ulong" /> class.
	/// </summary>
	public static class UInt32Ex
	{
		#region Methods

		/// <summary>
		/// Aligns <paramref name="value" /> to the <paramref name="powerOfTwoBase" />.
		/// </summary>
		/// <param name="value">The unsigned integer value.</param>
		/// <param name="powerOfTwoBase">The power of two align base.</param>
		/// <returns>Aligned value.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="powerOfTwoBase" /> is not power of two.</exception>
		public static UInt32 Align(this UInt32 value, UInt32 powerOfTwoBase)
		{
			if (!powerOfTwoBase.IsPowerOfTwo())
			{
				throw new ArgumentException("Is not power of two", nameof(powerOfTwoBase));
			}

			if (value == 0)
			{
				return powerOfTwoBase;
			}

			// get power
			var reminder = value & (powerOfTwoBase - 1);

			if (reminder == 0)
			{
				return value;
			}

			var power = powerOfTwoBase.GetHighestSetBitIndex();

			var multiplier = value >> power;

			return (multiplier + 1) << power;
		}

		/// <summary>
		/// Gets the zero based index of the highest set bit.
		/// </summary>
		/// <param name="value">The unsigned integer value.</param>
		/// <returns>The index of the highest set bit.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Int32 GetHighestSetBitIndex(this UInt32 value)
		{
			if (value == 0)
			{
				return 0;
			}

			var result = 0;

			if ((value & 0x0000FFFF) == 0)
			{
				result += 16;
			}

			if ((value & 0x00FF00FF) == 0)
			{
				result += 8;
			}

			if ((value & 0x0F0F0F0F) == 0)
			{
				result += 4;
			}

			if ((value & 0x33333333) == 0)
			{
				result += 2;
			}

			if ((value & 0x55555555) == 0)
			{
				result += 1;
			}

			return result;
		}

		/// <summary>
		/// Gets the count of set bits.
		/// </summary>
		/// <param name="value">The unsigned integer value.</param>
		/// <returns>The count of set bits.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static UInt32 GetSetBitsCount(this UInt32 value)
		{
			value = value - ((value >> 1) & 0x55555555);

			value = (value & 0x33333333) + ((value >> 2) & 0x33333333);

			value = (value + (value >> 4)) & 0x0F0F0F0F;

			value = value + (value >> 8);

			value = value + (value >> 16);

			return value & 0x3F;
		}

		/// <summary>
		/// Gets a <see cref="Boolean" /> value which indicates whether <paramref name="value" /> is power of 2.
		/// </summary>
		/// <param name="value">The unsigned integer value.</param>
		/// <returns><c>true</c> if <paramref name="value" /> is power of 2, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean IsPowerOfTwo(this UInt32 value)
		{
			if (value == 0)
			{
				return false;
			}

			return (value & (value - 1)) == 0;
		}

		#endregion
	}
}