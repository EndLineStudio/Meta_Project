namespace Fusion.KCC
{
	// Partial implementation of KCCCollision class - use this to extend with your own functionality.
	// KCCCollision is a container which contains information related to single collision.
	public partial class KCCCollision
	{
		// PUBLIC MEMBERS

		// Put your properties here.
		// This instance is pooled, only stateless getters!

		// Example: return HUD information if this modifier provides it => for displaying on screen.
		// IHUDProvider must be derived from IInteractionProvider
		// public IHUD HUD => Provider is IHUDProvider hudProvider ? hudProvider.HUD : null;
	}

	// Partial implementation of KCCCollisions class - use this to extend with your own functionality.
	// KCCCollisions is a collection which maintains all collisions.
	public partial class KCCCollisions
	{
		// PUBLIC METHODS

		// Put your methods here.
		// You can iterate over All property and apply custom filter.

		// Example: returns HUD provider of a specific type.
		/*
		public T GetHUDProvider<T>() where T : IHUDProvider
		{
			for (int i = 0, count = All.Count; i < count; ++i)
			{
				if (All[i].Provider is T hudProvider)
					return hudProvider;
			}

			return null;
		}
		*/
	}
}
