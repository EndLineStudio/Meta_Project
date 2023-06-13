namespace Fusion.Plugin
{
	/// <summary>
	/// This class exposes access to internal structures and properties further used by other utilities.
	/// </summary>
	public static class FusionExtensions
	{
		// PUBLIC MEMBERS

		public static NetworkAreaOfInterestBehaviour GetAOIPositionSource(this NetworkObject networkObject)
		{
			return networkObject.AoiPositionSource;
		}

		public static double GetRTT(this Simulation simulation)
		{
			return simulation is Simulation.Client client ? client.RttToServer : 0.0;
		}
	}
}
