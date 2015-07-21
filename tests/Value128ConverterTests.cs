using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="Value128"/> structure.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class Value128ConverterTests
	{
		#region Constant and Static Fields

		private const Int32 loadTestsIterationsCount = 0x80000;

		private const Int32 unitTestsIterationsCount = 0x10000;

		private static readonly Random random = new Random(DateTime.UtcNow.Millisecond);

		#endregion

		#region Test methods

		#region Guid

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertGuid()
		{
			var address = new Byte[16];

			for (var index = 0; index < unitTestsIterationsCount; index++)
			{
				// Generate Data
				random.NextBytes(address);

				// Get expected result
				var expectedResult = new Guid(address);

				// Convert to Value128
				var intermediateResult = Value128Converter.FromGuid(expectedResult);

				// Convert to Guid
				var actualResult = Value128Converter.ToGuid(intermediateResult);

				// Compare
				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion

		#region Load Tests

		[TestMethod]
		[TestCategory("LoadTests")]
		public void ConvertStringCustomVsNativeLoadTest()
		{
			// Initialize Data
			var testSamples = new Value128[loadTestsIterationsCount];

			for (var index = 0; index < loadTestsIterationsCount; index++)
			{
				testSamples[index] = (Value128) Guid.NewGuid();
			}

			// Encode
			var customIntermediateResults = new String[loadTestsIterationsCount];

			var nativeIntermediateResults = new String[loadTestsIterationsCount];

			var testResults1 = LoadTest.ExecuteCompare
				(
					"Value128Converter.TryToBase64String",
					index =>
					{
						var inputValue = testSamples[(Int32) index];

						customIntermediateResults[index] = Value128Converter.TryToBase64String(inputValue, Base64Encoding.Mime).Result;
					},
					"Convert.ToBase64String",
					index =>
					{
						var inputValue = testSamples[(Int32) index];

						nativeIntermediateResults[index] = Convert.ToBase64String((Byte[]) inputValue);
					},
					loadTestsIterationsCount
				);

			Trace.TraceInformation(testResults1.ToString());

			// Decode
			var customActualResults = new Value128[loadTestsIterationsCount];

			var decodedGuids = new Value128[loadTestsIterationsCount];

			var testResults2 = LoadTest.ExecuteCompare
				(
					"Value128Converter.TryFromBase64String",
					index =>
					{
						var inputValue = customIntermediateResults[index];

						customActualResults[index] = Value128Converter.TryFromBase64String(inputValue, 0, Base64Encoding.Mime).Result;
					},
					"Convert.FromBase64String",
					index =>
					{
						var inputValue = nativeIntermediateResults[index];

						decodedGuids[index] = Value128Converter.TryFromByteArray(Convert.FromBase64String(inputValue), 0).Result;
					}, loadTestsIterationsCount);

			Trace.TraceInformation(testResults2.ToString());
		}

		#endregion

		#region IPAddress

		[TestMethod]
		[TestCategory("UnitTests")]
		public void IPAddress()
		{
			// Try null
			var tryFromResult = Value128Converter.TryFromIPAddress(null);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			var address = new Byte[16];

			for (var index = 0; index < unitTestsIterationsCount; index++)
			{
				// Generate Data
				random.NextBytes(address);

				// Get expected result
				var expectedResult = new IPAddress(address);

				// Convert to Value128
				var intermediateResult = Value128Converter.TryFromIPAddress(expectedResult).Result;

				// Convert to IPAddress
				var actualResult = (IPAddress) intermediateResult;

				// Compare
				CollectionAssert.AreEqual(expectedResult.GetAddressBytes(), actualResult.GetAddressBytes());
			}
		}

		#endregion

		#region Byte Array

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TryByteArray()
		{
			// Try null
			var tryFromResult = Value128Converter.TryFromByteArray(null, 0);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad length
			tryFromResult = Value128Converter.TryFromByteArray(new Byte[15], 0);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad start index
			tryFromResult = Value128Converter.TryFromByteArray(new Byte[17], 2);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			var expectedResult = new Byte[16];

			for (var index = 0; index < unitTestsIterationsCount; index++)
			{
				// Generate expected result
				random.NextBytes(expectedResult);

				// Get intermediate result
				var intermediateResult = Value128Converter.TryFromByteArray(expectedResult, 0).Result;

				// Get actual result
				var actualResult = Value128Converter.ToByteArray(intermediateResult);

				// Compare
				CollectionAssert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion

		#endregion

		#region String

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TryBase64String()
		{
			// Try null alphabet
			var tryToResult = Value128Converter.TryToBase64String(Value128.Zero, null);

			Assert.IsFalse(tryToResult.Success, $"Actual result is {tryToResult.Result}");

			// Try null string
			var tryFromResult = Value128Converter.TryFromBase64String(null, 0, Base64Encoding.Lex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad length
			tryFromResult = Value128Converter.TryFromBase64String(@"0123456789ABCDEFGHIJ", 0, Base64Encoding.Lex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad start index
			tryFromResult = Value128Converter.TryFromBase64String(@"0123456789ABCDEFGHIJKL", 2, Base64Encoding.Lex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad symbol
			tryFromResult = Value128Converter.TryFromBase64String(@"0123456789ABCDEFGHIJK+", 0, Base64Encoding.Lex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad symbol
			tryFromResult = Value128Converter.TryFromBase64String(@"01234+6789ABCDEFGHIJKL", 0, Base64Encoding.Lex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			for (var index = 0; index < unitTestsIterationsCount; index++)
			{
				// Generate Data
				var expectedResult = (Value128) Guid.NewGuid();

				// Convert Value128 to String
				var intermediateResult = Value128Converter.TryToBase64String(expectedResult, Base64Encoding.Lex).Result;

				// Convert String to Value128
				var actualResult = Value128Converter.TryFromBase64String(intermediateResult, 0, Base64Encoding.Lex);

				Assert.IsTrue(actualResult.Success);

				// Compare
				Assert.AreEqual(expectedResult, actualResult.Result);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TryBase32String()
		{
			// Try null alphabet
			var tryToResult = Value128Converter.TryToBase32String(Value128.Zero, null);

			Assert.IsFalse(tryToResult.Success, $"Actual result is {tryToResult.Result}");

			// Try null string
			var tryFromResult = Value128Converter.TryFromBase32String(null, 0, Base32Encoding.Hex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad length
			tryFromResult = Value128Converter.TryFromBase32String(@"0123456789ABCDEFGHIJ", 0, Base32Encoding.Hex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad start index
			tryFromResult = Value128Converter.TryFromBase32String(@"0123456789ABCDEFGHIJKLMNOPQ", 2, Base32Encoding.Hex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad symbol
			tryFromResult = Value128Converter.TryFromBase32String(@"0123456789ABCDEFGHIJKLMNO+", 0, Base32Encoding.Hex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			// Try bad symbol
			tryFromResult = Value128Converter.TryFromBase32String(@"01234+6789ABCDEFGHIJKLMNOP", 0, Base32Encoding.Hex);

			Assert.IsFalse(tryFromResult.Success, $"Actual result is {tryFromResult.Result}");

			for (var index = 0; index < unitTestsIterationsCount; index++)
			{
				// Generate Data
				var expectedResult = (Value128) Guid.NewGuid();

				// Convert Value128 to String
				var intermediateResult = Value128Converter.TryToBase32String(expectedResult, Base32Encoding.Hex).Result;

				// Convert String to Value128
				var actualResult = Value128Converter.TryFromBase32String(intermediateResult, 0, Base32Encoding.Hex);

				Assert.IsTrue(actualResult.Success);

				// Compare
				Assert.AreEqual(expectedResult, actualResult.Result);
			}
		}

		/*
		[TestMethod]
		[TestCategory("UnitTests")]
		public void StringBase16()
		{
			// Try null
			var tryResult = Value128Converter.TryFrom(null, 0, BaseEncoding.Base16);

			Assert.IsFalse(tryResult.Success, String.Format("Actual result is {0}", tryResult.Result));

			// Try bad length
			tryResult = Value128Converter.TryFrom(@"0123456789ABCDE", 0, BaseEncoding.Base16);

			Assert.IsFalse(tryResult.Success, String.Format("Actual result is {0}", tryResult.Result));

			// Try bad start index
			tryResult = Value128Converter.TryFrom(@"00112233445566778899AABBCCDDEEFF", 2, BaseEncoding.Base16);

			Assert.IsFalse(tryResult.Success, String.Format("Actual result is {0}", tryResult.Result));

			// Try bad symbol
			tryResult = Value128Converter.TryFrom(@"00112233445566778899AABBCCDDEEZZ", 0, BaseEncoding.Base16);

			Assert.IsFalse(tryResult.Success, String.Format("Actual result is {0}", tryResult.Result));

			for (var index = 0; index < unitTestsIterationsCount; index++)
			{
				// Generate Data
				var initialData = Value128Converter.From(Guid.NewGuid());

				var expectedIntermediateResult = initialData.HigherHalf.ToString("X16") + initialData.LowerHalf.ToString("X16");

				// Convert Value128 to String
				var actualIntermediateResult = Value128Converter.ToString(initialData, BaseEncoding.Base16);

				// Compare
				Assert.AreEqual(expectedIntermediateResult, actualIntermediateResult);

				// Convert String to Value128
				var actualValue128Result = Value128Converter.TryFrom(actualIntermediateResult, 0, BaseEncoding.Base16);

				Assert.IsTrue(actualValue128Result.Success);

				// Compare
				Assert.AreEqual(initialData, actualValue128Result.Result);
			}
		}*/

		#endregion
	}
}