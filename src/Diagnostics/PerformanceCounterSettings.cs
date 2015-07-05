namespace System.Diagnostics
{
	/// <summary>
	/// Specifies the configuration settings for the <see cref="PerformanceCounter" /> class.
	/// </summary>
	public sealed class PerformanceCounterSettings
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PerformanceCounterSettings" /> class.
		/// </summary>
		/// <param name="category">The name of the performance counter category for this performance counter.</param>
		/// <param name="counterType">The type of the performance counter.</param>
		/// <param name="name">The name of the performance counter.</param>
		public PerformanceCounterSettings(String category, PerformanceCounterType counterType, String name)
		{
			if (category == null)
			{
				throw new ArgumentNullException(nameof(category));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			Category = category;

			CounterType = counterType;

			Name = name;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The name of the performance counter category for this performance counter.
		/// </summary>
		public String Category
		{
			get;
		}

		/// <summary>
		/// The type of the performance counter.
		/// </summary>
		public PerformanceCounterType CounterType
		{
			get;
		}

		/// <summary>
		/// The name of the performance counter.
		/// </summary>
		public String Name
		{
			get;
		}

		#endregion
	}
}