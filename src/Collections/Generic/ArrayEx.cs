using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="Array" /> class.
	/// </summary>
	public static class ArrayEx
	{
		#region Methods

		/// <summary>
		/// Divides the <paramref name="dividen" /> on the specified number of the segments.
		/// </summary>
		/// <typeparam name="T">The type of the elements of the <paramref name="dividen" />.</typeparam>
		/// <param name="dividen">The dividend..</param>
		/// <param name="divisor">The divisor.</param>
		/// <param name="remainder">The remainder.</param>
		/// <returns>The array of the segments.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArraySegment<T>[] Divide<T>(this T[] dividen, Int32 divisor, out ArraySegment<T> remainder)
		{
			// check array
			if (dividen == null)
			{
				throw new ArgumentNullException(nameof(dividen));
			}

			// check segments
			if (divisor < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(divisor));
			}

			Int32 divRemainder;

			// calc the quotient and remainder
			var quotient = Math.DivRem(dividen.Length, divisor, out divRemainder);

			// initialize reminder
			remainder = new ArraySegment<T>(dividen, quotient * divisor, divRemainder);

			// calc result length
			var resultLength = divisor > dividen.Length ? 0 : divisor;

			// initialize result array
			var result = new ArraySegment<T>[resultLength];

			// initialize result array items
			for (var index = 0; index < resultLength; index++)
			{
				// initialize item
				result[index] = new ArraySegment<T>(dividen, quotient * index, quotient);
			}

			// return result
			return result;
		}

		#endregion
	}
}