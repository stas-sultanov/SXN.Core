using System.Runtime.CompilerServices;

namespace System.ServiceModel
{
	/// <summary>
	/// Represents the transition of the state of the entity.
	/// </summary>
	public struct EntityStateTransition : IEquatable<EntityStateTransition>
	{
		#region Fields

		/// <summary>
		/// An initial state of the entity.
		/// </summary>
		private readonly EntityState initialState;

		/// <summary>
		/// A target state of the entity.
		/// </summary>
		private readonly EntityState targetState;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of <see cref="EntityStateTransition" /> structure.
		/// </summary>
		/// <param name="initialState">An initial state of the entity.</param>
		/// <param name="targetState">A target state of the entity.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityStateTransition(EntityState initialState, EntityState targetState)
		{
			this.initialState = initialState;

			this.targetState = targetState;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the initial state of the entity.
		/// </summary>
		public EntityState InitialState => initialState;

		/// <summary>
		/// Gets the target state of the entity.
		/// </summary>
		public EntityState TargetState => targetState;

		#endregion

		#region Overrides of ValueType

		/// <summary>
		/// Returns a value that indicates whether this instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><c>true</c> if <paramref name="obj" /> is a <see cref="EntityStateTransition" /> that has the same value as this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override Boolean Equals(Object obj)
		{
			if (obj is EntityStateTransition)
			{
				return Equals((EntityStateTransition) obj);
			}

			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override Int32 GetHashCode()
		{
			return initialState.GetHashCode() ^ targetState.GetHashCode();
		}

		#endregion

		#region Methods of IEquatable<EntityStateTransition>

		/// <summary>
		/// Returns a value indicating whether this instance and a specified <see cref="EntityStateTransition" /> represent the same value.
		/// </summary>
		/// <param name="other">An object to compare to this instance.</param>
		/// <returns><c>true</c> if <paramref name="other" /> is equal to this instance; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Boolean Equals(EntityStateTransition other)
		{
			return (initialState == other.initialState) && (targetState == other.targetState);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Indicates whether the values of two specified <see cref="EntityStateTransition" /> objects are equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator ==(EntityStateTransition first, EntityStateTransition second)
		{
			return first.Equals(second);
		}

		/// <summary>
		/// Indicates whether the values of two specified <see cref="EntityStateTransition" /> objects are not equal.
		/// </summary>
		/// <param name="first">The first object to compare.</param>
		/// <param name="second">The second object to compare.</param>
		/// <returns><c>true</c> if <paramref name="first" /> and <paramref name="second" /> are not equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean operator !=(EntityStateTransition first, EntityStateTransition second)
		{
			return !first.Equals(second);
		}

		#endregion
	}
}