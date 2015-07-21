namespace System.ServiceModel
{
	/// <summary>
	/// Describes state of the entity.
	/// </summary>
	public enum EntityState
	{
		/// <summary>
		/// The entity has no state.
		/// </summary>
		None = 0x00,

		/// <summary>
		/// The entity is inactive.
		/// </summary>
		Inactive = 0x01,

		/// <summary>
		/// The entity is active.
		/// </summary>
		Active = 0x02,

		/// <summary>
		/// The entity is deleted.
		/// </summary>
		Retired = 0x04,

		/// <summary>
		/// The entity is transitioning.
		/// </summary>
		Transitioning = 0x80,

		/// <summary>
		/// The entity is updating it's state.
		/// </summary>
		Updating = Transitioning,

		/// <summary>
		/// The entity is transitioning from the <see cref="None"/> to the <see cref="Inactive"/> state.
		/// </summary>
		Creating = Transitioning | (None << 8) | (Inactive << 16),

		/// <summary>
		/// The entity is transitioning from the <see cref="Inactive"/> to the <see cref="Active"/> state.
		/// </summary>
		Activating = Transitioning | (Inactive << 8) | (Active << 16),

		/// <summary>
		/// The entity is transitioning from the <see cref="Active"/> to the <see cref="Inactive"/> state.
		/// </summary>
		Deactivating = Transitioning | (Active << 8) | (Inactive << 16),

		/// <summary>
		/// The entity is transitioning from the <see cref="Inactive"/> to the <see cref="Retired"/> state.
		/// </summary>
		Retiring = Transitioning | (Inactive << 8) | (Retired << 16),

		/// <summary>
		/// The entity is transitioning from the <see cref="Retired"/> to the <see cref="Inactive"/> state.
		/// </summary>
		Restoring = Transitioning | (Retired << 8) | (Inactive << 16)
	}
}