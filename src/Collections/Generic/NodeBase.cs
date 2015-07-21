using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
	/// <summary>
	/// Represents a base class for nodes.
	/// </summary>
	/// <typeparam name="T">The type of the value that will be associated with the node.</typeparam>
	public abstract class NodeBase<T>
		where T : IComparable<T>
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="NodeBase{T}"/> class.
		/// </summary>
		/// <param name="value">The data to associate with a node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected NodeBase(T value)
		{
			Value = value;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The value associated with the node.
		/// </summary>
		public T Value
		{
			get;
		}

		#endregion
	}
}