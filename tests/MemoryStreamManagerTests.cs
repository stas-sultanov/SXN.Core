using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of unit tests for the <see cref="MemoryStreamManager" /> structure.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class MemoryStreamManagerTests
	{
		#region Constant and Static Fields

		private const Int32 iterationCount = 1048576;

		private const Int32 memoryBlockSize = 8190;

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("LoadTests")]
		public void GetVsNew()
		{
			var manager = new MemoryStreamManager(memoryBlockSize);

			var results = LoadTest.ExecuteCompare
				(
					"Get",
					index =>
					{
						var stream = manager.Get();

						manager.Put(stream);
					},
					"New",
					index =>
					{
						var stream = new MemoryStream(memoryBlockSize);

						stream.Dispose();
					},
					iterationCount
				);

			Trace.Write(results.ToString());
		}

		[TestMethod]
		[TestCategory("LoadTests")]
		public void ParallelNewVsGet()
		{
			var manager = new MemoryStreamManager(memoryBlockSize);

			var newResults = LoadTest.ExecuteParallelAsync
				(
					"new",
					index =>
					{
						var stream = new MemoryStream(memoryBlockSize);

						stream.Dispose();
					},
					iterationCount,
					4096
				).Result;

			Trace.Write(newResults.ToString());

			var cacheResults = LoadTest.ExecuteParallelAsync
				(
					"get",
					index =>
					{
						var stream = manager.Get();

						manager.Put(stream);
					},
					iterationCount,
					4096
				).Result;

			Trace.Write(cacheResults.ToString());
		}

		#endregion
	}
}