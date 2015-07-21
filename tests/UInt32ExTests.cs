using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="UInt32Ex"/> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class UInt32ExTests
	{
		#region GetHighestSetBitIndex Tests

		private static readonly Tuple<UInt32, Int32>[] getHighestSetBitIndexTestSamples =
		{
			Tuple.Create(0u, 0),
			Tuple.Create(1u, 0),
			Tuple.Create(128u, 7),
			Tuple.Create(0x80000000, 31)
		};

		/// <summary>
		/// Tests the <see cref="UInt32Ex.GetHighestSetBitIndex"/> method.
		/// </summary>
		[TestMethod]
		[TestCategory("UnitTests")]
		public void GetHighestSetBitIndexTest()
		{
			for (var index = 0; index < getHighestSetBitIndexTestSamples.Length; index++)
			{
				var inputValue = getHighestSetBitIndexTestSamples[index].Item1;

				var expectedResult = getHighestSetBitIndexTestSamples[index].Item2;

				var actualResult = inputValue.GetHighestSetBitIndex();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion

		#region IsPowerOfTwo Tests

		private static readonly Tuple<UInt32, Boolean>[] isPowerOfTwoTestSamples =
		{
			Tuple.Create(0u, false), Tuple.Create(1u, true), Tuple.Create(2u, true), Tuple.Create(3u, false), Tuple.Create(0x80000000, true)
		};

		/// <summary>
		/// Tests the <see cref="UInt32Ex.IsPowerOfTwo"/> method.
		/// </summary>
		[TestMethod]
		[TestCategory("UnitTests")]
		public void IsPowerOfTwoTest()
		{
			for (var index = 0; index < isPowerOfTwoTestSamples.Length; index++)
			{
				var inputValue = isPowerOfTwoTestSamples[index].Item1;

				var expectedResult = isPowerOfTwoTestSamples[index].Item2;

				var actualResult = inputValue.IsPowerOfTwo();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion

		#region GetSetBitsCount Test

		private static readonly Tuple<UInt32, UInt32>[] getSetBitsCountTestSamples =
		{
			Tuple.Create(0u, 0u),
			Tuple.Create(1u, 1u),
			Tuple.Create(2u, 1u),
			Tuple.Create(4545u, 5u),
			Tuple.Create(878545u, 12u),
			Tuple.Create(UInt32.MaxValue, 32u)
		};

		/// <summary>
		/// Tests the <see cref="UInt32Ex.GetSetBitsCount"/> method.
		/// </summary>
		[TestMethod]
		[TestCategory("UnitTests")]
		public void GetSetBitsCountTest()
		{
			for (var index = 0; index < getSetBitsCountTestSamples.Length; index++)
			{
				var inputValue = getSetBitsCountTestSamples[index].Item1;

				var expectedResult = getSetBitsCountTestSamples[index].Item2;

				var actualResult = inputValue.GetSetBitsCount();

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion

		#region Allign Test

		private static readonly Tuple<UInt32, UInt32>[] alignTo128TestSamples =
		{
			Tuple.Create(0u, 128u),
			Tuple.Create(1u, 128u),
			Tuple.Create(127u, 128u),
			Tuple.Create(511u, 512u),
			Tuple.Create(1024u, 1024u),
			Tuple.Create(0xFFFFFF7F, 0xFFFFFF80)
		};

		/// <summary>
		/// Tests the <see cref="UInt32Ex.Align"/> method.
		/// </summary>
		[TestMethod]
		[TestCategory("UnitTests")]
		public void AlignTo128Test()
		{
			for (var index = 0; index < alignTo128TestSamples.Length; index++)
			{
				var inputValue = alignTo128TestSamples[index].Item1;

				var expectedResult = alignTo128TestSamples[index].Item2;

				var actualResult = inputValue.Align(128);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion
	}
}