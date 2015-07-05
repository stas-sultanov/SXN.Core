using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Threading
{
	/// <summary>
	/// Provides a set of tests for <see cref="AlignedTimer" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class AlignedTimerTests
	{
		#region Constant and Static Fields

		private static readonly AlignedTimer timer = new AlignedTimer(Action, TimeUnit.Second, TimeSpan.Zero);

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("ManualTests")]
		public void ExecutionOnStopTest()
		{
			timer.Enabled = true;

			Thread.Sleep(TimeSpan.FromSeconds(4.8));

			timer.Enabled = false;

			Thread.Sleep(TimeSpan.FromSeconds(3));
		}

		#endregion

		#region Private methods

		private static void Action(DateTime fireTime)
		{
			Trace.TraceInformation("Fire time: {0:HH:mm:ss:FFFFFFF}, Actual Time: {1:HH:mm:ss:FFFFFFF}", fireTime, DateTime.UtcNow);
		}

		#endregion
	}
}