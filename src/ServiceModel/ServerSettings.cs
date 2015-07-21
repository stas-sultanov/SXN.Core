using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.ServiceModel
{
	/// <summary>
	/// Specifies the configuration settings for a <see cref="ServerBase"/> class.
	/// </summary>
	[DataContract]
	public class ServerSettings
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="ServerSettings"/> class.
		/// </summary>
		/// <param name="name">The name of the server.</param>
		/// <param name="performanceCounters">The dictionary of configuration settings for the performance counters.</param>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="performanceCounters"/> is <c>null</c>.</exception>
		public ServerSettings(String name, IReadOnlyDictionary<String, PerformanceCounterSettings> performanceCounters)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (performanceCounters == null)
			{
				throw new ArgumentNullException(nameof(performanceCounters));
			}

			Name = name;

			PerformanceCounters = performanceCounters;

			if (!performanceCounters.ContainsKey(@"ActiveTasks"))
			{
				throw new ArgumentException(@"Counter can not be found: ActiveTasks.", nameof(performanceCounters));
			}

			if (!performanceCounters.ContainsKey(@"BadRequestsPerSecond"))
			{
				throw new ArgumentException(@"Counter can not be found: BadRequestsPerSecond.", nameof(performanceCounters));
			}

			if (!performanceCounters.ContainsKey(@"RequestsPerSecond"))
			{
				throw new ArgumentException(@"Counter can not be found: RequestsPerSecond.", nameof(performanceCounters));
			}

			if (!performanceCounters.ContainsKey(@"RequestProcessingAverageBase"))
			{
				throw new ArgumentException(@"Counter can not be found: RequestProcessingAverageBase.", nameof(performanceCounters));
			}

			if (!performanceCounters.ContainsKey(@"RequestProcessingAverageTime"))
			{
				throw new ArgumentException(@"Counter can not be found: RequestProcessingAverageTime.", nameof(performanceCounters));
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// The name of the server.
		/// </summary>
		[DataMember]
		public String Name
		{
			get;
		}

		/// <summary>
		/// The dictionary of configuration settings for the performance counters.
		/// </summary>
		[DataMember]
		public IReadOnlyDictionary<String, PerformanceCounterSettings> PerformanceCounters
		{
			get;
		}

		#endregion
	}
}