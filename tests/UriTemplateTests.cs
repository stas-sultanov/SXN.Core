using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System
{
	[TestClass]
	[ExcludeFromCodeCoverage]
	public class UriTemplateTests
	{
		#region Constant and Static Fields

		private static readonly Uri baseAddress = new Uri("http://localhost");

		private static readonly UriTemplate template = new UriTemplate("{sourceId}/{operationId}?r={redirectUrl}");

		private static readonly Uri[] uris =
		{
			new Uri("http://service.domain.net/0ZpVzpXKOkSg_cqeEzjdNw==/101?r=http%3A%2F%2Fmsdn.microsoft.com%2Fquery%2Fdev12.query%3FappId%3DDev12IDEF1%26l%3DEN-US%26k%3Dk(System.Net.WebRequest)%3Bk(TargetFrameworkMoniker-.NETFramework%2CVersion%253Dv4.5.1)%3Bk(DevLang-csharp)%26rd%3Dtrue"), new Uri("http://service.net/0ZpVzpXKOkSg_cqeEzjdNw==/sutehut?r=https%3A%2F%2Fwww.google.com.ua%2Fwebhp%3Fsourceid%3Dchrome-instant%26ion%3D1%26espv%3D2%26es_th%3D1%26ie%3DUTF-8%23newwindow%3D1%26safe%3Doff%26q%3Dsend%2520life%2520sms%2520ua"), new Uri("http://service.net/0ZpVzpXKOkSg_cqeEzjdNw==/09309_33333?r=https%3A%2F%2Fwww.myget.org%2Ffeed%2Ftest-common%2Fpackage%2FAdRout.Common.Azure"), new Uri("http://service.test.test.net/0ZpVzpXKOkSg_cqeEzjdNw==/989084iiii989?r=https://www.google.com.ua/webhp?sourceid=chrome-instant&ion=1&espv=2&es_th=1&ie=UTF-8#newwindow=1&safe=off&q=encode+url"), new Uri("http://service.test.test.net/0ZpVzpXKOkSg_cqeEzjdNw==/989084iiii989")
		};

		#endregion

		#region Test methods

		[TestCategory("LoadTests")]
		[TestMethod]
		public void MatchLoadTest()
		{
			var testResult = LoadTest.Execute("Dictionary", index =>
			{
				var subIndex = index % uris.Length;

				var uri = uris[subIndex];

				template.Match(baseAddress, uri);
			}, 1000000);

			Trace.TraceInformation(testResult.ToString());

			//Assert.AreEqual(actualResult1, actualResult2);
		}

		#endregion
	}
}