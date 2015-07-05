using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="Value128" /> structure.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class Value128Tests
	{
		#region Constant and Static Fields

		private const Int32 addIterations = 8;

		private const Int32 subIterations = 7;

		private const Int32 testSamplesCount = 409600;

		private static readonly Random random = new Random(DateTime.UtcNow.Millisecond);

		private static readonly Value128[] addExpectedResults = new Value128[addIterations]
		{
			new Value128(0, 0), new Value128(0, 1), new Value128(0, 1), new Value128(0, 2), new Value128(1, 0), new Value128(1, 0), new Value128(1, 0xFFFFFFFFFFFFFFFE), new Value128(0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFE)
		};

		private static readonly Value128[] addInputArrayA = new Value128[addIterations]
		{
			new Value128(0, 0), new Value128(0, 1), new Value128(0, 0), new Value128(0, 1), new Value128(0, 0xFFFFFFFFFFFFFFFF), new Value128(0, 1), new Value128(0, 0xFFFFFFFFFFFFFFFF), new Value128(0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF)
		};

		private static readonly Value128[] addInputArrayB = new Value128[addIterations]
		{
			new Value128(0, 0), new Value128(0, 0), new Value128(0, 1), new Value128(0, 1), new Value128(0, 1), new Value128(0, 0xFFFFFFFFFFFFFFFF), new Value128(0, 0xFFFFFFFFFFFFFFFF), new Value128(0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF)
		};

		private static readonly Value128[] subExpectedResults = new Value128[subIterations]
		{
			new Value128(0, 0), new Value128(0, 1), new Value128(0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF), new Value128(0, 0), new Value128(0, 0xFFFFFFFFFFFFFFFF), new Value128(0, 1), new Value128(1, 0)
		};

		private static readonly Value128[] subInputArrayA = new Value128[subIterations]
		{
			new Value128(0, 0), new Value128(0, 1), new Value128(0, 0), new Value128(0, 1), new Value128(1, 0), new Value128(1, 0), new Value128(0, 0xFFFFFFFFFFFFFFFF)
		};

		private static readonly Value128[] subInputArrayB = new Value128[subIterations]
		{
			new Value128(0, 0), new Value128(0, 0), new Value128(0, 1), new Value128(0, 1), new Value128(0, 1), new Value128(0, 0xFFFFFFFFFFFFFFFF), new Value128(0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF)
		};

		#endregion

		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Addition()
		{
			for (var index = 0; index < addIterations; index++)
			{
				var operandA = addInputArrayA[index];

				var operandB = addInputArrayB[index];

				var expectedResult = addExpectedResults[index];

				var actualResult = Value128.Add(operandA, operandB);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertGuid()
		{
			var address = new Byte[16];

			for (var index = 0; index < testSamplesCount; index++)
			{
				// Generate Data
				random.NextBytes(address);

				// Get expected result
				var expectedResult = new Guid(address);

				// Convert to Value128
				var intermediateResult = (Value128) expectedResult;

				// Convert to Guid
				var actualResult = (Guid) intermediateResult;

				// Compare
				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void ConvertNewtonSoftJson()
		{
			for (var index = 0; index < testSamplesCount; index++)
			{
				// Get value128
				var expectedResult = (Value128) Guid.NewGuid();

				// Convert to JSON string
				var initialValue = JsonConvert.SerializeObject(expectedResult);

				// Convert back to value 128
				var actualResult = JsonConvert.DeserializeObject<Value128>(initialValue);

				// Compare
				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Equals()
		{
			var operandA = (Value128) Guid.NewGuid();

			var operandB = (Value128) Guid.NewGuid();

			var operandC = operandA;

			Assert.IsFalse(operandA.Equals(new Object()));

			Assert.IsFalse(operandA.Equals((Object) operandB));

			Assert.IsFalse(operandA == operandB);

			Assert.IsFalse(operandA != operandC);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void NullableConvertNewtonSoftJson()
		{
			for (var index = 0; index < testSamplesCount; index++)
			{
				// Get value128
				Value128? expectedResult = null;

				if (index % 2 == 1)
				{
					expectedResult = (Value128?) Guid.NewGuid();
				}

				// Convert to JSON string
				var initialValue = JsonConvert.SerializeObject(expectedResult);

				// Convert back to value 128
				var actualResult = JsonConvert.DeserializeObject<Value128?>(initialValue);

				// Compare
				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Subtraction()
		{
			for (var index = 0; index < subIterations; index++)
			{
				var operandA = subInputArrayA[index];

				var operandB = subInputArrayB[index];

				var expectedResult = subExpectedResults[index];

				var actualResult = Value128.Subtract(operandA, operandB);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		#endregion
	}
}