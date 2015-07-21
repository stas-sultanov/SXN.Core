namespace System.Diagnostics.Tracing
{
	/// <summary>
	/// Encapsulates data of the diagnostics event.
	/// </summary>
	public sealed class DiagnosticsEventArgs : EventArgs
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="DiagnosticsEventArgs"/> class.
		/// </summary>
		/// <param name="level">The level of the event.</param>
		/// <param name="message">The message of the event.</param>
		/// <param name="source">The source of the event.</param>
		public DiagnosticsEventArgs(EventLevel level, String message, String source)
		{
			Level = level;

			Message = message;

			Source = source;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The level of the event.
		/// </summary>
		public EventLevel Level
		{
			get;
		}

		/// <summary>
		/// The message of the event.
		/// </summary>
		public String Message
		{
			get;
		}

		/// <summary>
		/// The source of the event.
		/// </summary>
		public String Source
		{
			get;
		}

		#endregion
	}
}