using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a set of tests for <see cref="ArrayEx" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class ArrayExTests
	{
		#region Nested Types

		private struct TestSample
		{
			#region Properties

			public Int32 Count
			{
				get;

				set;
			}

			public ArraySegment<Int32>[] ExpecedResult
			{
				get;

				set;
			}

			public ArraySegment<Int32> ExpecedRemainder
			{
				get;

				set;
			}

			#endregion
		}

		#endregion

		#region Constant and Static Fields

		private static readonly Int32[] testInputSamples =
		{
			1, 2, 3, 5, 7, 9, 11, 13, 17, 19
		};

		private static readonly TestSample[] testSamples;

		#endregion

		#region Constructor

		static ArrayExTests()
		{
			testSamples = new[]
			{
				new TestSample
				{
					Count = 1,
					ExpecedResult = new[]
					{
						new ArraySegment<Int32>(testInputSamples, 0, 10)
					},
					ExpecedRemainder = new ArraySegment<Int32>(testInputSamples, 10, 0)
				},
				new TestSample
				{
					Count = 2,
					ExpecedResult = new[]
					{
						new ArraySegment<Int32>(testInputSamples, 0, 5),
						new ArraySegment<Int32>(testInputSamples, 5, 5)
					},
					ExpecedRemainder = new ArraySegment<Int32>(testInputSamples, 10, 0)
				},
				new TestSample
				{
					Count = 3,
					ExpecedResult = new[]
					{
						new ArraySegment<Int32>(testInputSamples, 0, 3),
						new ArraySegment<Int32>(testInputSamples, 3, 3),
						new ArraySegment<Int32>(testInputSamples, 6, 3)
					},
					ExpecedRemainder = new ArraySegment<Int32>(testInputSamples, 9, 1)
				},
				new TestSample
				{
					Count = 5,
					ExpecedResult = new[]
					{
						new ArraySegment<Int32>(testInputSamples, 0, 2),
						new ArraySegment<Int32>(testInputSamples, 2, 2),
						new ArraySegment<Int32>(testInputSamples, 4, 2),
						new ArraySegment<Int32>(testInputSamples, 6, 2),
						new ArraySegment<Int32>(testInputSamples, 8, 2)
					},
					ExpecedRemainder = new ArraySegment<Int32>(testInputSamples, 10, 0)
				},
				new TestSample
				{
					Count = 10,
					ExpecedResult = new[]
					{
						new ArraySegment<Int32>(testInputSamples, 0, 1),
						new ArraySegment<Int32>(testInputSamples, 1, 1),
						new ArraySegment<Int32>(testInputSamples, 2, 1),
						new ArraySegment<Int32>(testInputSamples, 3, 1),
						new ArraySegment<Int32>(testInputSamples, 4, 1),
						new ArraySegment<Int32>(testInputSamples, 5, 1),
						new ArraySegment<Int32>(testInputSamples, 6, 1),
						new ArraySegment<Int32>(testInputSamples, 7, 1),
						new ArraySegment<Int32>(testInputSamples, 8, 1),
						new ArraySegment<Int32>(testInputSamples, 9, 1)
					},
					ExpecedRemainder = new ArraySegment<Int32>(testInputSamples, 10, 0)
				},
				new TestSample
				{
					Count = 11,
					ExpecedResult = new ArraySegment<Int32>[0],
					ExpecedRemainder = new ArraySegment<Int32>(testInputSamples, 0, 10)
				}
			};
		}

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TestSplitByLength()
		{
			foreach (var testSample in testSamples)
			{
				// perform operation
				ArraySegment<Int32> actualRemainder;

				var actualResult = testInputSamples.Divide(testSample.Count, out actualRemainder);

				// check results
				AssertAreEqual(testSample.ExpecedResult, actualResult);

				AssertAreEqual(testSample.ExpecedRemainder, actualRemainder);
			}
		}

		#endregion

		#region Private methods

		private static void AssertAreEqual<T>(IReadOnlyList<ArraySegment<T>> expected, IReadOnlyList<ArraySegment<T>> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count);

			for (var index = 0; index < expected.Count; index++)
			{
				AssertAreEqual(expected[index], actual[index]);
			}
		}

		private static void AssertAreEqual<T>(ArraySegment<T> expected, ArraySegment<T> actual)
		{
			Assert.AreEqual(expected.Array, expected.Array);

			Assert.AreEqual(expected.Offset, actual.Offset);

			Assert.AreEqual(expected.Count, actual.Count);
		}

		#endregion
	}
}