using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Collections.Generic
{
	[TestClass]
	public class ArneTreeLoadTest
	{
		#region Nested Types

		private sealed class Ip2GeoLocation : IComparable<Ip2GeoLocation>, IComparable<UInt32>
		{
			#region Fields

			public Range<UInt32> addressRange;

			public String Country;

			#endregion

			#region Methods of IComparable<Ip2GeoLocation>

			#region Implementation of IComparable<in Ip2GeoLocation>

			/// <summary>
			/// Compares the current object with another object of the same type.
			/// </summary>
			/// <returns>
			/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
			/// </returns>
			/// <param name="other">An object to compare with this object.</param>
			public Int32 CompareTo(Ip2GeoLocation other)
			{
				return addressRange.CompareTo(other.addressRange);
			}

			#endregion

			#endregion

			#region Methods of IComparable<uint>

			#region Implementation of IComparable<in uint>

			/// <summary>
			/// Compares the current object with another object of the same type.
			/// </summary>
			/// <returns>
			/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
			/// </returns>
			/// <param name="other">An object to compare with this object.</param>
			public Int32 CompareTo(UInt32 other)
			{
				return addressRange.CompareTo(other);
			}

			#endregion

			#endregion
		}

		#endregion

		#region Constant and Static Fields

		private static readonly ArneTree<UInt32, Ip2GeoLocation> geolocationTree = new ArneTree<UInt32, Ip2GeoLocation>();

		private static readonly IList<Range<UInt32>> invalidAddressRanges = new List<Range<UInt32>>();

		private static readonly Random random = new Random(DateTime.Now.Millisecond);

		#endregion

		#region Constructor

		public ArneTreeLoadTest()
		{
			//using (var reader = new StreamReader(@"TestsData\IP-COUNTRY-REGION-CITY.CSV"))
			using (var reader = new StreamReader(@"TestsData\IP-COUNTRY.CSV"))
			{
				var rangesCounter = 0;

				String line;

				var startTime = DateTime.Now;

				while ((line = reader.ReadLine()) != null)
				{
					rangesCounter++;

					var row = line.Split(',');

					var parsedInt = 0u;

					var addressRangeBegin = UInt32.TryParse(row[0].Replace('"', ' '), out parsedInt) ? parsedInt : 0u;

					var addressRangeEnd = UInt32.TryParse(row[1].Replace('"', ' '), out parsedInt) ? parsedInt : 0u;

					var addressRange = new Range<UInt32>(addressRangeBegin, addressRangeEnd);

					// Skip
					if (row[2] == @"""-""")
					{
						invalidAddressRanges.Add(addressRange);
					}
					else
					{
						var ip2Loc = new Ip2GeoLocation
						{
							addressRange = addressRange,
							Country = row[3]
						};

						geolocationTree.Add(ip2Loc);
					}
				}

				var stopTime = DateTime.Now;

				Trace.TraceInformation("Start reading data file at {0}", startTime);

				Trace.TraceInformation("Finished reading data file at {0}", stopTime);

				Trace.TraceInformation("Time taken to read is {0} seconds", (stopTime - startTime).TotalSeconds);

				Trace.TraceInformation("Processed ranges count {0}. Valid: {1}. Invalid {2}", rangesCounter, geolocationTree.Count, invalidAddressRanges.Count);
			}
		}

		#endregion

		#region Test methods

		[TestMethod]
		public void ContainsTest()
		{
			const Int32 count = 1000000;

			// generate ip
			var ipArray = new UInt32[count];

			for (var index = 0; index < count; index++)
			{
				ipArray[index] = (UInt32) random.Next();
			}

			var resultArray = new Boolean[count];

			var testResult = LoadTest.Execute
				(
					"ContainsTest",
					index =>
					{
						var ipAddress = ipArray[index];

						resultArray[index] = geolocationTree.Contains(ipAddress);
					},
					count
				);

			Trace.WriteLine(testResult.ToString());

			Trace.TraceInformation("Result is {0} hits and {1} misses", resultArray.Count(x => x), resultArray.Count(x => x == false));
		}

		[TestMethod]
		public void TryGetValue()
		{
			const Int32 Count = 1000000;

			// generate ip
			var ipArray = new UInt32[Count];

			for (var index = 0; index < Count; index++)
			{
				ipArray[index] = (UInt32) random.Next(Int32.MinValue, Int32.MaxValue);
			}

			var hitCount = 0;

			var missCount = 0;

			var testResult = LoadTest.Execute
				(
					"ContainsTest",
					index =>
					{
						var ipAddress = ipArray[index];

						if (geolocationTree.TryGetValue(ipAddress).Success)
						{
							hitCount++;
						}
						else
						{
							missCount++;
						}
					},
					Count
				);

			Trace.WriteLine(testResult.ToString());

			Trace.TraceInformation("Result is {0} hits and {1} misses", hitCount, missCount);
		}

		[TestMethod]
		public void TryGetInvalidAddress()
		{
			foreach (var invalidAddress in invalidAddressRanges.Select(invalidAddressRange => invalidAddressRange.Begin + (UInt32) random.Next((Int32) (invalidAddressRange.End - invalidAddressRange.Begin))))
			{
				Assert.IsFalse(geolocationTree.TryGetValue(invalidAddress).Success);
			}
		}

		#endregion
	}
}