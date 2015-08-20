using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Represents a base-16 encoding.
	/// </summary>
	public sealed class Base16Encoding : BaseEncoding
	{
		#region Constant and Static Fields

		/// <summary>
		/// The standard hexadecimal alphabet.
		/// </summary>
		private const String hexAlphabet = @"0123456789ABCDEF";

		/// <summary>
		/// The standard base-16 encoding
		/// </summary>
		public static readonly Base16Encoding Hex;

		/// <summary>
		/// The case invariant look-up table for the <see cref="hexAlphabet" />.
		/// </summary>
		private static readonly Byte[] hexLookupTable =
		{
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
		};

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="Base16Encoding" /> class.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Base16Encoding()
		{
			Hex = new Base16Encoding(hexAlphabet, hexLookupTable);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Base16Encoding" /> class.
		/// </summary>
		/// <param name="alphabet">The alphabet.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet" /> is not equal to 16.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Base16Encoding(String alphabet)
			: this(alphabet, BuildLookupTable(alphabet))
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Base16Encoding" /> class.
		/// </summary>
		/// <param name="alphabet">The alphabet.</param>
		/// <param name="lookupTable">The look-up table.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet" /> is not equal to 16.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="lookupTable" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="lookupTable" /> is not equal to 128.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Base16Encoding(String alphabet, Byte[] lookupTable)
			: base(16, alphabet, lookupTable)
		{
		}

		#endregion

		#region Overrides of BaseEncoding

		/// <summary>
		/// Decodes the specified string, which encodes binary data with <see cref="BaseEncoding.Alphabet" />, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <param name="offset">An offset in <paramref name="data" />.</param>
		/// <param name="count">The number of elements of <paramref name="data" /> to convert.</param>
		/// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="count" /> elements of <paramref name="data" />, starting at position <paramref name="offset" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Byte[] DecodeInternal(String data, Int32 offset, Int32 count)
		{
			if ((count % 2) != 0)
			{
				return null;
			}

			// Calc result items count
			var resultItemsCount = count / 2;

			// Initialize result array
			var result = new Byte[resultItemsCount];

			for (Int32 sourceIndex = offset, resultIndex = 0; resultIndex < resultItemsCount; sourceIndex += 2, resultIndex++)
			{
				// Get symbol
				var higherHalfSymbol = data[sourceIndex + 0] & 0x7F;

				// Get symbol value via look-up table
				var higherHalf = lookupTable[higherHalfSymbol];

				// Get symbol
				var lowerHalfSymbol = data[sourceIndex + 1] & 0x7F;

				// Get symbol value via look-up table
				var lowerHalf = lookupTable[lowerHalfSymbol];

				// Check if symbols are valid
				if ((higherHalf | lowerHalf) == 0xFF)
				{
					return null;
				}

				result[resultIndex] = (Byte) ((higherHalf << 4) | lowerHalf);
			}

			return result;
		}

		/// <summary>
		/// Encodes an array of 8-bit unsigned integers to its equivalent string representation that is encoded with <see cref="BaseEncoding.Alphabet" />.
		/// </summary>
		/// <param name="data">An array of 8-bit unsigned integers to convert.</param>
		/// <param name="offset">An offset in <paramref name="data" />.</param>
		/// <param name="count">The number of elements of <paramref name="data" /> to convert.</param>
		/// <returns>The string representation of <paramref name="count" /> elements of <paramref name="data" />, starting at position <paramref name="offset" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override String EncodeInternal(Byte[] data, Int32 offset, Int32 count)
		{
			var resultItemsCount = count * 2;

			// Initialize result array
			var resultArray = new Char[resultItemsCount];

			for (Int32 sourceIndex = offset, resultIndex = 0; resultIndex < resultItemsCount; sourceIndex++, resultIndex += 2)
			{
				var item = data[sourceIndex];

				resultArray[resultIndex + 0] = alphabet[(item >> 4) & 0x0F];

				resultArray[resultIndex + 1] = alphabet[item & 0x0F];
			}

			return new String(resultArray);
		}

		#endregion
	}
}