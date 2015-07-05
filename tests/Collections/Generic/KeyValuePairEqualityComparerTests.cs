using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a set of tests for <see cref="KeyValuePairEqualityComparer{TKey,TValue}" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class KeyValuePairEqualityComparerTests
	{
		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void EqualsTest()
		{
			var first = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				},
				{
					"d3", "abc14"
				}
			};

			var second = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				},
				{
					"d3", "abc14"
				}
			};

			var actualResult = first.SafeSequenceEqual(second, new KeyValuePairEqualityComparer<String, String>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase));

			Assert.IsTrue(actualResult);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void LengthNotEqualsTest()
		{
			var first = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				},
				{
					"d3", "abc14"
				}
			};

			var second = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				}
			};

			var actualResult = first.SafeSequenceEqual(second, new KeyValuePairEqualityComparer<String, String>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase));

			Assert.IsFalse(actualResult);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void KeyNotEqualsTest()
		{
			var first = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				},
				{
					"d3", "abc14"
				}
			};

			var second = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				},
				{
					"d4", "abc14"
				}
			};

			var actualResult = first.SafeSequenceEqual(second, new KeyValuePairEqualityComparer<String, String>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase));

			Assert.IsFalse(actualResult);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ValueNotEqualsTest()
		{
			var first = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				},
				{
					"d3", "abc14"
				}
			};

			var second = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc15"
				},
				{
					"d3", "abc14"
				}
			};

			var actualResult = first.SafeSequenceEqual(second, new KeyValuePairEqualityComparer<String, String>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase));

			Assert.IsFalse(actualResult);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void GetHashCodeTest()
		{
			var testSamples = new Dictionary<String, String>
			{
				{
					"a0", "abc00"
				},
				{
					"b1", "abc12"
				},
				{
					"c2", "abc13"
				},
				{
					"d3", "abc14"
				}
			};

			var comparer = new KeyValuePairEqualityComparer<String, String>(StringComparer.Ordinal, StringComparer.OrdinalIgnoreCase);

			foreach (var testSample in testSamples)
			{
				{
					Assert.AreEqual(testSample.GetHashCode(), comparer.GetHashCode(testSample));
				}
			}
		}

		#endregion
	}
}