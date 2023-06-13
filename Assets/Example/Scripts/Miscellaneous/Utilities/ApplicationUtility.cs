namespace Example
{
	using UnityEngine.XR.Management;

	public static class ApplicationUtility
	{
		// PUBLIC METHODS

		public static bool IsVREnabled()
		{
			return XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null && XRGeneralSettings.Instance.Manager.activeLoader != null;
		}
	}
}
