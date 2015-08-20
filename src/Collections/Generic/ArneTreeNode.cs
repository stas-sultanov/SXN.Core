using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// Represents the node of the Arne Anderson tree.
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	internal sealed class ArneTreeNode<TKey, TValue> : NodeBase<TValue>
		where TValue : IComparable<TValue>, IComparable<TKey>
	{
		#region Fields

		/// <summary>
		/// The left leaf.
		/// </summary>
		private ArneTreeNode<TKey, TValue> left;

		/// <summary>
		/// The right leaf.
		/// </summary>
		private ArneTreeNode<TKey, TValue> right;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ArneTreeNode{TValue, TKey}" /> class.
		/// </summary>
		/// <param name="value">The data of the node.</param>
		/// <param name="level">The level in the tree.</param>
		/// <param name="left">The left leaf.</param>
		/// <param name="right">The right leaf.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ArneTreeNode(TValue value, UInt32 level, ArneTreeNode<TKey, TValue> left, ArneTreeNode<TKey, TValue> right)
			: base(value)
		{
			Level = level;

			Left = left;

			Right = right;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ArneTreeNode{TValue, TKey}" /> class.
		/// </summary>
		/// <param name="value">The data of the node.</param>
		/// <param name="level">The level in the tree.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ArneTreeNode(TValue value, UInt32 level)
			: base(value)
		{
			Level = level;

			left = null;

			right = null;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a <see cref="Boolean" /> value that indicates whether node is leaf.
		/// </summary>
		internal Boolean IsLeaf
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (left == null) && (right == null);
			}
		}

		/// <summary>
		/// Gets a <see cref="Boolean" /> value that indicates whether node is root.
		/// </summary>
		internal Boolean IsRoot
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return Parent == null;
			}
		}

		/// <summary>
		/// Gets or sets the left leaf.
		/// </summary>
		internal ArneTreeNode<TKey, TValue> Left
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return left;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (left != null)
				{
					left.Parent = null;
				}

				left = value;

				if (left != null)
				{
					left.Parent = this;
				}
			}
		}

		/// <summary>
		/// Gets or sets the level of the node.
		/// </summary>
		internal UInt32 Level
		{
			get;

			private set;
		}

		/// <summary>
		/// Gets a node with the maximal value.
		/// </summary>
		private ArneTreeNode<TKey, TValue> Max
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var current = this;

				while (current.Right != null)
				{
					current = current.Right;
				}

				return current;
			}
		}

		/// <summary>
		/// Gets a node with the minimal value.
		/// </summary>
		private ArneTreeNode<TKey, TValue> Min
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var current = this;

				while (current.Left != null)
				{
					current = current.Left;
				}

				return current;
			}
		}

		/// <summary>
		/// Gets or sets the parent of the node.
		/// </summary>
		private ArneTreeNode<TKey, TValue> Parent
		{
			get;

			set;
		}

		/// <summary>
		/// Gets a node with maximum value in the left sub tree.
		/// </summary>
		internal ArneTreeNode<TKey, TValue> Predecessor
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				// Max of the right sub tree.
				if (Left != null)
				{
					return Left.Max;
				}

				// ancestor of node without left sub tree.
				var ancestor = Parent;

				var node = this;

				while ((ancestor != null) && (ancestor.Left == node))
				{
					node = ancestor;

					ancestor = ancestor.Parent;
				}

				return ancestor;
			}
		}

		/// <summary>
		/// Gets or sets the right leaf.
		/// </summary>
		internal ArneTreeNode<TKey, TValue> Right
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return right;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (right != null)
				{
					right.Parent = null;
				}

				right = value;

				if (right != null)
				{
					right.Parent = this;
				}
			}
		}

		/// <summary>
		/// Gets a node with minimum value in the right sub tree.
		/// </summary>
		internal ArneTreeNode<TKey, TValue> Successor
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				// Min of the right sub tree.
				if (Right != null)
				{
					return Right.Min;
				}

				// ancestor of node without right sub tree.
				var ancestor = Parent;

				var node = this;

				while ((ancestor != null) && (ancestor.Right == node))
				{
					node = ancestor;

					ancestor = ancestor.Parent;
				}

				return ancestor;
			}
		}

		#endregion

		#region Overrides of object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		[ExcludeFromCodeCoverage]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override String ToString()
		{
			return $"Node [Value {Value} Level {Level}]";
		}

		#endregion

		#region Methods

		/// <summary>
		/// Decreases the level of the tree.
		/// </summary>
		/// <returns>A tree with level decreased.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ArneTreeNode<TKey, TValue> DecraseLevel()
		{
			if (Left == null || Right == null)
			{
				return this;
			}

			var targetLevel = Math.Min(Left.Level, Right.Level) + 1;

			if (targetLevel >= Level)
			{
				return this;
			}

			Level = targetLevel;

			if (targetLevel < Right.Level)
			{
				Right.Level = targetLevel;
			}

			return this;
		}

		/// <summary>
		/// Makes a right rotation to replace a subtree containing a left horizontal link with one containing a right horizontal link instead.
		/// </summary>
		/// <returns>A node representing the rebalanced Arne Anderson tree.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ArneTreeNode<TKey, TValue> Skew()
		{
			if ((Left == null) || (Level != Left.Level))
			{
				return this;
			}

			// Swap the pointers of horizontal left links.
			var leftLeaf = Left;

			Left = leftLeaf.Right;

			leftLeaf.Right = this;

			return leftLeaf;
		}

		/// <summary>
		/// Makes a left rotation and level increase to replace a sub tree containing two or more consecutive right horizontal links with one containing two fewer consecutive right horizontal links.
		/// </summary>
		/// <returns>A node representing the rebalanced Arne Anderson tree.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ArneTreeNode<TKey, TValue> Split()
		{
			if (Right?.Right == null || (Right.Right.Level != Level))
			{
				return this;
			}

			// We have two horizontal right links.
			// Take the middle node, elevate it, and return it.
			var rightLeaf = Right;

			Right = rightLeaf.Left;

			rightLeaf.Left = this;

			rightLeaf.Level++;

			return rightLeaf;
		}

		#endregion
	}
}