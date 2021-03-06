﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	/// <summary>
	/// Provides a set of tests for <see cref="StringTemplate{T}" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class StringTemplateTests
	{
		#region Nested Types

		private enum UrlArgs
		{
			Host,

			CampaignId,

			RequestId
		}

		#endregion

		#region Constant and Static Fields

		private const Int32 iterationCount = 327680;

		private const String testFileName = @"TestsData\StringTemplateTest.html";

		#endregion

		#region Fields

		private readonly String template;

		private readonly IReadOnlyCollection<Tuple<String, String[]>> testSamples = new[]
		{
			new Tuple<String, String[]>("http://[host]/[data]/[campaignId]/[requestId]?[redirectUrlParam]=[redirectUrl]", new[]
			{
				"[host]", "[campaignId]", "[requestId]", "[redirectUrlParam]", "[redirectUrl]"
			}),
			new Tuple<String, String[]>("arg0 supppa test", new[]
			{
				"arg0"
			}),
			new Tuple<String, String[]>("supppa arg0 test", new[]
			{
				"arg0"
			}),
			new Tuple<String, String[]>("supppa test arg0", new[]
			{
				"arg0"
			}),
			new Tuple<String, String[]>("arg0 supppa arg0 test", new[]
			{
				"arg0"
			}),
			new Tuple<String, String[]>("supppa arg0 test arg0", new[]
			{
				"arg0"
			}),
			new Tuple<String, String[]>("arg0 supppa arg0 test arg0", new[]
			{
				"arg0"
			}),
			new Tuple<String, String[]>("arg0 supppa arg1 test", new[]
			{
				"arg0", "arg1"
			}),
			new Tuple<String, String[]>("supppa arg0 test arg1", new[]
			{
				"arg0", "arg1"
			}),
			new Tuple<String, String[]>("arg0 supppa arg1 test arg2", new[]
			{
				"arg0", "arg1", "arg2"
			})
		};

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of <see cref="StringTemplateTests" /> class.
		/// </summary>
		public StringTemplateTests()
		{
			template = File.ReadAllText(testFileName);
		}

		#endregion

		#region Test methods

		[TestCategory("UnitTests")]
		[TestMethod]
		public void CtorPatternTest()
		{
			const String template = "https://play.google.com/store/apps/details?id=com.wooga.jelly_splash&referrer=tid%2D{121}&33Dutm_sid={121}";

			const String expectedResult = "https://play.google.com/store/apps/details?id=com.wooga.jelly_splash&referrer=tid%2D1020&33Dutm_sid=1020";

			var stringTemplate = StringTemplate.Initialize(Encoding.UTF8, template, "{\\w+}");

			Assert.IsTrue(stringTemplate.Variables.Contains("{121}"));

			var actualResult = stringTemplate.Instantiate("1020");

			Assert.AreEqual(expectedResult, actualResult);
		}

		[TestCategory("UnitTests")]
		[TestMethod]
		public void IntegrityTest()
		{
			foreach (var testSample in testSamples)
			{
				var stringTemplate = StringTemplate.Initialize(Encoding.Unicode, testSample.Item1, testSample.Item2);

				var actualResult = stringTemplate.Instantiate(testSample.Item2);

				Assert.AreEqual(testSample.Item1, actualResult, false, CultureInfo.InvariantCulture);
			}
		}

		[TestCategory("LoadTests")]
		[TestMethod]
		public void StringFormatVsStringTemplateTest()
		{
			var stringTemplate = StringTemplate.Initialize(Encoding.UTF8, template, "arg0", "arg1", "arg2", "arg3", "arg4", "arg5");

			var stringFormat = template.Replace("{", "{{").Replace("}", "}}").Replace("arg0", "{0}").Replace("arg1", "{1}").Replace("arg2", "{2}").Replace("arg3", "{3}").Replace("arg4", "{4}").Replace("arg5", "{5}");

			var memoryStream = new MemoryStream(8192);

			// Perform test
			var testResults = LoadTest.ExecuteCompare
				(
					"String.Format",
					index =>
					{
						var str = String.Format(stringFormat, "arg0", "arg1", "arg2", "arg3", "arg4", "arg5");

						var encodedStr = Encoding.UTF8.GetBytes(str);

						memoryStream.Write(encodedStr, 0, encodedStr.Length);

						memoryStream.SetLength(0);
					},
					"StringTemplate.Instantiate",
					index =>
					{
						stringTemplate.Instantiate(memoryStream, "arg0", "arg1", "arg2", "arg3", "arg4", "arg5");

						memoryStream.SetLength(0);
					},
					iterationCount
				);

			Trace.TraceInformation(testResults.ToString());
		}

		[TestCategory("LoadTests")]
		[TestMethod]
		public void StringFormatVsStringTemplateTest2()
		{
			var stringTemplate = StringTemplate.Initialize(Encoding.UTF8, "http://$host$/data/$campaignId$/$requestId$", "$host$", "$campaignId$", "$requestId$");

			var campaignIdAsString = Value128Converter.FromGuid(Guid.NewGuid()).ToString();

			var requestIdAsString = Value128Converter.FromGuid(Guid.NewGuid()).ToString();

			var memoryStream = new MemoryStream(1024);

			// Perform test
			var testResults = LoadTest.ExecuteCompare
				(
					"String.Format",
					index =>
					{
						var postbackUrl = String.Format(CultureInfo.InvariantCulture, "http://{0}/{1}/{2}/{3}", "localhost", "data", campaignIdAsString, requestIdAsString);

						var encodedStr = Encoding.UTF8.GetBytes(postbackUrl);

						memoryStream.Write(encodedStr, 0, encodedStr.Length);

						memoryStream.SetLength(0);
					},
					"StringTemplate.Instantiate",
					index =>
					{
						stringTemplate.Instantiate(memoryStream, "localhost", campaignIdAsString, requestIdAsString);

						memoryStream.SetLength(0);
					},
					iterationCount
				);

			Trace.TraceInformation(testResults.ToString());
		}

		[TestCategory("UnitTests")]
		[TestMethod]
		public void InstantiateViaDictionary()
		{
			var tx = "arg0 supppa arg1 test arg2";

			var template = StringTemplate.Initialize(Encoding.UTF7, tx, "arg2", "arg4");

			var rx = template.Instantiate(new Dictionary<String, String>
			{
				{
					"arg2", "shit"
				}
			}.TryGetValue);
		}

		[TestCategory("UnitTests")]
		[TestMethod]
		public void NoParamsDictionary()
		{
			const String ExpectedResult = "https://google.com/";

			var emptyTemplate = StringTemplate.Initialize(Encoding.UTF7, ExpectedResult, "{\\w+}");

			var actualResult = emptyTemplate.Instantiate();

			Assert.AreEqual(ExpectedResult, actualResult);
		}

		[TestCategory("UnitTests")]
		[TestMethod]
		public void EnumTypeAndLambda()
		{
			var variables = new Dictionary<String, UrlArgs>
			{
				{
					"{host}", UrlArgs.Host
				},
				{
					"{campaign_id}", UrlArgs.CampaignId
				},
				{
					"{request_id}", UrlArgs.RequestId
				}
			};

			var variablesValues = new Dictionary<UrlArgs, String>
			{
				{
					UrlArgs.Host, "54.10.17.18"
				},
				{
					UrlArgs.CampaignId, "camp1"
				},
				{
					UrlArgs.RequestId, "102"
				}
			};

			// Test with no agrs
			{
				const String ExpectedResult = @"http://54.10.17.18/data/camp1/102";

				var stringTemplate = StringTemplate.Initialize(Encoding.UTF8, ExpectedResult, variables);

				var actualResult = stringTemplate.Instantiate(variablesValues.TryGetValue);

				Assert.AreEqual(ExpectedResult, actualResult);
			}

			// Test with args
			{
				const String ExpectedResult = @"http://54.10.17.18/data/camp1/102";

				var stringTemplate = StringTemplate.Initialize(Encoding.UTF8, "http://{host}/data/{campaign_id}/{request_id}", variables);

				var actualResult = stringTemplate.Instantiate(variablesValues.TryGetValue);

				Assert.AreEqual(ExpectedResult, actualResult);
			}
		}

		#endregion
	}
}