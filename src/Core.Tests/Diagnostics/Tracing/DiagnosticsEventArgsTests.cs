using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace System.Diagnostics.Tracing
{
	/// <summary>
	/// Provides a set of tests for <see cref="DiagnosticsEventArgsTests" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class DiagnosticsEventArgsTests
	{
		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Serialize()
		{
			var testSample = new DiagnosticsEventArgs(EventLevel.Critical, new ArgumentNullException().ToString(), "Serialize");

			var x = JsonConvert.SerializeObject(testSample);

			var y = JsonConvert.DeserializeObject<DiagnosticsEventArgs>(x);

			Assert.AreEqual(testSample.Level, y.Level);

			Assert.AreEqual(testSample.Message, y.Message);

			Assert.AreEqual(testSample.Source, y.Source);
		}

		#endregion
	}
}