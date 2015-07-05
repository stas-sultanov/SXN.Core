using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a set of tests for <see cref="GenericEqualityComparer{T}" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class GenericEqualityComparerTests
	{
		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void EqualsTest()
		{
			var first = new List<UriFormat>
			{
				UriFormat.SafeUnescaped,
				UriFormat.Unescaped,
				UriFormat.UriEscaped
			};

			var second = new List<UriFormat>
			{
				UriFormat.SafeUnescaped,
				UriFormat.Unescaped,
				UriFormat.UriEscaped
			};

			var comparer = new GenericEqualityComparer<UriFormat>((x, y) => x == y, x => x.GetHashCode());

			var actualResult = first.SafeSequenceEqual(second, comparer);

			Assert.IsTrue(actualResult);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void NotEqualsTest()
		{
			var first = new List<UriFormat>
			{
				UriFormat.SafeUnescaped,
				UriFormat.Unescaped,
				UriFormat.UriEscaped
			};

			var second = new List<UriFormat>
			{
				UriFormat.Unescaped,
				UriFormat.SafeUnescaped,
				UriFormat.UriEscaped
			};

			var comparer = new GenericEqualityComparer<UriFormat>((x, y) => x == y, x => x.GetHashCode());

			var actualResult = first.SafeSequenceEqual(second, comparer);

			Assert.IsFalse(actualResult);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void GetHashCodeTest()
		{
			var testSamples = new List<UriFormat>
			{
				UriFormat.SafeUnescaped,
				UriFormat.Unescaped,
				UriFormat.UriEscaped
			};

			var comparer = new GenericEqualityComparer<UriFormat>((x, y) => x == y, x => x.GetHashCode());

			for (var index = 0; index < testSamples.Count; index++)
			{
				Assert.AreEqual(testSamples[index].GetHashCode(), comparer.GetHashCode(testSamples[index]));
			}
		}

		#endregion
	}
}