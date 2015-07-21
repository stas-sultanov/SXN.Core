using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides a base class for types that are disposable.
	/// </summary>
	public abstract class DisposableBase : IDisposable
	{
		#region Constructors

		/// <summary>
		/// Initialize a new instance of <see cref="DisposableBase"/> class.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected DisposableBase()
		{
			IsDisposed = false;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a <see cref="Boolean"/> value which indicates whether instance is disposed.
		/// </summary>
		public Boolean IsDisposed
		{
			get;

			private set;
		}

		#endregion

		#region Methods of IDisposable

		/// <summary>
		/// Releases resources associated with the instance.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			// Release the resources
			Dispose(true);

			// Request system not to call the finalize method for a specified object
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Releases resources associated with the instance.
		/// </summary>
		/// <param name="fromDispose">Value indicating whether method was called from the <see cref="Dispose()"/> method.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Dispose(Boolean fromDispose)
		{
			// Check if object is disposed already
			if (IsDisposed)
			{
				return;
			}

			// Check if call is from dispose
			if (fromDispose)
			{
				ReleaseResources();
			}

			// Set disposed
			IsDisposed = true;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Releases managed resources held by the instance.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected abstract void ReleaseResources();

		#endregion
	}
}