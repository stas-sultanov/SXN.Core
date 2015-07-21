using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="Base32Encoding"/> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class Base32EncodingTests : BaseEncodingTests
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
		public void ConvertBase32Crockford()
		{
			ConvertTest(Base32Encoding.Crockford);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertBase32Hex()
		{
			ConvertTest(Base32Encoding.Hex);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TestCtor()
		{
			try // null alphabet
			{
				var x = new Base32Encoding(null);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // bad length alphabet
			{
				var x = new Base32Encoding("1245645");

				Assert.Fail(typeof(ArgumentException).ToString());
			}
			catch (ArgumentException)
			{
			}

			try // look-up table null
			{
				var x = new Base32Encoding(Base32Encoding.Hex.Alphabet, (Byte[]) null);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // look-up table bad length
			{
				var x = new Base32Encoding(Base32Encoding.Hex.Alphabet, new Byte[20]);

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
			TestDecodeMethodArguments(Base32Encoding.Hex);

			TestDecodeOffsetCountMethodArguments(Base32Encoding.Hex);

			try // bad count
			{
				var result = Base32Encoding.Hex.Decode("0");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // bad count
			{
				var result = Base32Encoding.Hex.Decode("012");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // bad count
			{
				var result = Base32Encoding.Hex.Decode("012345");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // bad symbol
			{
				var result = Base32Encoding.Hex.Decode("012+");

				Assert.Fail(typeof(FormatException).ToString());
			}
			catch (FormatException)
			{
			}

			try // good pass 1 byte result
			{
				var result = Base32Encoding.Hex.Decode("K0");

				Assert.IsTrue(result.SequenceEqual(testData.Take(1)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 2 byte result
			{
				var result = Base32Encoding.Hex.Decode("K3L0");

				Assert.IsTrue(result.SequenceEqual(testData.Take(2)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 3 byte result
			{
				var result = Base32Encoding.Hex.Decode("K3LBQ");

				Assert.IsTrue(result.SequenceEqual(testData.Take(3)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 4 byte result
			{
				var result = Base32Encoding.Hex.Decode("K3LBRA0");

				Assert.IsTrue(result.SequenceEqual(testData.Take(4)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 5 byte result
			{
				var result = Base32Encoding.Hex.Decode("K3LBRA43");

				Assert.IsTrue(result.SequenceEqual(testData.Take(5)));
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 6 byte result
			{
				var result = Base32Encoding.Hex.Decode("K3LBRA43PG");

				Assert.IsTrue(result.SequenceEqual(testData.Take(6)));
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
			TestEncodeMethodArguments(Base32Encoding.Hex);

			TestEncodeOffsetCountMethodArguments(Base32Encoding.Hex);

			try // good pass 1 byte length
			{
				var result = Base32Encoding.Hex.Encode(testData, 0, 1);

				Assert.AreEqual("K0", result);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 2 byte length
			{
				var result = Base32Encoding.Hex.Encode(testData, 0, 2);

				Assert.AreEqual("K3L0", result);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 3 byte length
			{
				var result = Base32Encoding.Hex.Encode(testData, 0, 3);

				Assert.AreEqual("K3LBQ", result);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 4 byte length
			{
				var result = Base32Encoding.Hex.Encode(testData, 0, 4);

				Assert.AreEqual("K3LBRA0", result);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 5 byte length
			{
				var result = Base32Encoding.Hex.Encode(testData, 0, 5);

				Assert.AreEqual("K3LBRA43", result);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass 6 byte length
			{
				var result = Base32Encoding.Hex.Encode(testData, 0, 6);

				Assert.AreEqual("K3LBRA43PG", result);
			}
			catch (Exception)
			{
				Assert.Fail();
			}
		}

		#endregion
	}
}