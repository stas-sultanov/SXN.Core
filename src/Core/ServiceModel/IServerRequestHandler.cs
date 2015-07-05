using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System.ServiceModel
{
	/// <summary>
	/// Represents the contract for classes which handles the requests to the server.
	/// </summary>
	public interface IServerRequestHandler : IDisposable
	{
		#region Properties

		/// <summary>
		/// The UTC time when request was accepted by the server.
		/// </summary>
		DateTime AcceptTime
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Initiates an asynchronous operation to try processes the request.
		/// </summary>
		/// <returns>
		/// A <see cref="Task{TResult}" /> object of type <see cref="Boolean" />> that represents the asynchronous operation.
		/// <see cref="Task{TResult}.Result" /> equals <c>true</c> if operation has completed successfully, <c>false</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		Task<Boolean> TryProcessAsync();

		#endregion
	}
}