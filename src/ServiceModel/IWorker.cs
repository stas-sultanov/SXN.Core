using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	/// <summary>
	/// Represents a contract for classes that implements the worker pattern.
	/// </summary>
	public interface IWorker
	{
		#region Properties

		/// <summary>
		/// The boolean value which indicates whether current instance is active.
		/// </summary>
		Boolean IsActive
		{
			get;
		}

		/// <summary>
		/// The identifier of the worker.
		/// </summary>
		String Id
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initiates an asynchronous operation to activate worker.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> object of type <see cref="Boolean"/>> that represents the asynchronous operation.
		/// <see cref="Task{TResult}.Result"/> equals <c>true</c> if operation has completed successfully, <c>false</c> otherwise.
		/// </returns>
		Task<Boolean> ActivateAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Initiates an asynchronous operation to deactivate worker.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		Task DeactivateAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Initiates an asynchronous operation to perform work.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for a task to complete.</param>
		/// <returns>A <see cref="Task"/> object that represents the asynchronous operation.</returns>
		Task RunAsync(CancellationToken cancellationToken);

		#endregion
	}
}