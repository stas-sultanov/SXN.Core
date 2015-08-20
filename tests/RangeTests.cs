using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	using IntRange = Range<Int32>;

	/// <summary>
	/// Provides a set of unit tests for <see cref="Range{T}" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class RangeTests
	{
		#region Constant and Static Fields

		private const Int32 iterationsCount = 6;

		#endregion

		#region Fields

		private readonly Boolean[] greaterExpectedResults =
		{
			false, true, true, true, false, false
		};

		private readonly IntRange[] inputArrayA =
		{
			new IntRange(0, 10), new IntRange(20, 30), new IntRange(-10, -5), new IntRange(10, 20), new IntRange(0, 0), new IntRange(0, 10)
		};

		private readonly IntRange[] inputArrayB =
		{
			new IntRange(20, 30), new IntRange(0, 10), new IntRange(-20, -15), new IntRange(-10, -5), new IntRange(0, 0), new IntRange(-5, 5)
		};

		private readonly Boolean[] lessExpectedResults =
		{
			true, false, false, false, false, false
		};

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Greater()
		{
			for (var index = 0; index < iterationsCount; index++)
			{
				var operandA = inputArrayA[index];

				var operandB = inputArrayB[index];

				var expectedResult = greaterExpectedResults[index];

				var actualResult = operandA > operandB;

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Less()
		{
			for (var index = 0; index < iterationsCount; index++)
			{
				var operandA = inputArrayA[index];

				var operandB = inputArrayB[index];

				var expectedResult = lessExpectedResults[index];

				var actualResult = operandA < operandB;

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Equals()
		{
			for (var index = 0; index < iterationsCount; index++)
			{
				var operandA = inputArrayA[index];

				var operandB = new IntRange(operandA.Begin, operandA.End);

				var operandC = new IntRange(operandA.Begin + 10, operandA.End - 10);

				Assert.AreEqual(operandA, operandB);

				Assert.AreEqual(operandA, (Object) operandB);

				Assert.IsTrue(operandA == operandB);

				Assert.IsFalse(operandA != operandB);

				Assert.AreNotEqual(operandA, operandC);

				Assert.AreNotEqual(operandA, (Object) operandC);

				Assert.IsTrue(operandA != operandC);

				Assert.IsFalse(operandA == operandC);
			}
		}

		#endregion
	}
}