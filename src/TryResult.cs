using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Encapsulates the results of the execution of the Try-Do methods.
	/// </summary>
	/// <typeparam name="T">The type of the result returned by a Try-Do method.</typeparam>
	public struct TryResult<T> : IEquatable<TryResult<T>>, ITryResult<T>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="TryResult{T}" /> structure.
		/// </summary>
		/// <param name="success">A <see cref="Boolean" /> value that indicates whether an operation was successful.</param>
		/// <param name="result">A valid <typeparamref name="T" /> object if operation was successful.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TryResult(Boolean success, T result)
		{
			Success = success;

			Result = result;
		}

		#endregion

		#region Overrides of ValueType

		/// <summary>
		/// Returns a value that indicates whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><c>true</c> if <paramref name="obj" /> is a <see cref="Value128" /> that has the same value as this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Equals(Object obj)
		{
			if (obj is TryResult<T>)
			{
				return Equals((TryResult<T>) obj);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override Int32 GetHashCode()
		{
			return Success.GetHashCode();
		}

		#endregion

		#region Methods of IEquatable<TryResult<T>>

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="TryResult{T}" /> represent the same value.
		/// </summary>
		/// <param name="other">An object to compare to this instance.</param>
		/// <returns><c>true</c> if <paramref name="other" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Boolean Equals(TryResult<T> other)
		{
			return Success.Equals(other.Success) && Result.Equals(other.Result);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Crates a failed result of the operation.
		/// </summary>
		/// <param name="result">The default value of the <typeparamref name="T" />.</param>
		/// <returns>The failed result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<T> CreateFail(T result = default(T))
		{
			return new TryResult<T>(false, result);
		}

		/// <summary>
		/// Crates a successful result of the operation.
		/// </summary>
		/// <param name="result">The data returned by the operation.</param>
		/// <returns>The successful result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TryResult<T> CreateSuccess(T result)
		{
			return new TryResult<T>(true, result);
		}

		/// <summary>
		/// Indicates whether the values of two specified objects are equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator ==(TryResult<T> first, TryResult<T> second)
		{
			// Return true if the fields match:
			return first.Equals(second);
		}

		/// <summary>
		/// Converts instance of <see cref="TryResult{T}" /> to the instance of <typeparamref name="T" />.
		/// </summary>
		/// <param name="value">The instance of <see cref="TryResult{T}" /> to convert.</param>
		/// <returns>The instance of <typeparamref name="T" />.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator T(TryResult<T> value)
		{
			return value.Result;
		}

		/// <summary>
		/// Indicates whether the values of two specified objects are not equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are not equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator !=(TryResult<T> first, TryResult<T> second)
		{
			return !first.Equals(second);
		}

		#endregion

		#region Implementation of ITryResult<TResult>

		/// <summary>
		/// Gets an instance of <typeparamref name="T" /> if operation was successful, <c>default</c>(<typeparamref name="T" />) otherwise.
		/// </summary>
		public T Result
		{
			get;
		}

		/// <summary>
		/// Gets a <see cref="Boolean" /> value that indicates whether an operation was successful.
		/// </summary>
		public Boolean Success
		{
			get;
		}

		#endregion
	}
}