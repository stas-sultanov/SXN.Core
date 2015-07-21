using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.ServiceModel
{
	/// <summary>
	/// Provides a set of extension methods for the <see cref="EntityState"/> enumeration.
	/// </summary>
	public static class EntityStateEx
	{
		#region Methods

		/// <summary>
		/// Determines whether the state of the entity can be changed from the <paramref name="source"/> state to the <paramref name="target"/> state.
		/// </summary>
		/// <param name="workflow">The workflow of the entity.</param>
		/// <param name="source">The source state of the entity.</param>
		/// <param name="target">The target state of the entity.</param>
		/// <returns><c>true</c> if state can be change to the target one, <c>false</c> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Boolean CanChangeTo(this IEnumerable<EntityStateTransition> workflow, EntityState source, EntityState target)
		{
			// Return value
			return workflow.Any(transition => transition.InitialState == source && transition.TargetState == target);
		}

		/// <summary>
		/// Changes the state of the entity from the <paramref name="source"/> state to the <paramref name="target"/> state.
		/// </summary>
		/// <param name="workflow">The workflow of the entity.</param>
		/// <param name="source">The source state of the entity.</param>
		/// <param name="target">The target state of the entity.</param>
		/// <exception cref="InvalidOperationException">State can not be changed from the <paramref name="source"/> state to the <paramref name="target"/> state.</exception>
		/// <returns>The original value of <paramref name="source"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static EntityState ChangeTo(this IEnumerable<EntityStateTransition> workflow, ref EntityState source, EntityState target)
		{
			if (!workflow.CanChangeTo(source, target))
			{
				var message = String.Format(CultureInfo.InvariantCulture, "State can not be changed from [{0}] to [{1}].", source, target);

				throw new InvalidOperationException(message);
			}

			var oldState = source;

			source = target;

			return oldState;
		}

		#endregion
	}
}