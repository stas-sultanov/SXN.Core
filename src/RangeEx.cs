using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="Range{T}" /> class.
	/// </summary>
	public static class RangeEx
	{
		#region Methods

		/// <summary>
		/// Sorts and combines sequences into the enumeration of <see cref="Range{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of the elements of the <paramref name="source" />.</typeparam>
		/// <param name="source">An <see cref="IEnumerable{T}" /> in which ranges should be looked.</param>
		/// <param name="isNext">A delegate to the method which checks whether two instance of <typeparamref name="T" /> are next to each other.</param>
		/// <param name="singleValues">Contains items within the <paramref name="source" /> enumeration which does not fall in to any range.</param>
		/// <returns>Collection of ranges within the <paramref name="source" />.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="isNext" /> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IList<Range<T>> CombineSequences<T>(this IEnumerable<T> source, Func<T, T, Boolean> isNext, out IList<T> singleValues)
			where T : IEquatable<T>, IComparable<T>
		{
			if (isNext == null)
			{
				throw new ArgumentNullException(nameof(isNext));
			}

			// Initialize output array
			singleValues = new List<T>();

			// Initialize result array
			var result = new List<Range<T>>();

			// Sort and distinct source list
			using (var enumerator = source.OrderBy(x => x).GetEnumerator())
			{
				// Check if there are items in source
				if (!enumerator.MoveNext())
				{
					// No items
					return result;
				}

				// Get first item
				var firstItem = enumerator.Current;

				while (true)
				{
					var currentItem = enumerator.Current;

					var isNotLast = enumerator.MoveNext();

					// Check if this is not last value
					if (isNotLast)
					{
						// Get next item
						var nextItem = enumerator.Current;

						// Check if next item equals to current or is next to current item
						if (currentItem.Equals(nextItem) || isNext(currentItem, nextItem))
						{
							continue;
						}
					}

					// Check if single value
					if (firstItem.Equals(currentItem))
					{
						singleValues.Add(currentItem);
					}
					else
					{
						// Initialize a new range
						var range = new Range<T>(firstItem, currentItem);

						// Add to result
						result.Add(range);
					}

					if (isNotLast)
					{
						firstItem = enumerator.Current;
					}
					else
					{
						return result;
					}
				}
			}
		}

		/// <summary>
		/// Returns the minimum and maximum value in the <paramref name="source" /> as the instance of <see cref="Range{T}" />.
		/// </summary>
		/// <typeparam name="T">The type of the elements of the <paramref name="source" />.</typeparam>
		/// <param name="source">A sequence of values to determine the range.</param>
		/// <returns>The minimum and maximum value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range<T> Range<T>(this IEnumerable<T> source)
			where T : IEquatable<T>, IComparable<T>
		{
			T minResult;

			T maxResult;

			// Get enumerator
			var enumerator = source.GetEnumerator();

			// Move to next
			if (enumerator.MoveNext())
			{
				minResult = maxResult = enumerator.Current;
			}
			else
			{
				return new Range<T>(default(T), default(T));
			}

			while (enumerator.MoveNext())
			{
				var currentItem = enumerator.Current;

				if (minResult.CompareTo(currentItem) > 0)
				{
					minResult = enumerator.Current;
				}

				if (maxResult.CompareTo(currentItem) < 0)
				{
					maxResult = enumerator.Current;
				}
			}

			return new Range<T>(minResult, maxResult);
		}

		/// <summary>
		/// Invokes a transform function on each element of a generic sequence and returns the minimum and maximum value in the <paramref name="source" /> as the instance of <see cref="Range{T}" />.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
		/// <param name="source">A sequence of values to determine the range.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <returns>The minimum and maximum value.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="selector" /> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Range<TResult> Range<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
			where TResult : IEquatable<TResult>, IComparable<TResult>
		{
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}

			TResult minResult;

			TResult maxResult;

			// Get enumerator
			var enumerator = source.GetEnumerator();

			// Move to next
			if (enumerator.MoveNext())
			{
				minResult = maxResult = selector(enumerator.Current);
			}
			else
			{
				return new Range<TResult>(default(TResult), default(TResult));
			}

			while (enumerator.MoveNext())
			{
				var currentItem = selector(enumerator.Current);

				if (minResult.CompareTo(currentItem) > 0)
				{
					minResult = currentItem;
				}

				if (maxResult.CompareTo(currentItem) < 0)
				{
					maxResult = currentItem;
				}
			}

			return new Range<TResult>(minResult, maxResult);
		}

		#endregion
	}
}