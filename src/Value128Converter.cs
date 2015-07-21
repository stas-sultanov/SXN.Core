using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides conversion of <see cref="Value128"/> type to other types.
	/// </summary>
	public sealed class Value128Converter : TypeConverter
	{
		#region Constant and Static Fields

		/// <summary>
		/// Number of symbols in Base16 representation of <see cref="Value128"/>.
		/// </summary>
		private const UInt32 base16EncodedSymbolsCount = 32;

		/// <summary>
		/// Number of symbols in Base32 representation of <see cref="Value128"/>.
		/// </summary>
		private const UInt32 base32EncodedSymbolsCount = 26;

		/// <summary>
		/// Number of symbols in Base64 representation of <see cref="Value128"/>.
		/// </summary>
		private const UInt32 base64EncodedSymbolsCount = 22;

		private static readonly Type byteArrayType = typeof(Byte[]);

		private static readonly TryResult<Value128> convertFailResult = TryResult<Value128>.CreateFail();

		private static readonly TryResult<String> encodeFailResult = TryResult<String>.CreateFail();

		private static readonly Type guidType = typeof(Guid);

		private static readonly Type ipAddressType = typeof(IPAddress);

		private static readonly Type stringType = typeof(String);

		private static readonly Type value128Type = typeof(Value128);

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list of types to/from which <see cref="Value128"/> can be converted.
		/// </summary>
		public static IReadOnlyList<Type> SupportedTypes
		{
			get;
		} = Array.AsReadOnly
			(
				new[]
				{
					guidType, stringType, ipAddressType, byteArrayType
				}
			);

		#endregion

		#region Overrides of TypeConverter

		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from. </param>
		/// <returns><c>true</c> if this converter can perform the conversion; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return SupportedTypes.Contains(sourceType);
		}

		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
		/// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return SupportedTypes.Contains(destinationType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture. </param>
		/// <param name="value">The <see cref="T:System.Object"/> to convert. </param>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		/// <returns> An <see cref="T:System.Object"/> that represents the converted value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			// Check argument
			if (value == null)
			{
				return null;
			}

			// Get value
			var valueType = value.GetType();

			if (valueType == byteArrayType)
			{
				var convertResult = TryFromByteArray((Byte[]) value, 0);

				return convertResult.Success ? convertResult.Result : (Value128?) null;
			}

			if (valueType == guidType)
			{
				return FromGuid((Guid) value);
			}

			if (valueType == ipAddressType)
			{
				var convertResult = TryFromIPAddress((IPAddress) value);

				return convertResult.Success ? convertResult.Result : (Value128?) null;
			}

			if (valueType == stringType)
			{
				var convertResult = TryFromBase64String((String) value, 0, Base64Encoding.Lex);

				return convertResult.Success ? convertResult.Result : (Value128?) null;
			}

			return null;
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed. </param>
		/// <param name="value">The <see cref="T:System.Object"/> to convert. </param>
		/// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. </param>
		/// <returns>An <see cref="T:System.Object"/> that represents the converted value. </returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
		{
			if (value == null)
			{
				return null;
			}

			if (value.GetType() != value128Type)
			{
				return null;
			}

			if (destinationType == byteArrayType)
			{
				return ToByteArray((Value128) value);
			}

			if (destinationType == guidType)
			{
				return ToGuid((Value128) value);
			}

			if (destinationType == ipAddressType)
			{
				return ToIPAddress((Value128) value);
			}

			if (destinationType == stringType)
			{
				return TryToBase64String((Value128) value, Base64Encoding.Lex).Result;
			}

			return null;
		}

		#endregion

		#region Private methods

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Convert(UInt64 value, Int32 offset, IList<Byte> result)
		{
			result[offset + 0] = (Byte) (value >> 56);
			result[offset + 1] = (Byte) (value >> 48);
			result[offset + 2] = (Byte) (value >> 40);
			result[offset + 3] = (Byte) (value >> 32);
			result[offset + 4] = (Byte) (value >> 24);
			result[offset + 5] = (Byte) (value >> 16);
			result[offset + 6] = (Byte) (value >> 8);
			result[offset + 7] = (Byte) (value >> 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static UInt64 Convert(IList<Byte> value, Int32 offset)
		{
			return (UInt64) value[offset + 0] << 56 | (UInt64) value[offset + 1] << 48 | (UInt64) value[offset + 2] << 40 | (UInt64) value[offset + 3] << 32 | (UInt64) value[offset + 4] << 24 | (UInt64) value[offset + 5] << 16 | (UInt64) value[offset + 6] << 8 | (UInt64) value[offset + 7] << 0;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Converts an instance of <see cref="Guid"/> structure to the instance of <see cref="Value128"/> structure.
		/// </summary>
		/// <param name="value">An instance of <see cref="Guid"/> to convert.</param>
		/// <returns>An <see cref="Value128"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Value128 FromGuid(Guid value)
		{
			var bytes = value.ToByteArray();

			var higherHalf = ((UInt64) bytes[0] << 56) | ((UInt64) bytes[1] << 48) | ((UInt64) bytes[2] << 40) | ((UInt64) bytes[3] << 32) | ((UInt64) bytes[4] << 24) | ((UInt64) bytes[5] << 16) | ((UInt64) bytes[6] << 8) | bytes[7];

			var lowerHalf = ((UInt64) bytes[8] << 56) | ((UInt64) bytes[9] << 48) | ((UInt64) bytes[10] << 40) | ((UInt64) bytes[11] << 32) | ((UInt64) bytes[12] << 24) | ((UInt64) bytes[13] << 16) | ((UInt64) bytes[14] << 8) | bytes[15];

			return new Value128(higherHalf, lowerHalf);
		}

		/// <summary>
		/// Converts an instance of <see cref="Value128"/> structure to the <see cref="Array"/> of <see cref="Byte"/>.
		/// </summary>
		/// <param name="value">The instance of <see cref="Value128"/> to convert.</param>
		/// <returns>The <see cref="Array"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Byte[] ToByteArray(Value128 value)
		{
			var result = new Byte[16];

			Convert(value.HigherHalf, 0, result);

			Convert(value.LowerHalf, 8, result);

			return result;
		}

		/// <summary>
		/// Converts an instance of <see cref="Value128"/> structure to the instance of <see cref="Guid"/> structure.
		/// </summary>
		/// <param name="value">An instance of <see cref="Value128"/> to convert.</param>
		/// <returns>An <see cref="Guid"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Guid ToGuid(Value128 value)
		{
			var higherHalf = value.HigherHalf;

			var lowerHalf = value.LowerHalf;

			return new Guid(new[]
			{
				(Byte) (higherHalf >> 56), (Byte) (higherHalf >> 48), (Byte) (higherHalf >> 40), (Byte) (higherHalf >> 32), (Byte) (higherHalf >> 24), (Byte) (higherHalf >> 16), (Byte) (higherHalf >> 8), (Byte) (higherHalf >> 0), (Byte) (lowerHalf >> 56), (Byte) (lowerHalf >> 48), (Byte) (lowerHalf >> 40), (Byte) (lowerHalf >> 32), (Byte) (lowerHalf >> 24), (Byte) (lowerHalf >> 16), (Byte) (lowerHalf >> 8), (Byte) (lowerHalf >> 0)
			});
		}

		/// <summary>
		/// Converts an instance of <see cref="Value128"/> structure to the instance of <see cref="IPAddress"/> class.
		/// </summary>
		/// <param name="value">An instance of <see cref="Value128"/> to convert.</param>
		/// <returns>An <see cref="IPAddress"/> instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IPAddress ToIPAddress(Value128 value)
		{
			// Get value as byte array
			var address = ToByteArray(value);

			// Initialize a new instance of IPAddress
			return new IPAddress(address);
		}

		/// <summary>
		/// Tries to convert the specified string, which encodes <see cref="Value128"/> data with <paramref name="base32Encoding"/>, to an equivalent <see cref="Value128"/> structure.
		/// </summary>
		/// <param name="data">The string to convert. Must be at least <see cref="base32EncodedSymbolsCount"/> chars long.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="base32Encoding">The base-32 encoding.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains valid <see cref="Value128"/> if operation was successful, <see cref="Value128.Zero"/> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<Value128> TryFromBase32String(String data, Int32 offset, Base32Encoding base32Encoding)
		{
			// Check input values
			if ((data == null) || (offset > data.Length - base32EncodedSymbolsCount) || (base32Encoding == null))
			{
				return convertFailResult;
			}

			// Get look-up table
			var lookupTable = base32Encoding.LookupTable;

			var lastIndex = offset + 12;

			var symbol12 = (UInt64) lookupTable[data[lastIndex] & 0x7F];

			var symbol25 = (UInt64) lookupTable[data[offset + 25] & 0x7F];

			// Check symbol
			if ((symbol12 | symbol25) == 0xFF)
			{
				return convertFailResult;
			}

			// Calculate higher half
			var higherHalf = symbol12 >> 1;

			// Calculate lower half
			var lowerHalf = symbol12 << 63 | symbol25 >> 2;

			// Decode symbols
			for (Int32 indexH = offset, indexL = offset + 13, shiftH = 59, shiftL = 58; indexH < lastIndex; indexH++, indexL++, shiftH -= 5, shiftL -= 5)
			{
				// Get symbols
				var symbolH = data[indexH] & 0x7F;

				var symbolL = data[indexL] & 0x7F;

				// Get symbols values
				var symbolHValue = (UInt64) lookupTable[symbolH];

				var symbolLValue = (UInt64) lookupTable[symbolL];

				// Check symbol
				if ((symbolHValue | symbolLValue) == 0xFF)
				{
					return convertFailResult;
				}

				higherHalf |= symbolHValue << shiftH;

				lowerHalf |= symbolLValue << shiftL;
			}

			// Initialize a new instance
			var result = new Value128(higherHalf, lowerHalf);

			return TryResult<Value128>.CreateSuccess(result);
		}

		/// <summary>
		/// Tries to convert the specified string, which encodes <see cref="Value128"/> data with <paramref name="base64Encoding"/>, to an equivalent <see cref="Value128"/> structure.
		/// </summary>
		/// <param name="data">The string to convert. Must be at least <see cref="base64EncodedSymbolsCount"/> chars long.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <param name="base64Encoding">The base-64 encoding.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains valid <see cref="Value128"/> if operation was successful, <see cref="Value128.Zero"/> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<Value128> TryFromBase64String(String data, Int32 offset, Base64Encoding base64Encoding)
		{
			// Check input values
			if ((data == null) || (offset > data.Length - base64EncodedSymbolsCount) || (base64Encoding == null))
			{
				return convertFailResult;
			}

			// Get look-up table
			var lookupTable = base64Encoding.LookupTable;

			var lastIndex = offset + 10;

			var symbol10 = (UInt64) lookupTable[data[lastIndex] & 0x7F];

			var symbol21 = (UInt64) lookupTable[data[offset + 21] & 0x7F];

			// Check symbol
			if ((symbol10 | symbol21) == 0xFF)
			{
				return convertFailResult;
			}

			// Calculate higher half
			var higherHalf = symbol10 >> 2;

			// Calculate lower half
			var lowerHalf = symbol10 << 62 | symbol21 >> 4;

			// Decode symbols
			for (Int32 indexH = offset, indexL = offset + 11, shiftH = 58, shiftL = 56; indexH < lastIndex; indexH++, indexL++, shiftH -= 6, shiftL -= 6)
			{
				// Get symbols
				var symbolH = data[indexH] & 0x7F;

				var symbolL = data[indexL] & 0x7F;

				// Get symbols values
				var symbolHValue = (UInt64) lookupTable[symbolH];

				var symbolLValue = (UInt64) lookupTable[symbolL];

				// Check symbol
				if ((symbolHValue | symbolLValue) == 0xFF)
				{
					return convertFailResult;
				}

				higherHalf |= symbolHValue << shiftH;

				lowerHalf |= symbolLValue << shiftL;
			}

			// Initialize a new instance
			var result = new Value128(higherHalf, lowerHalf);

			return TryResult<Value128>.CreateSuccess(result);
		}

		/// <summary>
		/// Tries to convert a byte array representation of a <see cref="Value128"/> to the equivalent structure.
		/// </summary>
		/// <param name="data">The instance of <see cref="Array"/> to convert.</param>
		/// <param name="offset">An offset in <paramref name="data"/>.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains valid <see cref="Value128"/> if operation was successful, <see cref="Value128.Zero"/> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<Value128> TryFromByteArray(Byte[] data, Int32 offset)
		{
			if ((data == null) || (data.Length - offset < 16))
			{
				return convertFailResult;
			}

			// Convert higher half
			var higherHalf = Convert(data, offset);

			// Convert lower half
			var lowerHalf = Convert(data, offset + 8);

			// Initialize result
			var result = new Value128(higherHalf, lowerHalf);

			return TryResult<Value128>.CreateSuccess(result);
		}

		/// <summary>
		/// Tries to convert an instance of <see cref="IPAddress"/> to the equivalent instance of <see cref="Value128"/> structure.
		/// </summary>
		/// <param name="value">An instance of <see cref="IPAddress"/> to convert.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains valid <see cref="Value128"/> if operation was successful, <see cref="Value128.Zero"/> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<Value128> TryFromIPAddress(IPAddress value)
		{
			// Check input value
			if (value == null)
			{
				return convertFailResult;
			}

			// Map to IPv6
			var bytes = value.MapToIPv6().GetAddressBytes();

			// Try convert from byte array
			return TryFromByteArray(bytes, 0);
		}

		/// <summary>
		/// Tries to convert an instance of <see cref="Value128"/> structure to to its equivalent string representation that is encoded with <paramref name="base32Encoding"/>.
		/// </summary>
		/// <param name="data">The 128 bit value to convert.</param>
		/// <param name="base32Encoding">The base-32 encoding.</param>
		/// <returns>The string representation, of the <paramref name="data"/>.</returns>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains the string representation of the <paramref name="data"/> if operation was successful, <c>null</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<String> TryToBase32String(Value128 data, Base32Encoding base32Encoding)
		{
			if (base32Encoding == null)
			{
				return encodeFailResult;
			}

			Char[] result;

			// Get padding symbol
			var paddingSymbol = base32Encoding.PaddingSymbol;

			if (paddingSymbol.HasValue)
			{
				result = new Char[32];

				// Set padding
				result[26] = result[27] = result[28] = result[29] = result[30] = result[31] = paddingSymbol.Value;
			}
			else
			{
				result = new Char[26];
			}

			var higherHalf = data.HigherHalf;

			var lowerHalf = data.LowerHalf;

			// Get alphabet
			var alphabet = base32Encoding.Alphabet;

			for (Int32 indexH = 0, indexL = 13, shiftH = 59, shiftL = 58; indexH < 12; indexH++, indexL++, shiftH -= 5, shiftL -= 5)
			{
				result[indexH] = alphabet[(Int32) (higherHalf >> shiftH) & 0x1F];

				result[indexL] = alphabet[(Int32) (lowerHalf >> shiftL) & 0x1F];
			}

			result[12] = alphabet[(Int32) (((higherHalf << 1) & 0x1E) | ((lowerHalf >> 63) & 0x01))];

			result[25] = alphabet[(Int32) ((lowerHalf << 2) & 0x1C)];

			return TryResult<String>.CreateSuccess(new String(result));
		}

		/// <summary>
		/// Tries to convert an instance of <see cref="Value128"/> structure to to its equivalent string representation that is encoded with <paramref name="base64Encoding"/>.
		/// </summary>
		/// <param name="data">The 128 bit value to convert.</param>
		/// <param name="base64Encoding">The base-64 encoding.</param>
		/// <returns>The string representation, of the <paramref name="data"/>.</returns>
		/// <returns>
		/// An instance of <see cref="TryResult{T}"/> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success"/> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result"/> contains the string representation of the <paramref name="data"/> if operation was successful, <c>null</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<String> TryToBase64String(Value128 data, Base64Encoding base64Encoding)
		{
			if (base64Encoding == null)
			{
				return encodeFailResult;
			}

			Char[] result;

			// Get padding symbol
			var paddingSymbol = base64Encoding.PaddingSymbol;

			if (paddingSymbol.HasValue)
			{
				result = new Char[24];

				result[22] = result[23] = paddingSymbol.Value;
			}
			else
			{
				result = new Char[22];
			}

			var higherHalf = data.HigherHalf;

			var lowerHalf = data.LowerHalf;

			// Get alphabet
			var alphabet = base64Encoding.Alphabet;

			for (Int32 indexH = 0, indexL = 11, shiftH = 58, shiftL = 56; indexH < 10; indexH++, indexL++, shiftH -= 6, shiftL -= 6)
			{
				result[indexH] = alphabet[(Int32) (higherHalf >> shiftH) & 0x3F];

				result[indexL] = alphabet[(Int32) (lowerHalf >> shiftL) & 0x3F];
			}

			result[10] = alphabet[(Int32) (((higherHalf << 2) & 0x3C) | ((lowerHalf >> 62) & 0x03))];

			result[21] = alphabet[(Int32) ((lowerHalf << 4) & 0x30)];

			return TryResult<String>.CreateSuccess(new String(result));
		}

		#endregion
	}
}