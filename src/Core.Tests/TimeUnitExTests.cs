using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="TimeUnitEx" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class TimeUnitExTests
	{
		#region Constant and Static Fields

		private static readonly Tuple<TimeUnit, TryResult<TimeUnit>>[] tryGetNextSamples =
		{
			Tuple.Create(TimeUnit.Microsecond, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Millisecond)),
			Tuple.Create(TimeUnit.Millisecond, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Second)),
			Tuple.Create(TimeUnit.Second, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Minute)),
			Tuple.Create(TimeUnit.Minute, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Hour)),
			Tuple.Create(TimeUnit.Hour, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Day)),
			Tuple.Create(TimeUnit.Day, TryResult<TimeUnit>.CreateFail())
		};

		private static readonly Tuple<TimeUnit, TryResult<TimeUnit>>[] tryGetPreviousSamples =
		{
			Tuple.Create(TimeUnit.Day, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Hour)),
			Tuple.Create(TimeUnit.Hour, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Minute)),
			Tuple.Create(TimeUnit.Minute, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Second)),
			Tuple.Create(TimeUnit.Second, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Millisecond)),
			Tuple.Create(TimeUnit.Millisecond, TryResult<TimeUnit>.CreateSuccess(TimeUnit.Microsecond)),
			Tuple.Create(TimeUnit.Microsecond, TryResult<TimeUnit>.CreateFail())
		};

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TryGetNext()
		{
			foreach (var testSample in tryGetNextSamples)
			{
				var expectedResult = testSample.Item2;

				var actualResult = testSample.Item1.TryGetNext();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void TryGetPrevious()
		{
			foreach (var testSample in tryGetPreviousSamples)
			{
				var expectedResult = testSample.Item2;

				var actualResult = testSample.Item1.TryGetPrevious();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion
	}
}