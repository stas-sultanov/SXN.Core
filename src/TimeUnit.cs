namespace System
{
	/// <summary>
	/// Defines the units of time measurement.
	/// </summary>
	public enum TimeUnit : long
	{
		/// <summary>
		/// None.
		/// </summary>
		None = 0L,

		/// <summary>
		/// The millisecond.
		/// </summary>
		Millisecond = 10000L,

		/// <summary>
		/// The second.
		/// </summary>
		Second = 10000000L,

		/// <summary>
		/// The minute.
		/// </summary>
		Minute = 600000000L,

		/// <summary>
		/// The hour.
		/// </summary>
		Hour = 36000000000L,

		/// <summary>
		/// The day.
		/// </summary>
		Day = 864000000000L
	}
}