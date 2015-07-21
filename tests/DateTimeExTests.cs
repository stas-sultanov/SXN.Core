using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="DateTimeEx"/> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class DateTimeExTests
	{
		#region Test methods

		[TestMethod]
		[TestCategory("UnitTests")]
		public void AddTimeUnit()
		{
			var time = DateTime.UtcNow;

			{
				var expectedResult = time.AddSeconds(1);

				var actualResult = time.Add(TimeUnit.Second);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddMinutes(1);

				var actualResult = time.Add(TimeUnit.Minute);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddHours(1);

				var actualResult = time.Add(TimeUnit.Hour);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddDays(1);

				var actualResult = time.Add(TimeUnit.Day);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void AddTimeInterval()
		{
			var time = DateTime.UtcNow;

			{
				var expectedResult = time.AddSeconds(1);

				var actualResult = time.Add(new TimeInterval(TimeUnit.Second));

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddMinutes(2);

				var actualResult = time.Add(new TimeInterval(TimeUnit.Minute, 2));

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddHours(3);

				var actualResult = time.Add(new TimeInterval(TimeUnit.Hour, 3));

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddDays(4);

				var actualResult = time.Add(new TimeInterval(TimeUnit.Day, 4));

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void SubtractTimeInterval()
		{
			var time = DateTime.UtcNow;

			{
				var expectedResult = time.AddSeconds(-1);

				var actualResult = time.Subtract(new TimeInterval(TimeUnit.Second));

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddMinutes(-2);

				var actualResult = time.Subtract(new TimeInterval(TimeUnit.Minute, 2));

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddHours(-3);

				var actualResult = time.Subtract(new TimeInterval(TimeUnit.Hour, 3));

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddDays(-4);

				var actualResult = time.Subtract(new TimeInterval(TimeUnit.Day, 4));

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void SubtractTimeUnit()
		{
			var time = DateTime.UtcNow;

			{
				var expectedResult = time.AddSeconds(-1);

				var actualResult = time.Subtract(TimeUnit.Second);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddMinutes(-1);

				var actualResult = time.Subtract(TimeUnit.Minute);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddHours(-1);

				var actualResult = time.Subtract(TimeUnit.Hour);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = time.AddDays(-1);

				var actualResult = time.Subtract(TimeUnit.Day);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Ceilling()
		{
			var time = DateTime.UtcNow;

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second).AddSeconds(1);

				var actualResult = time.Ceiling(TimeUnit.Second);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0).AddMinutes(1);

				var actualResult = time.Ceiling(TimeUnit.Minute);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0).AddHours(1);

				var actualResult = time.Ceiling(TimeUnit.Hour);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0).AddDays(1);

				var actualResult = time.Ceiling(TimeUnit.Day);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void Floor()
		{
			var time = DateTime.UtcNow;

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);

				var actualResult = time.Floor(TimeUnit.Second);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);

				var actualResult = time.Floor(TimeUnit.Minute);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);

				var actualResult = time.Floor(TimeUnit.Hour);

				Assert.AreEqual(expectedResult, actualResult);
			}

			{
				var expectedResult = new DateTime(time.Year, time.Month, time.Day, 0, 0, 0);

				var actualResult = time.Floor(TimeUnit.Day);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		public void IsLastInterval()
		{
			var testTrueTime = new DateTime(2019, 12, DateTime.DaysInMonth(1999, 12), 23, 59, 59, 999) + new TimeSpan(9);

			var testFalseTime = new DateTime(2015, 11, DateTime.DaysInMonth(1999, 11) - 1, 22, 58, 58, 998) + new TimeSpan(8);

			var units = new[]
			{
				TimeUnit.Microsecond, TimeUnit.Millisecond, TimeUnit.Second, TimeUnit.Minute, TimeUnit.Hour, TimeUnit.Day
			};

			foreach (var unit in units)
			{
				Assert.IsTrue(testTrueTime.IsLastTimeInterval(unit));

				Assert.IsFalse(testFalseTime.IsLastTimeInterval(unit));
			}
		}

		#endregion
	}
}