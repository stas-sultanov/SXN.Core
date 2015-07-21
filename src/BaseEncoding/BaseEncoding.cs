using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Represents a base-n encoding.
	/// </summary>
	public abstract class BaseEncoding
	{
		#region Constant and Static Fields

		private static readonly TryResult<Byte[]> decodeFailResult = TryResult<Byte[]>.CreateFail();

		private static readonly Byte[] emptyDecodeResult = new Byte[0];

		private static readonly TryResult<String> encodeFailResult = TryResult<String>.CreateFail();

		#endregion

		#region Fields

		/// <summary>
		/// The alphabet of the encoding.
		/// </summary>
		protected readonly String alphabet;

		/// <summary>
		/// The look-up table of the encoding.
		/// </summary>
		protected readonly Byte[] lookupTable;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="BaseEncoding"/> class.
		/// </summary>
		/// <param name="encodingBase">The base of the encoding.</param>
		/// <param name="alphabet">The alphabet.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet"/> is not equal to <see cref="Base"/>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected BaseEncoding(Byte encodingBase, String alphabet)
			: this(encodingBase, alphabet, BuildLookupTable(alphabet))
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="BaseEncoding"/> class.
		/// </summary>
		/// <param name="encodingBase">The base of the encoding.</param>
		/// <param name="alphabet">The alphabet.</param>
		/// <param name="lookupTable">The custom look-up table.</param>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet"/> is not equal to <see cref="Base"/>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="lookupTable"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="lookupTable"/> is not equal to 128.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected BaseEncoding(Byte encodingBase, String alphabet, Byte[] lookupTable)
		{
			// Check look-up table
			if (lookupTable == null)
			{
				throw new ArgumentNullException(nameof(lookupTable));
			}

			if (lookupTable.Length != 128)
			{
				throw new ArgumentException(@"The count of items is not equal to 128.", nameof(lookupTable));
			}

			// Check alphabet
			if (alphabet == null)
			{
				throw new ArgumentNullException(nameof(alphabet));
			}

			if (alphabet.Length != encodingBase)
			{
				throw new ArgumentException(@"The count of items is not equal to encoding base.", nameof(alphabet));
			}

			Base = encodingBase;

			this.alphabet = alphabet;

			this.lookupTable = lookupTable;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the alphabet.
		/// </summary>
		public String Alphabet => alphabet;

		/// <summary>
		/// Gets the base of the encoding.
		/// </summary>
		public Byte Base
		{
			get;
		}

		/// <summary>
		/// Gets the look-up table.
		/// </summary>
		public Byte[] LookupTable => lookupTable;

		#endregion

		#region Methods

		#region Static methods

		/// <summary>
		/// Builds a look-up table for the alphabet.
		/// </summary>
		/// <param name="alphabet">The alphabet.</param>
		/// <returns>The look-up table</returns>
		/// <exception cref="ArgumentNullException"><paramref name="alphabet"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">The count of items in the <paramref name="alphabet"/> is greater than 128.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte[] BuildLookupTable(String alphabet)
		{
			// Check alphabet argument
			if (alphabet == null)
			{
				throw new ArgumentNullException(nameof(alphabet));
			}

			if (alphabet.Length > 128)
			{
				throw new ArgumentException(@"The count of items is greater than 128.", nameof(alphabet));
			}

			// Initialize result array
			var result = new Byte[128];

			// Fill array
			for (Byte index = 0; index < 128; index++)
			{
				result[index] = 0xFF;
			}

			for (Byte index = 0; index < alphabet.Length; index++)
			{
				var code = alphabet[index];

				result[code] = index;
			}

			return result;
		}

		#endregion

		/// <summary>
		/// Decodes the specified string, which encodes binary data with <see cref="Alphabet"/>, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="data"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is <c>null</c>.</exception>
		/// <exception cref="FormatException">The format of <paramref name="data"/> is invalid.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Byte[] Decode(String data)
		{
			// Check data argument
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			// Initialize result array
			var result = DecodeInternal(data, 0, data.Length);

			if (result == null)
			{
				throw new FormatException(@"The format of the data is invalid.");
			}

			return result;
		}

		/// <summary>
		/// Decodes the specified string, which encodes binary data with <see cref="Alphabet"/>, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>The string representation of <paramref name="count"/> elements of <paramref name="data"/>, starting at position <paramref name="offset"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than<c>0</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is less than<c>0</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> plus <paramref name="count"/> is greater than count of items in <paramref name="data"/>.</exception>
		/// <exception cref="FormatException">The format of <paramref name="data"/> is invalid.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Byte[] Decode(String data, Int32 offset, Int32 count)
		{
			// Check data argument
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			// Check length argument
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), count, @"Is less than 0.");
			}

			// Check offset argument
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(offset), offset, @"Is less than 0.");
			}

			// Check data count
			if (offset > (data.Length - count))
			{
				throw new ArgumentOutOfRangeException(nameof(data), data.Length, @"Offset plus count is greater than the count of items in data.");
			}

			// Check count
			if (count == 0)
			{
				return emptyDecodeResult;
			}

			// Decode
			var result = DecodeInternal(data, offset, count);

			if (result == null)
			{
				throw new FormatException(@"The format of the data is invalid.");
			}

			return result;
		}

		/// <summary>
		/// Encodes an array of 8-bit unsigned integers to its equivalent string representation that is encoded with <see cref="Alphabet"/>.
		/// </summary>
		/// <param name="data">An array of 8-bit unsigned integers.</param>
		/// <returns>The string representation, of the contents of <paramref name="data"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String Encode(Byte[] data)
		{
			// Check data argument
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			// Check count
			if (data.Length == 0)
			{
				return String.Empty;
			}

			// Convert
			return EncodeInternal(data, 0, data.Length);
		}

		/// <summary>
		/// Encodes a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with <see cref="Alphabet"/>.
		/// </summary>
		/// <param name="data">An array of 8-bit unsigned integers.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>The string representation of <paramref name="count"/> elements of <paramref name="data"/>, starting at position <paramref name="offset"/>.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="data"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than<c>0</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> is less than<c>0</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="offset"/> plus <paramref name="count"/> is greater than count of items in <paramref name="data"/>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public String Encode(Byte[] data, Int32 offset, Int32 count)
		{
			// Check data argument
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			// Check length argument
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), count, @"Is less than 0.");
			}

			// Check offset argument
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(offset), offset, @"Is less than 0.");
			}

			// Check data count
			if (offset > (data.Length - count))
			{
				throw new ArgumentOutOfRangeException(nameof(data), data.Length, @"Offset plus count is greater than the count of items in data.");
			}

			// Check count
			if (count == 0)
			{
				return String.Empty;
			}

			// Convert
			return EncodeInternal(data, offset, count);
		}

		/// <summary>
		/// Tries to decode the specified string, which encodes binary data with <see cref="Alphabet"/>, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains array of 8-bit unsigned integers that is equivalent to <paramref name="data"/> if operation was successful, <c>null</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TryResult<Byte[]> TryDecode(String data)
		{
			// Check data argument
			if (data == null)
			{
				return decodeFailResult;
			}

			// Decode
			var result = DecodeInternal(data, 0, data.Length);

			return result == null ? decodeFailResult : TryResult<Byte[]>.CreateSuccess(result);
		}

		/// <summary>
		/// Tries to decode the specified string, which encodes binary data with <see cref="Alphabet"/>, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains array of 8-bit unsigned integers that is equivalent to <paramref name="count"/> elements of <paramref name="data"/> starting at position <paramref name="offset"/> if operation was successful, <c>null</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TryResult<Byte[]> TryDecode(String data, Int32 offset, Int32 count)
		{
			// Check data argument
			if (data == null)
			{
				return decodeFailResult;
			}

			// Check count argument
			if (count < 0)
			{
				return decodeFailResult;
			}

			// Check offset argument
			if (offset < 0)
			{
				return decodeFailResult;
			}

			// Check offset and count
			if (offset > (data.Length - count))
			{
				return decodeFailResult;
			}

			// Decode
			var result = DecodeInternal(data, offset, count);

			return result == null ? decodeFailResult : TryResult<Byte[]>.CreateSuccess(result);
		}

		/// <summary>
		/// Tries to encode an array of 8-bit unsigned integers to its equivalent string representation that is encoded with <see cref="Alphabet"/>.
		/// </summary>
		/// <param name="data">An array of 8-bit unsigned integers.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains the string representation of the contents of <paramref name="data"/> if operation was successful, <c>null</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TryResult<String> TryEncode(Byte[] data)
		{
			// Check data argument
			if (data == null)
			{
				return encodeFailResult;
			}

			// Encode
			var result = EncodeInternal(data, 0, data.Length);

			return TryResult<String>.CreateSuccess(result);
		}

		/// <summary>
		/// Tries to encode a subset of an array of 8-bit unsigned integers to its equivalent string representation that is encoded with <see cref="Alphabet"/>.
		/// </summary>
		/// <param name="data">An array of 8-bit unsigned integers.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains string representation of <paramref name="count"/> elements of <paramref name="data"/> starting at position <paramref name="offset"/> if operation was successful, <c>null</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TryResult<String> TryEncode(Byte[] data, Int32 offset, Int32 count)
		{
			// Check data
			if (data == null)
			{
				return encodeFailResult;
			}

			// Check count argument
			if (count < 0)
			{
				return encodeFailResult;
			}

			// Check offset argument
			if (offset < 0)
			{
				return encodeFailResult;
			}

			// Check offset and count
			if (offset > (data.Length - count))
			{
				return encodeFailResult;
			}

			// Encode
			var result = EncodeInternal(data, offset, count);

			return TryResult<String>.CreateSuccess(result);
		}

		#endregion

		#region Abstract methods

		/// <summary>
		/// Encodes an array of 8-bit unsigned integers to its equivalent string representation that is encoded with <see cref="Alphabet"/>.
		/// </summary>
		/// <param name="data">An array of 8-bit unsigned integers to convert.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>The string representation of <paramref name="count"/> elements of <paramref name="data"/>, starting at position <paramref name="offset"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract String EncodeInternal(Byte[] data, Int32 offset, Int32 count);

		/// <summary>
		/// Decodes the specified string, which encodes binary data with <see cref="Alphabet"/>, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="data">The string to convert.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="count">The number of elements of <paramref name="data"/> to convert.</param>
		/// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="count"/> elements of <paramref name="data"/>, starting at position <paramref name="offset"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract Byte[] DecodeInternal(String data, Int32 offset, Int32 count);

		#endregion
	}
}