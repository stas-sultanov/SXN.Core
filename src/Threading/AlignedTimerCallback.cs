namespace System.Threading
{
	/// <summary>
	/// Represents the method that handles calls from the instance of <see cref="AlignedTimer" />.
	/// </summary>
	/// <param name="invocationTime">The aligned time when callback has been invoked.</param>
	public delegate void AlignedTimerCallback(DateTime invocationTime);
}