using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="Base16Encoding" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class Base16EncodingTests : BaseEncodingTests
	{
		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertBase16Hex()
		{
			ConvertTest(Base16Encoding.Hex);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TestCtor()
		{
			try // null alphabet
			{
				var x = new Base16Encoding(null, Base16Encoding.Hex.LookupTable);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // bad length alphabet
			{
				var x = new Base16Encoding("1245645", Base16Encoding.Hex.LookupTable);

				Assert.Fail(typeof(ArgumentException).ToString());
			}
			catch (ArgumentException)
			{
			}

			try // look-up table null
			{
				var x = new Base16Encoding(Base16Encoding.Hex.Alphabet, null);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // look-up table bad length
			{
				var x = new Base16Encoding(Base16Encoding.Hex.Alphabet, new Byte[20]);

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
			TestDecodeMethodArguments(Base16Encoding.Hex);

			TestDecodeOffsetCountMethodArguments(Base16Encoding.Hex);

			Byte[] result;

			try // bad count
			{
				result = Base16Encoding.Hex.Decode("abc");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // bad symbol
			{
				result = Base16Encoding.Hex.Decode("a+");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // good pass
			{
				result = Base16Encoding.Hex.Decode("ab");
			}
			catch (Exception)
			{
				Assert.Fail();
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TestEncodeArguments()
		{
			TestEncodeMethodArguments(Base16Encoding.Hex);

			TestEncodeOffsetCountMethodArguments(Base16Encoding.Hex);

			var testData = new Byte[]
			{
				0, 1, 2, 3
			};

			try // good pass 1 byte length
			{
				Base16Encoding.Hex.Encode(testData, 0, 1);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 2 byte length
			{
				Base16Encoding.Hex.Encode(testData, 0, 2);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 3 byte length
			{
				Base16Encoding.Hex.Encode(testData, 0, 3);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 4 byte length
			{
				Base16Encoding.Hex.Encode(testData, 0, 4);
			}
			catch (Exception)
			{
				Assert.Fail();
			}
		}

		#endregion
	}
}