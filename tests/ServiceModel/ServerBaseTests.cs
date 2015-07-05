using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace System.ServiceModel
{
	/// <summary>
	/// Provides a set of test methods for <see cref="ServerBase" /> class.
	/// </summary>
	[TestClass]
	[ExcludeFromCodeCoverage]
	public sealed class ServerBaseTests
	{
		#region Nested Types

		private sealed class TestServer : ServerBase
		{
			#region Constructors

			public TestServer(EventHandler<DiagnosticsEventArgs> diagnosticsEventHandler, ServerSettings settings)
				: base(diagnosticsEventHandler, settings)
			{
				Assert.AreEqual(EntityState.Creating, State);

				State = EntityState.Inactive;
			}

			#endregion

			#region Overrides of ServerBase

			/// <summary>
			/// Releases managed resources held by class.
			/// </summary>
			protected override void ReleaseManagedResources()
			{
				Assert.AreEqual(EntityState.Retiring, State);

				base.ReleaseManagedResources();
			}

			protected override Task<TryResult<IServerRequestHandler>> TryAwaitRequestAsync()
			{
				return Task.FromResult(TryResult<IServerRequestHandler>.CreateSuccess(new ServerRequestHandler()));
			}

			#endregion

			#region Overrides of WorkerBase

			/// <summary>
			/// When implemented in a derived class, executes when a <see cref="WorkerBase.ActivateAsync" /> method is called.
			/// </summary>
			protected override Task<Boolean> OnActivatingAsync(CancellationToken cancellationToken)
			{
				Assert.AreEqual(EntityState.Activating, State);

				return Task.FromResult(true);
			}

			/// <summary>
			/// When implemented in a derived class, executes when a <see cref="WorkerBase.DeactivateAsync" /> method is called.
			/// </summary>
			protected override Task OnDeactivatingAsync(CancellationToken cancellationToken)
			{
				Assert.AreEqual(EntityState.Deactivating, State);

				return Task.FromResult(true);
			}

			#endregion
		}

		private sealed class ServerRequestHandler : IServerRequestHandler
		{
			#region Methods of IDisposable

			#region Implementation of IDisposable

			public void Dispose()
			{
			}

			#endregion

			#endregion

			#region Implementation of IServerRequestHandler

			public async Task<Boolean> TryProcessAsync()
			{
				await Task.Yield();

				return true;
			}

			public DateTime AcceptTime => DateTime.UtcNow;

			#endregion
		}

		#endregion

		#region Test methods

		/**/

		[TestMethod]
		[TestCategory("UnitTests")]
		public async Task ExceptionHandlingTestAsync()
		{
			var cancellationToken = CancellationToken.None;

			// 0.0 Get configuration as string
			var settingsAsString = File.ReadAllText(@"TestsData\TestServer.json");

			// 0.1 Get service pointer manager configuration
			var settings = JsonConvert.DeserializeObject<ServerSettings>(settingsAsString);

			var service = new TestServer(DiagnosticsEventHandler, settings);

			await service.ActivateAsync(cancellationToken);

#pragma warning disable 4014

			Task.Run(async () =>

			{
				await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

				await service.DeactivateAsync(cancellationToken);
			}, cancellationToken);

#pragma warning restore 4014

			await service.RunAsync();

			await service.DeactivateAsync(cancellationToken);
		}

		#endregion

		#region Private methods

		private static void DiagnosticsEventHandler(Object o, DiagnosticsEventArgs e)
		{
			Trace.TraceInformation("{0}:{1}:{2}", e.Source, e.Level, e.Message);
		}

		[TestMethod]
		[TestCategory("UnitTests")]
		private static async Task TestActivityAsync()
		{
			var cancellationToken = CancellationToken.None;

			// 0.0 Get configuration as string
			var settingsAsString = File.ReadAllText(@"TestServer.json");

			// 0.1 Get service pointer manager configuration
			var settings = JsonConvert.DeserializeObject<ServerSettings>(settingsAsString);

			var server = new TestServer(DiagnosticsEventHandler, settings);

			using (server)
			{
				Assert.AreEqual(EntityState.Inactive, server.State);

				// Test single start call
				await server.ActivateAsync(cancellationToken);

				Assert.AreEqual(EntityState.Active, server.State);

				// Test double start call
				await server.ActivateAsync(cancellationToken);

				Assert.IsTrue(server.IsActive);

				// Test single stop call
				await server.DeactivateAsync(cancellationToken);

				Assert.IsFalse(server.IsActive);

				// Test double stop call
				await server.DeactivateAsync(cancellationToken);

				Assert.IsFalse(server.IsActive);
			}

			Assert.AreEqual(EntityState.Retired, server.State);

			Assert.IsTrue(server.IsDisposed);

			server.Dispose();

			Assert.AreEqual(EntityState.Retired, server.State);
		}

		#endregion
	}
}