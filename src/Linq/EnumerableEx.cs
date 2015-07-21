using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="IEnumerable{T}"/> interface.
	/// </summary>
	public static class EnumerableEx
	{
		#region Methods

		/// <summary>
		/// Applies range on the iteration through the sequence of elements.
		/// </summary>
		/// <typeparam name="T">The type of the elements of the <paramref name="source"/>.</typeparam>
		/// <param name="source">The <see cref="IEnumerable{T}"/> on which to apply the range.</param>
		/// <param name="count">The count of elements to iterate.</param>
		/// <param name="startIndex">The index of the first element to start an iteration.</param>
		/// <returns>The range enumerator.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> ApplyRange<T>(this IEnumerable<T> source, Int64 count, Int64 startIndex = 0)
		{
			var endIndex = startIndex + count;

			var index = 0;

			foreach (var item in source)
			{
				index++;

				if (index > endIndex)
				{
					yield break;
				}

				if (index > startIndex)
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> value which indicates whether <paramref name="source"/> has duplicate elements.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="selector"/>.</typeparam>
		/// <param name="source">The <see cref="IEnumerable{T}"/> whose elements to check.</param>
		/// <param name="selector">The function to extract the key for each element.</param>
		/// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to compare keys with.</param>
		/// <returns><c>true</c> if <paramref name="source"/> has duplicates, <c>false</c> otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="selector"/> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean HasDuplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, IEqualityComparer<TKey> comparer = null)
		{
			if (selector == null)
			{
				throw new ArgumentNullException(nameof(selector));
			}

			if (comparer == null)
			{
				comparer = EqualityComparer<TKey>.Default;
			}

			return source.GroupBy(selector, comparer).Any(i => i.IsMultiple());
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> value which indicates whether <paramref name="source"/> has duplicate elements.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">The <see cref="IEnumerable{T}"/> whose elements to check.</param>
		/// <returns><c>true</c> if <paramref name="source"/> has duplicates, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean HasDuplicates<T>(this IEnumerable<T> source)
		{
			return HasDuplicates(source, x => x);
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> value which indicates whether <paramref name="source"/> has more than one element.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">The <see cref="IEnumerable{T}"/> on which to apply the range.</param>
		/// <returns><c>true</c> if <paramref name="source"/> has more than one element, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean IsMultiple<T>(this IEnumerable<T> source)
		{
			var enumerator = source.GetEnumerator();

			return enumerator.MoveNext() && enumerator.MoveNext();
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> value which indicates whether <paramref name="source"/> has only one element.
		/// </summary>
		/// <typeparam name="T">The type of the elements of the <paramref name="source"/>.</typeparam>
		/// <param name="source">The <see cref="IEnumerable{T}"/> that should be checked.</param>
		/// <returns><c>true</c> if <paramref name="source"/> has only one element, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean IsSingle<T>(this IEnumerable<T> source)
		{
			var enumerator = source.GetEnumerator();

			return enumerator.MoveNext() && !enumerator.MoveNext();
		}

		/// <summary>
		/// Determines whether two sequences are equal by comparing the elements by using the specified <paramref name="comparer"/> for their type.
		/// </summary>
		/// <typeparam name="T">The type of the elements of <paramref name="first"/> and <paramref name="second"/>.</typeparam>
		/// <param name="first">The <see cref="IEnumerable{T}"/> to compare to the <paramref name="second"/> sequence.</param>
		/// <param name="second">The <see cref="IEnumerable{T}"/> to compare to the <paramref name="first"/> sequence.</param>
		/// <param name="comparer">The <see cref="IEqualityComparer{TKey}"/> to use to compare elements of the <paramref name="first"/> and the <paramref name="second"/>.</param>
		/// <returns><c>true</c> if <paramref name="first"/> and <paramref name="second"/> are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean SafeSequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals(first, second))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (first == null || second == null)
			{
				return false;
			}

			return first.SequenceEqual(second, comparer);
		}

		#endregion
	}
}