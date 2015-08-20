using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// Represents an Arne Anderson form of balanced tree used for storing and retrieving ordered data efficiently.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public sealed class ArneTree<TKey, TValue>
		where TValue : IComparable<TValue>, IComparable<TKey>
	{
		#region Constant and Static Fields

		private static readonly TryResult<TValue> tryGetFailResult = TryResult<TValue>.CreateFail();

		#endregion

		#region Fields

		/// <summary>
		/// Root of the tree.
		/// </summary>
		private ArneTreeNode<TKey, TValue> root;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ArneTree{TKey, TValue}" /> class.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ArneTree()
		{
			root = null;

			Count = 0;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of elements contained in the tree.
		/// </summary>
		public Int32 Count
		{
			get;

			private set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds the specified element into the tree.
		/// </summary>
		/// <param name="item">The element to add to the tree.</param>
		/// <returns><c>true</c> if the element is added to the <see cref="ArneTree{TKey, TValue}" /> object; <c>false</c> if the element is already present.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Boolean Add(TValue item)
		{
			var node = Add(root, item);

			if (node == null)
			{
				return false;
			}

			root = node;

			Count++;

			return true;
		}

		/// <summary>
		/// Removes all elements from the tree.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			Count = 0;

			root = null;
		}

		/// <summary>
		/// Determines whether the the contains the specific value.
		/// </summary>
		/// <param name="key">The key of the object.</param>
		/// <returns><c>true</c> if item is found in the tree, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Boolean Contains(TKey key)
		{
			return Contains(root, key);
		}

		/// <summary>
		/// Removes the specified element from the tree.
		/// </summary>
		/// <param name="item">The element to remove.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(TValue item)
		{
			if (root == null)
			{
				return;
			}

			root = Remove(root, item);

			Count--;
		}

		/// <summary>
		/// Tries to get a value associated with the specified <paramref name="key" />.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}" /> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success" /> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result" /> contains valid object if operation was successful, <c>default(<typeparamref name="TValue" />)</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TryResult<TValue> TryGetValue(TKey key)
		{
			return TryGetValue(root, key);
		}

		#endregion

		#region Private Static

		/// <summary>
		/// Adds the specified element to a tree.
		/// </summary>
		/// <param name="node">The root of the tree to insert an item.</param>
		/// <param name="leaf">The element to add to the tree.</param>
		/// <returns>A balanced version of the Arne Anderson tree, including the new item.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ArneTreeNode<TKey, TValue> Add(ArneTreeNode<TKey, TValue> node, TValue leaf)
		{
			if (node == null)
			{
				return new ArneTreeNode<TKey, TValue>(leaf, 1, null, null);
			}

			var compare = leaf.CompareTo(node.Value);

			if (compare < 0)
			{
				node.Left = Add(node.Left, leaf);
			}
			else if (compare > 0)
			{
				node.Right = Add(node.Right, leaf);
			}
			else
			{
				return null;
			}

			// Perform skew and then split.
			// The conditionals that determine whether or not a rotation will occur or not are inside of the procedures, as given above.
			node = node.Skew();

			node = node.Split();

			return node;
		}

		/// <summary>
		/// Determines whether specified tree contains the specified element.
		/// </summary>
		/// <param name="node">The root of the tree.</param>
		/// <param name="key">The key of the element to locate in the tree.</param>
		/// <returns><c>true</c> if the tree contains the specified element; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Boolean Contains(ArneTreeNode<TKey, TValue> node, TKey key)
		{
			while (true)
			{
				if (node == null)
				{
					return false;
				}

				var compare = node.Value.CompareTo(key);

				if (compare > 0)
				{
					node = node.Left;
				}
				else if (compare < 0)
				{
					node = node.Right;
				}
				else
				{
					return true;
				}
			}
		}

		/// <summary>
		/// Tries to get the value associated with the specified <paramref name="key" />.
		/// </summary>
		/// <param name="node">The root of the sub tree.</param>
		/// <param name="key">The key of the value to get.</param>
		/// <returns>
		/// An instance of <see cref="TryResult{T}" /> which encapsulates result of the operation.
		/// <see cref="TryResult{T}.Success" /> contains <c>true</c> if operation was successful, <c>false</c> otherwise.
		/// <see cref="TryResult{T}.Result" /> contains valid object if operation was successful, <c>default(<typeparamref name="TValue" />)</c> otherwise.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static TryResult<TValue> TryGetValue(ArneTreeNode<TKey, TValue> node, TKey key)
		{
			while (true)
			{
				if (node == null)
				{
					return tryGetFailResult;
				}

				var compare = node.Value.CompareTo(key);

				if (compare > 0)
				{
					node = node.Left;
				}
				else if (compare < 0)
				{
					node = node.Right;
				}
				else
				{
					return TryResult<TValue>.CreateSuccess(node.Value);
				}
			}
		}

		/// <summary>
		/// Removes the specified element from the tree.
		/// </summary>
		/// <param name="node">The root of the tree from which a data should be deleted.</param>
		/// <param name="item">The element to remove.</param>
		/// <returns>A root of the tree without deleted data.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ArneTreeNode<TKey, TValue> Remove(ArneTreeNode<TKey, TValue> node, TValue item)
		{
			if (node == null)
			{
				return null;
			}

			var compare = item.CompareTo(node.Value);

			if (compare < 0)
			{
				node.Left = Remove(node.Left, item);
			}
			else if (compare > 0)
			{
				node.Right = Remove(node.Right, item);
			}
			else
			{
				// If NodeBase is leaf, easy, otherwise reduce to leaf case.
				if (node.IsLeaf)
				{
					return null;
				}

				if (node.Left == null)
				{
					var right = node.Successor;

					node.Right = Remove(node.Right, right.Value);

					node = new ArneTreeNode<TKey, TValue>(right.Value, node.Level, node.Left, node.Right);
				}
				else
				{
					var left = node.Predecessor;

					node.Left = Remove(node.Left, left.Value);

					node = new ArneTreeNode<TKey, TValue>(left.Value, node.Level, node.Left, node.Right);
				}
			}

			// Re-balance the tree.
			// Decrease the level of all nodes in this level if necessary
			node = node.DecraseLevel();

			// Skew and split all nodes in the new level.
			node = node.Skew();

			if (node.Right != null)
			{
				node.Right = node.Right.Skew();
			}

			if (node.Right?.Right != null)
			{
				node.Right.Right = node.Right.Right.Skew();
			}

			node = node.Split();

			if (node.Right != null)
			{
				node.Right = node.Right.Split();
			}

			return node;
		}

		#endregion
	}
}