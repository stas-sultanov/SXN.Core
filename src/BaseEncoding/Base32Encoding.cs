using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Represents a base-32 encoding.
	/// </summary>
	public sealed class Base32Encoding : BaseEncoding
	{
		#region Constant and Static Fields

		/// <summary>
		/// The Crockford alphabet.
		/// </summary>
		private const String crockfordAlphabet = @"0123456789ABCDEFGHJKMNPQRSTVWXYZ";

		/// <summary>
		/// The hex alphabet.
		/// </summary>
		private const String hexAlphabet = @"0123456789ABCDEFGHIJKLMNOPQRSTUV";

		/// <summary>
		/// The Crockford encoding.
		/// </summary>
		public static readonly Base32Encoding Crockford;

		/// <summary>
		/// The look-up table for <see cref="crockfordAlphabet"/>.
		/// </summary>
		private static readonly Byte[] crockfordLookupTable =
		{
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x01, 0x12, 0x13, 0x01, 0x14, 0x15, 0x00,
			0x16, 0x17, 0x18, 0x19, 0x1A, 0xFF, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
			0xFF, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x01, 0x12, 0x13, 0x01, 0x14, 0x15, 0x00,
			0x16, 0x17, 0x18, 0x19, 0x1A, 0xFF, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
		};

		/// <summary>
		/// The hex encoding.
		/// </summary>
		public static readonly Base32Encoding Hex;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a <see cref="Base32Encoding"/> class.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Base32Encoding()
		{
			Crockford = new Base32Encoding(crockfordAlphabet, crockfordLookupTable);

			Hex = new Base32Encoding(hexAlphabet);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Base32Encoding"/> class.
		/// </summary>
		/// <param name="alphabet">The alphabet.</param>
		/// <param name="paddingSymbol">The padding symbol.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet"/> is not equal to 32.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Base32Encoding(String alphabet, Char? paddingSymbol = null)
			: this(alphabet, BuildLookupTable(alphabet), paddingSymbol)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Base32Encoding"/> class.
		/// </summary>
		/// <param name="alphabet">The alphabet.</param>
		/// <param name="lookupTable">The look-up table.</param>
		/// <param name="paddingSymbol">The padding symbol.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet"/> is not equal to 32.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="lookupTable"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="lookupTable"/> is not equal to 128.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Base32Encoding(String alphabet, Byte[] lookupTable, Char? paddingSymbol = null)
			: base(32, alphabet, lookupTable)
		{
			PaddingSymbol = paddingSymbol;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the padding symbol.
		/// </summary>
		public Char? PaddingSymbol
		{
			get;
		}

		#endregion

		#region Overrides of BaseEncoding

		/// <summary>
		/// Decodes the specified string, which encodes binary data with <see cref="BaseEncoding.Alphabet"/>, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="count"/> elements of <paramref name="data"/>, starting at position <paramref name="offset"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Byte[] DecodeInternal(String data, Int32 offset, Int32 count)
		{
			var sourceIndex = offset + count;

			// Exclude padding symbols from decode
			if (PaddingSymbol.HasValue)
			{
				while (data[sourceIndex - 1] == PaddingSymbol.Value)
				{
					count--;

					sourceIndex--;
				}
			}

			// Calc blocks count
			var blocksCount = count / 8;

			// Calc reminder
			var reminder = count % 8;

			Byte[] result;

			Int32 resultIndex;

			switch (reminder)
			{
				default:
				{
					// bad format
					return null;
				}
				case 0:
				{
					// Initialize result array
					result = new Byte[blocksCount * 5 + 0];

					// Set result index
					resultIndex = result.Length;

					break;
				}
				case 2:
				{
					// Initialize result array
					result = new Byte[blocksCount * 5 + 1];

					// Set result index
					resultIndex = result.Length;

					var a = lookupTable[data[--sourceIndex] & 0x7F];
					var b = lookupTable[data[--sourceIndex] & 0x7F];

					if ((a | b) == 0xFF)
					{
						// bad symbol
						return null;
					}

					// Compose buffer
					var buffer = a >> 2 | b << 3;

					// Decompose buffer
					result[--resultIndex] = (Byte) buffer;

					break;
				}
				case 4:
				{
					// Initialize result array
					result = new Byte[blocksCount * 5 + 2];

					// Set result index
					resultIndex = result.Length;

					var a = lookupTable[data[--sourceIndex] & 0x7F];
					var b = lookupTable[data[--sourceIndex] & 0x7F];
					var c = lookupTable[data[--sourceIndex] & 0x7F];
					var d = lookupTable[data[--sourceIndex] & 0x7F];

					if ((a | b | c | d) == 0xFF)
					{
						// bad symbol
						return null;
					}

					// Compose buffer
					var buffer = (a << 4) | (b << 9) | (c << 14) | (d << 19);

					// Decompose buffer
					result[--resultIndex] = (Byte) (buffer >> 8);
					result[--resultIndex] = (Byte) (buffer >> 16);

					break;
				}
				case 5:
				{
					// Initialize result array
					result = new Byte[blocksCount * 5 + 3];

					// Set result index
					resultIndex = result.Length;

					var a = lookupTable[data[--sourceIndex] & 0x7F];
					var b = lookupTable[data[--sourceIndex] & 0x7F];
					var c = lookupTable[data[--sourceIndex] & 0x7F];
					var d = lookupTable[data[--sourceIndex] & 0x7F];
					var e = lookupTable[data[--sourceIndex] & 0x7F];

					if ((a | b | c | d | e) == 0xFF)
					{
						// bad symbol
						return null;
					}

					// Compose buffer
					var buffer = (a << 7) | (b << 12) | (c << 17) | (d << 22) | (e << 27);

					// Decompose buffer
					result[--resultIndex] = (Byte) (buffer >> 8);
					result[--resultIndex] = (Byte) (buffer >> 16);
					result[--resultIndex] = (Byte) (buffer >> 24);

					break;
				}
				case 7:
				{
					// Initialize result array
					result = new Byte[blocksCount * 5 + 4];

					// Set result index
					resultIndex = result.Length;

					var a = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
					var b = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
					var c = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
					var d = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
					var e = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
					var f = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
					var g = (Int64) lookupTable[data[--sourceIndex] & 0x7F];

					if ((a | b | c | d | e | f | g) == 0xFF)
					{
						// bad symbol
						return null;
					}

					// Compose buffer
					var buffer = (a << 5) | (b << 10) | (c << 15) | (d << 20) | (e << 25) | (f << 30) | (g << 35);

					// Decompose buffer
					result[--resultIndex] = (Byte) (buffer >> 8);
					result[--resultIndex] = (Byte) (buffer >> 16);
					result[--resultIndex] = (Byte) (buffer >> 24);
					result[--resultIndex] = (Byte) (buffer >> 32);

					break;
				}
			}

			// Convert blocks
			for (var blockIndex = 0; blockIndex < blocksCount; blockIndex++)
			{
				var a = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
				var b = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
				var c = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
				var d = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
				var e = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
				var f = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
				var g = (Int64) lookupTable[data[--sourceIndex] & 0x7F];
				var h = (Int64) lookupTable[data[--sourceIndex] & 0x7F];

				if ((a | b | c | d | e | f | g | h) == 0xFF)
				{
					// bad symbol
					return null;
				}

				// Compose buffer
				var buffer = a | (b << 5) | (c << 10) | (d << 15) | (e << 20) | (f << 25) | (g << 30) | (h << 35);

				// Decompose buffer
				result[--resultIndex] = (Byte) (buffer);
				result[--resultIndex] = (Byte) (buffer >> 8);
				result[--resultIndex] = (Byte) (buffer >> 16);
				result[--resultIndex] = (Byte) (buffer >> 24);
				result[--resultIndex] = (Byte) (buffer >> 32);
			}

			return result;
		}

		/// <summary>
		/// Encodes an array of 8-bit unsigned integers to its equivalent string representation that is encoded with <see cref="BaseEncoding.Alphabet"/>.
		/// </summary>
		/// <param name="data">An array of 8-bit unsigned integers to convert.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>The string representation of <paramref name="count"/> elements of <paramref name="data"/>, starting at position <paramref name="offset"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override String EncodeInternal(Byte[] data, Int32 offset, Int32 count)
		{
			// Calc blocks count
			var blocksCount = count / 5;

			// Calc reminder
			var reminder = count % 5;

			var sourceIndex = offset + count;

			Int32 resultIndex;

			Char[] resultArray;

			switch (reminder)
			{
				default:
				{
					resultArray = new Char[blocksCount * 8];

					resultIndex = resultArray.Length;

					break;
				}
				case 1:
				{
					// Check if padding symbol should be used
					if (PaddingSymbol.HasValue)
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 8];

						// Set result index
						resultIndex = resultArray.Length;

						// Set padding symbol
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
					}
					else
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 2];

						// Set result index
						resultIndex = resultArray.Length;
					}

					// Compose buffer
					var buffer = data[--sourceIndex];

					// Decompose buffer
					resultArray[--resultIndex] = alphabet[(buffer & 0x07) << 2];
					resultArray[--resultIndex] = alphabet[(buffer & 0xF8) >> 3];

					break;
				}
				case 2:
				{
					// Check if padding symbol should be used
					if (PaddingSymbol.HasValue)
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 8];

						// Set result index
						resultIndex = resultArray.Length;

						// Set padding symbol
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
					}
					else
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 4];

						// Set result index
						resultIndex = resultArray.Length;
					}

					// Compose buffer
					var buffer = (data[--sourceIndex] << 8) | (data[--sourceIndex] << 16);

					// Decompose buffer
					resultArray[--resultIndex] = alphabet[(buffer >> 4) & 0x1F];
					resultArray[--resultIndex] = alphabet[(buffer >> 9) & 0x1F];
					resultArray[--resultIndex] = alphabet[(buffer >> 14) & 0x1F];
					resultArray[--resultIndex] = alphabet[(buffer >> 19) & 0x1F];

					break;
				}
				case 3:
				{
					// Check if padding symbol should be used
					if (PaddingSymbol.HasValue)
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 8];

						// Set result index
						resultIndex = resultArray.Length;

						// Set padding symbol
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
					}
					else
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 5];

						// Set result index
						resultIndex = resultArray.Length;
					}

					// Compose buffer
					var buffer = (data[--sourceIndex] << 8) | (data[--sourceIndex] << 16) | (data[--sourceIndex] << 24);

					// Decompose buffer
					resultArray[--resultIndex] = alphabet[(buffer >> 7) & 0x1F];
					resultArray[--resultIndex] = alphabet[(buffer >> 12) & 0x1F];
					resultArray[--resultIndex] = alphabet[(buffer >> 17) & 0x1F];
					resultArray[--resultIndex] = alphabet[(buffer >> 22) & 0x1F];
					resultArray[--resultIndex] = alphabet[(buffer >> 27) & 0x1F];

					break;
				}
				case 4:
				{
					// Check if padding symbol should be used
					if (PaddingSymbol.HasValue)
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 8];

						// Set result index
						resultIndex = resultArray.Length;

						// Set padding symbol
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
						resultArray[--resultIndex] = PaddingSymbol.Value;
					}
					else
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 8 + 7];

						// Set result index
						resultIndex = resultArray.Length;
					}

					// Compose buffer
					var buffer = ((Int64) data[--sourceIndex] << 8) | ((Int64) data[--sourceIndex] << 16) | ((Int64) data[--sourceIndex] << 24) | ((Int64) data[--sourceIndex] << 32);

					// Decompose buffer
					resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 5) & 0x1F];
					resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 10) & 0x1F];
					resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 15) & 0x1F];
					resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 20) & 0x1F];
					resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 25) & 0x1F];
					resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 30) & 0x1F];
					resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 35) & 0x1F];

					break;
				}
			}

			// Convert blocks of three bytes at a time to 4 chars.
			for (var blockIndex = 0; blockIndex < blocksCount; blockIndex++)
			{
				// Compose buffer
				var buffer = data[--sourceIndex] | ((Int64) data[--sourceIndex] << 8) | ((Int64) data[--sourceIndex] << 16) | ((Int64) data[--sourceIndex] << 24) | ((Int64) data[--sourceIndex] << 32);

				// Decompose buffer
				resultArray[--resultIndex] = alphabet[(Byte) buffer & 0x1F];
				resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 5) & 0x1F];
				resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 10) & 0x1F];
				resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 15) & 0x1F];
				resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 20) & 0x1F];
				resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 25) & 0x1F];
				resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 30) & 0x1F];
				resultArray[--resultIndex] = alphabet[(Byte) (buffer >> 35) & 0x1F];
			}

			return new String(resultArray);
		}

		#endregion
	}
}