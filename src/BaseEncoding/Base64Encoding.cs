using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Represents a base-64 encoding.
	/// </summary>
	public sealed class Base64Encoding : BaseEncoding
	{
		#region Constant and Static Fields

		/// <summary>
		/// The unpadded lexicographically sorted alphabet.
		/// </summary>
		private const String lexAlphabet = @"-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";

		/// <summary>
		/// The transfer alphabet for MIME, defined by the <a href='https://tools.ietf.org/html/rfc2045'>RFC 2045</a>.
		/// </summary>
		private const String mimeAlphabet = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

		/// <summary>
		/// The padding symbol of <see cref="mimeAlphabet"/>.
		/// </summary>
		private const Char mimePaddingSymbol = '=';

		/// <summary>
		/// The unpadded url safe alphabet.
		/// </summary>
		private const String urlAlphabet = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

		/// <summary>
		/// The unpadded lexicographically sorted encoding.
		/// </summary>
		public static readonly Base64Encoding Lex;

		/// <summary>
		/// The MIME encoding.
		/// </summary>
		public static readonly Base64Encoding Mime;

		/// <summary>
		/// The unpadded url safe encoding.
		/// </summary>
		public static readonly Base64Encoding Url;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="Base64Encoding"/> class.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Base64Encoding()
		{
			Lex = new Base64Encoding(lexAlphabet);

			Mime = new Base64Encoding(mimeAlphabet, mimePaddingSymbol);

			Url = new Base64Encoding(urlAlphabet);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Base64Encoding"/> class.
		/// </summary>
		/// <param name="alphabet">The alphabet.</param>
		/// <param name="paddingSymbol">The padding symbol.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet"/> is not equal to 64.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Base64Encoding(String alphabet, Char? paddingSymbol = null)
			: this(alphabet, BuildLookupTable(alphabet), paddingSymbol)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="Base64Encoding"/> class.
		/// </summary>
		/// <param name="alphabet">The alphabet.</param>
		/// <param name="lookupTable">The look-up table.</param>
		/// <param name="paddingSymbol">The padding symbol.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet"/> is not equal to 64.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="lookupTable"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="lookupTable"/> is not equal to 128.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Base64Encoding(String alphabet, Byte[] lookupTable, Char? paddingSymbol)
			: base(64, alphabet, lookupTable)
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
			var blocksCount = count / 4;

			// Calc reminder
			var reminder = count % 4;

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
					result = new Byte[blocksCount * 3];

					// Set result index
					resultIndex = result.Length;

					break;
				}
				case 2:
				{
					// Initialize result array
					result = new Byte[blocksCount * 3 + 1];

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
					var buffer = a >> 4 | b << 2;

					// Decompose buffer
					result[--resultIndex] = (Byte) buffer;

					break;
				}
				case 3:
				{
					// Initialize result array
					result = new Byte[blocksCount * 3 + 2];

					// Set result index
					resultIndex = result.Length;

					var a = lookupTable[data[--sourceIndex] & 0x7F];
					var b = lookupTable[data[--sourceIndex] & 0x7F];
					var c = lookupTable[data[--sourceIndex] & 0x7F];

					if ((a | b | c) == 0xFF)
					{
						// bad symbol
						return null;
					}

					// Compose buffer
					var buffer = (a << 6) | (b << 12) | (c << 18);

					// Decompose buffer
					result[--resultIndex] = (Byte) (buffer >> 8);
					result[--resultIndex] = (Byte) (buffer >> 16);

					break;
				}
			}

			// Convert blocks
			for (var blockIndex = 0; blockIndex < blocksCount; blockIndex++)
			{
				var a = (Int32) lookupTable[data[--sourceIndex] & 0x7F];
				var b = (Int32) lookupTable[data[--sourceIndex] & 0x7F];
				var c = (Int32) lookupTable[data[--sourceIndex] & 0x7F];
				var d = (Int32) lookupTable[data[--sourceIndex] & 0x7F];

				if ((a | b | c | d) == 0xFF)
				{
					// bad symbol
					return null;
				}

				// Compose buffer
				var buffer = a | (b << 6) | (c << 12) | (d << 18);

				// Decompose buffer
				result[--resultIndex] = (Byte) (buffer);
				result[--resultIndex] = (Byte) (buffer >> 8);
				result[--resultIndex] = (Byte) (buffer >> 16);
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
			var blocksCount = count / 3;

			// Calc reminder
			var reminder = count % 3;

			Char[] resultArray;

			Int32 resultIndex;

			var sourceIndex = offset + count;

			switch (reminder)
			{
				default:
				{
					resultArray = new Char[blocksCount * 4];

					resultIndex = resultArray.Length;

					break;
				}
				case 1:
				{
					// Check if padding symbol should be used
					if (PaddingSymbol.HasValue)
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 4 + 4];

						// Set result index
						resultIndex = resultArray.Length;

						// Set padding symbol
						resultArray[--resultIndex] = PaddingSymbol.Value;

						resultArray[--resultIndex] = PaddingSymbol.Value;
					}
					else
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 4 + 2];

						// Set result index
						resultIndex = resultArray.Length;
					}

					// Compose buffer
					var buffer = data[--sourceIndex];

					resultArray[--resultIndex] = alphabet[(buffer & 0x03) << 4];

					resultArray[--resultIndex] = alphabet[(buffer & 0xfc) >> 2];

					break;
				}
				case 2:
				{
					// Check if padding symbol should be used
					if (PaddingSymbol.HasValue)
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 4 + 4];

						// Set result index
						resultIndex = resultArray.Length;

						// Set padding symbol
						resultArray[--resultIndex] = PaddingSymbol.Value;
					}
					else
					{
						// Initialize result array
						resultArray = new Char[blocksCount * 4 + 3];

						// Set result index
						resultIndex = resultArray.Length;
					}

					// Compose buffer
					var buffer = (data[--sourceIndex] << 8) | (data[--sourceIndex] << 16);

					resultArray[--resultIndex] = alphabet[(buffer >> 6) & 0x3F];

					resultArray[--resultIndex] = alphabet[(buffer >> 12) & 0x3F];

					resultArray[--resultIndex] = alphabet[(buffer >> 18) & 0x3F];

					break;
				}
			}

			// Convert blocks of three bytes at a time to 4 chars.
			for (var blockIndex = 0; blockIndex < blocksCount; blockIndex++)
			{
				// Combine 3 bytes in one 24 bit buffer
				var buffer = (data[--sourceIndex]) | (data[--sourceIndex] << 8) | (data[--sourceIndex] << 16);

				resultArray[--resultIndex] = alphabet[buffer & 0x3f];

				resultArray[--resultIndex] = alphabet[(buffer >> 6) & 0x3F];

				resultArray[--resultIndex] = alphabet[(buffer >> 12) & 0x3F];

				resultArray[--resultIndex] = alphabet[(buffer >> 18) & 0x3F];
			}

			return new String(resultArray);
		}

		#endregion
	}
}