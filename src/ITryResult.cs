namespace System
{
	/// <summary>
	/// Provides an interface for types which encapsulates the results of the methods,
	/// that implements Try-Do pattern.
	/// </summary>
	/// <typeparam name="T">The type of the data returned by a Try-Do method.</typeparam>
	public interface ITryResult<out T>
	{
		#region Properties

		/// <summary>
		/// Gets an instance of <typeparamref name="T"/>
		/// if the method has completed successfully, default value otherwise.
		/// </summary>
		T Result
		{
			get;
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> value,
		/// that indicates whether the method has completed successfully.
		/// </summary>
		Boolean Success
		{
			get;
		}

		#endregion
	}
}