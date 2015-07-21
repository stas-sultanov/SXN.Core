namespace System.Collections.Generic
{
	/// <summary>
	/// Provides equality comparison of two <typeparamref name="T"/> objects.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	public sealed class GenericEqualityComparer<T> : IEqualityComparer<T>
	{
		#region Fields

		private readonly Func<T, T, Boolean> equals;

		private readonly Func<T, Int32> getHashCode;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="GenericEqualityComparer{T}"/> class.
		/// </summary>
		/// <param name="equals">A delegate to the method that determines whether the specified objects are equal.</param>
		/// <param name="getHashCode">A delegate to the method that determines whether the specified objects are equal.</param>
		/// <exception cref="ArgumentNullException"><paramref name="equals"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="getHashCode"/> is <c>null</c>.</exception>
		public GenericEqualityComparer(Func<T, T, Boolean> equals, Func<T, Int32> getHashCode)
		{
			if (equals == null)
			{
				throw new ArgumentNullException(nameof(equals));
			}

			if (getHashCode == null)
			{
				throw new ArgumentNullException(nameof(getHashCode));
			}

			this.equals = equals;

			this.getHashCode = getHashCode;
		}

		#endregion

		#region Methods of IEqualityComparer<T>

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="x">The first object of type to compare.</param>
		/// <param name="y">The second object of type to compare.</param>
		/// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
		public Boolean Equals(T x, T y)
		{
			return equals(x, y);
		}

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		public Int32 GetHashCode(T obj)
		{
			return getHashCode(obj);
		}

		#endregion
	}
}