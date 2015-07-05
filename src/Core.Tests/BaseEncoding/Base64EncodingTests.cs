using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="Base64Encoding" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class Base64EncodingTests : BaseEncodingTests
	{
		#region Constant and Static Fields

		private static readonly Byte[] testData =
		{
			160, 234, 189, 168, 131, 204
		};

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void CompareWithNative()
		{
			var initialData = new Byte[128];

			var odd = false;

			for (var index = 0; index < UnitTestsIterationsCount; index++, odd = !odd)
			{
				// Generate test data
				Random.NextBytes(initialData);

				var offset = odd ? 0 : Random.Next(64);

				var length = odd ? 128 : Random.Next(64);

				// Convert byte array to string
				var intermediateResult = odd ? Base64Encoding.Mime.Encode(initialData) : Base64Encoding.Mime.Encode(initialData, offset, length);

				// Convert byte array to string
				var intermediateNativeResult = odd ? Convert.ToBase64String(initialData) : Convert.ToBase64String(initialData, offset, length);

				// Compare native and custom
				Assert.AreEqual(intermediateNativeResult, intermediateResult);

				/*
				// Convert string to byte array
				var actualResult = encoding.Decode(intermediateResult, 0, intermediateResult.Length);

				// Compare by item
				for (int expectedResultIndex = offset, actualResultIndex = 0; expectedResultIndex < offset + length; expectedResultIndex++, actualResultIndex++)
				{
					var expectedItem = initialData[expectedResultIndex];

					var actualItem = actualResult[actualResultIndex];

					Assert.AreEqual(expectedItem, actualItem);
				}*/
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertBase64Lex()
		{
			ConvertTest(Base64Encoding.Lex);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertBase64Mime()
		{
			ConvertTest(Base64Encoding.Mime);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertBase64Url()
		{
			ConvertTest(Base64Encoding.Url);
		}

		[TestMethod]
		[TestCategory("LoadTests")]
		public void ConvertStringCustomVsNativeLoadTest()
		{
			// Initialize Data
			var testSamples = new Byte[LoadTestsIterationsCount][];

			for (var index = 0; index < LoadTestsIterationsCount; index++)
			{
				testSamples[index] = new Byte[128];

				// Generate test data
				Random.NextBytes(testSamples[index]);
			}

			// Encode
			var customIntermediateResults = new String[LoadTestsIterationsCount];

			var nativeIntermediateResults = new String[LoadTestsIterationsCount];

			var testResults1 = LoadTest.ExecuteCompare
				(
					"Base64Encoding.Mime.Encode",
					index =>
					{
						var inputValue = testSamples[(Int32) index];

						customIntermediateResults[index] = Base64Encoding.Mime.Encode(inputValue);
					},
					"Convert.ToBase64String",
					index =>
					{
						var inputValue = testSamples[(Int32) index];

						nativeIntermediateResults[index] = Convert.ToBase64String(inputValue);
					},
					LoadTestsIterationsCount
				);

			Trace.TraceInformation(testResults1.ToString());

			// Decode
			var customActualResults = new Byte[LoadTestsIterationsCount][];

			var nativeActualResults = new Byte[LoadTestsIterationsCount][];

			var testResults2 = LoadTest.ExecuteCompare
				(
					"Base64Encoding.Mime.Decode",
					index =>
					{
						var inputValue = customIntermediateResults[index];

						customActualResults[index] = Base64Encoding.Mime.Decode(inputValue);
					},
					"Convert.FromBase64String",
					index =>
					{
						var inputValue = nativeIntermediateResults[index];

						nativeActualResults[index] = Convert.FromBase64String(inputValue);
					},
					LoadTestsIterationsCount
				);

			Trace.TraceInformation(testResults2.ToString());
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TestCtor()
		{
			try // null alphabet
			{
				var x = new Base64Encoding(null, Base64Encoding.Mime.LookupTable, '=');

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // bad length alphabet
			{
				var x = new Base64Encoding("1245645", Base64Encoding.Mime.LookupTable, null);

				Assert.Fail(typeof(ArgumentException).ToString());
			}
			catch (ArgumentException)
			{
			}

			try // look-up table null
			{
				var x = new Base64Encoding(Base64Encoding.Mime.Alphabet, null, '=');

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // look-up table bad length
			{
				var x = new Base64Encoding(Base64Encoding.Mime.Alphabet, new Byte[20], null);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentException)
			{
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TestDecodeArguments()
		{
			TestDecodeMethodArguments(Base64Encoding.Lex);

			TestDecodeOffsetCountMethodArguments(Base64Encoding.Lex);

			try // no padding bad count
			{
				Base64Encoding.Lex.Decode("a");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // padding bad count
			{
				Base64Encoding.Mime.Decode("abcde");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // bad symbol
			{
				Base64Encoding.Lex.Decode("a+");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // no padding bad count
			{
				Base64Encoding.Lex.Decode("abc", 0, 1);

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // good pass 1 byte result
			{
				var result = Base64Encoding.Url.Decode("oA");

				Assert.IsTrue(result.SequenceEqual(testData.Take(1)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 2 byte result
			{
				var result = Base64Encoding.Url.Decode("oOo");

				Assert.IsTrue(result.SequenceEqual(testData.Take(2)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 3 byte result
			{
				var result = Base64Encoding.Url.Decode("oOq9");

				Assert.IsTrue(result.SequenceEqual(testData.Take(3)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 3 byte result
			{
				var result = Base64Encoding.Url.Decode("oOq9");

				Assert.IsTrue(result.SequenceEqual(testData.Take(3)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 4 byte result
			{
				var result = Base64Encoding.Url.Decode("oOq9qA");

				Assert.IsTrue(result.SequenceEqual(testData.Take(4)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // bad symbol
			{
				Base64Encoding.Mime.Decode("%b==", 0, 4);

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TestEncodeArguments()
		{
			TestEncodeMethodArguments(Base64Encoding.Lex);

			TestEncodeOffsetCountMethodArguments(Base64Encoding.Lex);

			try // good pass 1 byte length
			{
				var result = Base64Encoding.Url.Encode(testData, 0, 1);

				Assert.AreEqual(2, result.Length);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 2 byte length
			{
				var result = Base64Encoding.Url.Encode(testData, 0, 2);

				Assert.AreEqual(3, result.Length);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 3 byte length
			{
				var result = Base64Encoding.Url.Encode(testData, 0, 3);

				Assert.AreEqual(4, result.Length);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 4 byte length
			{
				var result = Base64Encoding.Url.Encode(testData, 0, 4);

				Assert.AreEqual(6, result.Length);
			}
			catch (Exception)
			{
				Assert.Fail();
			}
		}

		#endregion
	}
}