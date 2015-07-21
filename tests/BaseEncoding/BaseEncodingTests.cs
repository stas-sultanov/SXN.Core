using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="BaseEncoding"/> class.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public abstract class BaseEncodingTests
	{
		#region Constant and Static Fields

		protected const Int32 LoadTestsIterationsCount = 0x100000;

		protected const Int32 UnitTestsIterationsCount = 0x10000;

		protected static readonly Random Random = new Random(DateTime.UtcNow.Millisecond);

		#endregion

		#region Methods

		protected static void ConvertTest(BaseEncoding encoding)
		{
			var initialData = new Byte[128];

			var odd = false;

			for (var index = 0; index < UnitTestsIterationsCount; index++, odd = !odd)
			{
				// Generate test data
				Random.NextBytes(initialData);

				var offset = odd ? 0 : Random.Next(64);

				var count = odd ? 128 : Random.Next(64);

				// Convert byte array to string
				var intermediateResult = odd ? encoding.Encode(initialData) : encoding.Encode(initialData, offset, count);

				// Convert string to byte array
				var actualResult = encoding.Decode(intermediateResult, 0, intermediateResult.Length);

				// Compare by item
				for (Int32 expectedResultIndex = offset, actualResultIndex = 0; expectedResultIndex < offset + count; expectedResultIndex++, actualResultIndex++)
				{
					var expectedItem = initialData[expectedResultIndex];

					var actualItem = actualResult[actualResultIndex];

					Assert.AreEqual(expectedItem, actualItem);
				}
			}
		}

		protected static void TestDecodeMethodArguments(BaseEncoding encoding)
		{
			try // null
			{
				encoding.Decode(null);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}
		}

		protected static void TestDecodeOffsetCountMethodArguments(BaseEncoding encoding)
		{
			try // null
			{
				encoding.Decode(null, 0, 2);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // bad count
			{
				encoding.Decode("abcd", 0, -4);

				Assert.Fail(typeof(ArgumentOutOfRangeException).ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
			}

			try // bad offset
			{
				encoding.Decode("abcd", -1, 4);

				Assert.Fail(typeof(ArgumentOutOfRangeException).ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
			}

			try // bad offset
			{
				encoding.Decode("abcd", 1, 4);

				Assert.Fail(typeof(ArgumentOutOfRangeException).ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}

		protected static void TestEncodeMethodArguments(BaseEncoding encoding)
		{
			var testData = new Byte[]
			{
				0, 1, 2, 3
			};

			try // null data
			{
				encoding.Encode(null);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // good pass zero length
			{
				encoding.Encode(new Byte[0]);
			}
			catch (Exception)
			{
				Assert.Fail();
			}

			try // good pass
			{
				encoding.Encode(testData);
			}
			catch (Exception)
			{
				Assert.Fail();
			}
		}

		protected static void TestEncodeOffsetCountMethodArguments(BaseEncoding encoding)
		{
			var testData = new Byte[]
			{
				0, 1, 2, 3
			};

			try // null data
			{
				encoding.Encode(null, 0, 2);

				Assert.Fail(typeof(ArgumentNullException).ToString());
			}
			catch (ArgumentNullException)
			{
			}

			try // bad length
			{
				encoding.Encode(testData, 0, -4);

				Assert.Fail(typeof(ArgumentOutOfRangeException).ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
			}

			try // bad offset
			{
				encoding.Encode(testData, -1, 4);

				Assert.Fail(typeof(ArgumentOutOfRangeException).ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
			}

			try // bad offset
			{
				encoding.Encode(testData, 1, 4);

				Assert.Fail(typeof(ArgumentOutOfRangeException).ToString());
			}
			catch (ArgumentOutOfRangeException)
			{
			}

			try // good pass zero length
			{
				encoding.Encode(testData, 0, 0);
			}
			catch (Exception)
			{
				Assert.Fail();
			}
		}

		#endregion
	}
}