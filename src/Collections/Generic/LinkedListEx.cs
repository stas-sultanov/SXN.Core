namespace System.Collections.Generic
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="LinkedList{T}"/> class.
	/// </summary>
	public static class LinkedListEx
	{
		#region Methods

		/// <summary>
		/// Removes nodes from the <paramref name="list"/>, which values satisfy the <paramref name="predicate"/>.
		/// </summary>
		/// <typeparam name="T">The type of the elements of the linked <paramref name="list"/>.</typeparam>
		/// <param name="list">The linked list.</param>
		/// <param name="predicate">The function that defines whether node should be removed.</param>
		public static void Remove<T>(this LinkedList<T> list, Func<T, Boolean> predicate)
		{
			// Get first node
			var currentNode = list.First;

			while (currentNode != null)
			{
				// Check if value satisfy the predicate
				if (predicate(currentNode.Value))
				{
					var tempNode = currentNode;

					// Move to next node
					currentNode = currentNode.Next;

					// Remove node
					list.Remove(tempNode);
				}
				else
				{
					// Move to next node
					currentNode = currentNode.Next;
				}
			}
		}

		#endregion
	}
}