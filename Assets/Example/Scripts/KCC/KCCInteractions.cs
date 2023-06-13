namespace Fusion.KCC
{
	// Partial implementation of generic KCCInteraction class - use this to extend with your own functionality.
	// KCCInteraction is a container which contains information related to single interaction.
	// KCCInteraction is abstract class, extending it also extends derived classes (all other interaction container types)!
	public abstract partial class KCCInteraction<TInteraction>
	{
		// PUBLIC MEMBERS

		// Put your properties here.
		// This instance is pooled, only stateless getters!

		// Example: return HUD information if this interaction provides it => for displaying on screen.
		// IHUDProvider must be derived from IInteractionProvider
		// public IHUD HUD => Provider is IHUDProvider hudProvider ? hudProvider.HUD : null;
	}

	// Partial implementation of KCCInteractions class - use this to extend with your own functionality.
	// KCCInteractions is a collection which maintains all interactions.
	// KCCInteractions is abstract class, extending it also extends derived classes (all other interaction collection types)!
	public abstract partial class KCCInteractions<TInteraction>
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
