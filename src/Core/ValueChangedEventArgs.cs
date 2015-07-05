namespace System
{
	/// <summary>
	/// Provides data for the value changed event.
	/// </summary>
	public sealed class ValueChangedEventArgs<T> : EventArgs
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="ValueChangedEventArgs{T}" /> class.
		/// </summary>
		public ValueChangedEventArgs(T previousValue, T currentValue)
		{
			PreviousValue = previousValue;

			CurrentValue = currentValue;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets current value of the object.
		/// </summary>
		public T CurrentValue
		{
			get;
		}

		/// <summary>
		/// Gets previous value of the object.
		/// </summary>
		public T PreviousValue
		{
			get;
		}

		#endregion
	}
}