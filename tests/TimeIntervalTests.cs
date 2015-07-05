using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="TimeInterval" /> structure.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class TimeIntervalTests
	{
		#region Test methods

		[TestMethod]
		[TestCategory("ManualTests")]
		public void ToStringTest()
		{
			var s = new TimeInterval(TimeUnit.Hour, 5).ToString();
		}

		#endregion
	}
}