namespace Fusion.KCC
{
	using System.Collections.Generic;

	/// <summary>
	/// Partial implementation of KCC class - use this to extend API with your own functionality - sprint, crouch, ...
	/// Storing information usually requires adding a property in KCCData which has support for rollback.
	/// </summary>
	public partial class KCC
	{
		// PUBLIC METHODS

		public void SetSprint(bool sprint)
		{
			// Only input and state authority can modify the state
			if (HasAnyAuthority == false)
				return;

			// Set Sprint property in KCCData instance. This assignment is done ONLY on current KCCData instance (_fixedData for fixed update, _renderData for render update).
			// If you call this method after KCC fixed update, it will NOT propagate to render for the same frame.
			Data.Sprint = sprint;

			// More correct approach in this case is to explicitly set Sprint for render data and fixed data, this way you'll not lose sprint information for following render frames.
			/*
			_renderData.Sprint = sprint;

			if (IsInFixedUpdate == true)
			{
				_fixedData.Sprint = sprint;
			}
			*/

			// And even more correct is to force SetSprint() being called always before the update.
		}

		// Here you can add your own methods

		// PARTIAL METHODS

		partial void InitializeUserNetworkProperties(KCCNetworkContext networkContext, List<IKCCNetworkProperty> networkProperties)
		{
			// By default KCC supports network synchronization for position, look rotation, settings properties, collisions, modifiers and ignored colliders.
			// If you need more properties to synchronize which cannot be deduced or precision of the deduced value is not sufficient, you can add your own entries.
			// Same applies for opposite direction, you can remove properties from the list if you don't want to synchronize them.

			//networkProperties.Add(new KCCNetworkBool<KCCNetworkContext>(networkContext, (context, value) => context.Data.Sprint = value, (context) => context.Data.Sprint, null));
		}

		partial void InterpolateUserNetworkData(InterpolationData interpolationData)
		{
			// At this point, all networked properties are already interpolated in _fixedData, including your own added in InitializeUserNetworkProperties().
			// Data which is not networked needs to be deduced somehow because it might not be available at all (proxies).

			// As example, KCCData.RealVelocity and KCCData.RealSpeed are not networked.
			// On proxies they are calculated from position difference in snapshots you are interpolating between.

			// All changes should be done on _fixedData which will automatically propagate to _renderData after this method ends.
		}
	}
}
