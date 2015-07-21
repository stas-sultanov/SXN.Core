using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Linq
{
	/// <summary>
	/// Provides a set of tests for <see cref="EnumerableEx"/> class.
	/// </summary>
	[TestClass]
	// ReSharper disable once InconsistentNaming
	[ExcludeFromCodeCoverage]
	public class EnumerableExTests
	{
		#region Constant and Static Fields

		private const Int32 maxBlockCount = 100;

		private static readonly IReadOnlyList<String> blockIds;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of <see cref="Enumerable"/> class.
		/// </summary>
		static EnumerableExTests()
		{
			// Initialize a list of block ids
			var blockIdsTemp = new String[maxBlockCount];

			for (UInt16 index = 0; index < maxBlockCount; index++)
			{
				// Get a byte representation of the ushort
				var indexAsByteArray = BitConverter.GetBytes(index);

				// Convert to base-64 string
				blockIdsTemp[index] = Convert.ToBase64String(indexAsByteArray);
			}

			blockIds = Array.AsReadOnly(blockIdsTemp);
		}

		#endregion

		#region Test methods

		#region ApplyRange

		[TestMethod]
		[TestCategory("LoadTests")]
		public void ApplyRange_LoadTest()
		{
			LoadTest.ExecuteParallelAsync("one", x =>
			{
				x++;

				var y = blockIds.ApplyRange(x);

				Trace.TraceInformation("res {0:D2}: {1}", x, y.Aggregate((a, b) => a + b));
			}, 20, 20).Wait();
		}

		#endregion

		#endregion

		#region IsSingle and IsMultiple

		private static readonly Int32[][] singleInputData =
		{
			new Int32[]
			{
			},
			new[]
			{
				1
			},
			new[]
			{
				1, 2
			},
			new[]
			{
				1, 2, 3, 4, 5
			}
		};

		private static readonly Boolean[] singleExpectedResults =
		{
			false, true, false, false
		};

		private static readonly Boolean[] multipleExpectedResults =
		{
			false, false, true, true
		};

		[TestMethod]
		[TestCategory("UnitTests")]
		public void IsSingle_UnitTest()
		{
			for (var index = 0; index < singleInputData.Length; index++)
			{
				var inputData = singleInputData[index];

				var expectedResult = singleExpectedResults[index];

				var actualResult = inputData.IsSingle();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void IsMultiple_UnitTest()
		{
			for (var index = 0; index < singleInputData.Length; index++)
			{
				var inputData = singleInputData[index];

				var expectedResult = multipleExpectedResults[index];

				var actualResult = inputData.IsMultiple();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion

		#region HasDuplicates

		private static readonly Int32[][] duplicatesInputData =
		{
			new Int32[]
			{
			},
			new[]
			{
				1
			},
			new[]
			{
				1, 2, 1
			},
			new[]
			{
				1, 2, 3, 4, 5
			},
			new[]
			{
				1, 2, 1, 3, 4, 5, 2, 6
			}
		};

		private static readonly Boolean[] duplicatesExpectedResults =
		{
			false, false, true, false, true
		};

		[TestMethod]
		[TestCategory("UnitTests")]
		public void HasDuplicates_UnitTest_Arguments()
		{
			try
			{
				((Int32[]) null).HasDuplicates(x => x);

				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}

			try
			{
				(new[]
				{
					1, 2, 3
				}).HasDuplicates((Func<Int32, Int32>) null);

				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void HasDuplicates_UnitTest()
		{
			for (var index = 0; index < duplicatesInputData.Length; index++)
			{
				var inputData = duplicatesInputData[index];

				var expectedResult = duplicatesExpectedResults[index];

				var actualResult = inputData.HasDuplicates();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion

		#region EnumerableEquals

		private static readonly Int32[] enArray0 =
		{
			1, 2, 3, 4, 5
		};

		private static readonly Int32[] enArray1 =
		{
			1, 2, 3, 4, 5, 6, 7
		};

		private static readonly Int32[] enArray2 =
		{
			1, 2, 3, 4, 5
		};

		private static readonly Tuple<Int32[], Int32[], Boolean>[] enumerableEqualsData =
		{
			Tuple.Create((Int32[]) null, (Int32[]) null, true),
			Tuple.Create(enArray0, (Int32[]) null, false),
			Tuple.Create((Int32[]) null, enArray1, false),
			Tuple.Create(enArray0, enArray0, true),
			Tuple.Create(enArray0, enArray1, false),
			Tuple.Create(enArray0, enArray2, true)
		};

		[TestMethod]
		[TestCategory("UnitTests")]
		public void EnumerableEquals_UnitTest()
		{
			foreach (var inputData in enumerableEqualsData)
			{
				var enumA = inputData.Item1;

				var enumB = inputData.Item2;

				var actualResult = enumA.SafeSequenceEqual(enumB);

				Assert.AreEqual(inputData.Item3, actualResult);
			}
		}

		#endregion

		#region CombineSequences

		private readonly Tuple<Int32[], Range<Int32>[], Int32[]>[] makeRangesTestSamlpes =
		{
			// Empty
			Tuple.Create(new Int32[]
			{
			}, new Range<Int32>[]
			{
			}, new Int32[]
			{
			}),

			// One item
			Tuple.Create(new[]
			{
				1
			}, new Range<Int32>[]
			{
			}, new[]
			{
				1
			}),

			// Two items in range
			Tuple.Create(new[]
			{
				1, 2
			}, new[]
			{
				new Range<Int32>(1, 2)
			}, new Int32[]
			{
			}),

			// Two items not in range
			Tuple.Create(new[]
			{
				1, 3
			}, new Range<Int32>[]
			{
			}, new[]
			{
				1, 3
			}),
			Tuple.Create(new[]
			{
				0, 2, 3
			}, new[]
			{
				new Range<Int32>(2, 3)
			}, new[]
			{
				0
			}),
			Tuple.Create(new[]
			{
				0, 1, 3
			}, new[]
			{
				new Range<Int32>(0, 1)
			}, new[]
			{
				3
			}),
			Tuple.Create(new[]
			{
				0, 2, 3, 4, 6
			}, new[]
			{
				new Range<Int32>(2, 4)
			}, new[]
			{
				0, 6
			}),
			Tuple.Create(new[]
			{
				0, 2, 3, 4, 6, 7, 8
			}, new[]
			{
				new Range<Int32>(2, 4), new Range<Int32>(6, 8)
			}, new[]
			{
				0
			}),
			Tuple.Create(new[]
			{
				0, 2, 3, 4, 6, 8, 9, 11
			}, new[]
			{
				new Range<Int32>(2, 4), new Range<Int32>(8, 9)
			}, new[]
			{
				0, 6, 11
			}),
			Tuple.Create(new[]
			{
				0, 1, 2, 4, 6, 8, 10, 11
			}, new[]
			{
				new Range<Int32>(0, 2), new Range<Int32>(10, 11)
			}, new[]
			{
				4, 6, 8
			}),

			// Unsorted with duplicates
			Tuple.Create(new[]
			{
				2, 2, 1, 1, 1, 1, 2
			}, new[]
			{
				new Range<Int32>(1, 2)
			}, new Int32[]
			{
			}),
			Tuple.Create(new[]
			{
				9, 5, 2, 2, 1, 7, 1, 1, 1, 2
			}, new[]
			{
				new Range<Int32>(1, 2)
			}, new[]
			{
				5, 7, 9
			})
		};

		[TestMethod]
		[TestCategory("UnitTests")]
		public void CombineSequences_UnitTest()
		{
			Func<Int32, Int32, Boolean> isNext = (x, y) => y - x == 1;

			foreach (var testSamlpe in makeRangesTestSamlpes)
			{
				IList<Int32> singleValues;

				var ranges = testSamlpe.Item1.CombineSequences(isNext, out singleValues).ToArray();

				CollectionAssert.AreEqual(testSamlpe.Item2, ranges);

				CollectionAssert.AreEqual(testSamlpe.Item3, singleValues.ToArray());
			}
		}

		#endregion

		#region Range

		private readonly Tuple<Int32[], Range<Int32>>[] rangeTestSamlpes =
		{
			Tuple.Create(new Int32[]
			{
			}, new Range<Int32>(0, 0)),
			Tuple.Create(new[]
			{
				1
			}, new Range<Int32>(1, 1)),
			Tuple.Create(new[]
			{
				1, 3, 5
			}, new Range<Int32>(1, 5)),
			Tuple.Create(new[]
			{
				5, 1, 3
			}, new Range<Int32>(1, 5)),
			Tuple.Create(new[]
			{
				-9, -3, -1
			}, new Range<Int32>(-9, -1)),
			Tuple.Create(new[]
			{
				-9, 0, 1, 3
			}, new Range<Int32>(-9, 3))
		};

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Range_UnitTest()
		{
			foreach (var testSamlpe in rangeTestSamlpes)
			{
				var expectedResult = testSamlpe.Item2;

				var actualResultA = testSamlpe.Item1.Range();

				Assert.AreEqual(expectedResult, actualResultA);

				var actualResultB = testSamlpe.Item1.Range(x => x);

				Assert.AreEqual(expectedResult, actualResultB);
			}
		}

		#endregion
	}
}