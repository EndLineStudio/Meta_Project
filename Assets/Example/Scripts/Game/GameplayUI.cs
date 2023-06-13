namespace Example
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using Fusion;
	using Fusion.KCC;

	/// <summary>
	///	Interface to get custom status from providers.
	/// </summary>
	public interface IMapStatusProvider
	{
		bool   IsActive(PlayerRef player);
		string GetStatus(PlayerRef player);
	}

	/// <summary>
	/// Shows information related to gameplay.
	/// </summary>
	public sealed class GameplayUI : NetworkBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private GameObject _mapStatus;
		[SerializeField]
		private Text       _mapStatusText;

		private PlayerRef _localPlayer;
		private KCC       _localPlayerKCC;

		private List<IMapStatusProvider> _statusProviders = new List<IMapStatusProvider>();

		// MonoBehaviour INTERFACE

		private void Update()
		{
			string mapStatus = default;

			if (GetLocalPlayerKCC(out KCC localPlayerKCC, out PlayerRef localPlayer) == true)
			{
				// Get all processors from KCC that implement IMapStatusProvider.
				localPlayerKCC.GetProcessors<IMapStatusProvider>(_statusProviders);

				// Iterate over processors and try to collect status.
				for (int i = 0; i < _statusProviders.Count; ++i)
				{
					IMapStatusProvider statusProvider = _statusProviders[i];
					if (statusProvider.IsActive(localPlayer) == true)
					{
						mapStatus = statusProvider.GetStatus(localPlayer);
						break;
					}
				}

				if (mapStatus == default)
				{
					// Following is an example of incorrect usage (unless you 100% know what you are doing):
					// No status provided from processors above, let's try to iterate over all collision hits.
					// This approach works well until is a condition which defer a processor from starting the interaction (controlled by overriding IKCCInteractionProvider.CanStartInteraction()),
					// you'll see map status even if the interaction not yet started.
					// Please be careful when processing raw collision hits.

					KCCHits hits = localPlayerKCC.Data.Hits;
					for (int i = 0; i < hits.Count; ++i)
					{
						IMapStatusProvider statusProvider = hits.All[i].Transform.GetComponentNoAlloc<IMapStatusProvider>();
						if (statusProvider != null && statusProvider.IsActive(localPlayer) == true)
						{
							mapStatus = statusProvider.GetStatus(localPlayer);
							break;
						}
					}
				}
			}

			if (string.IsNullOrEmpty(mapStatus) == true)
			{
				_mapStatus.SetActive(false);
			}
			else
			{
				_mapStatus.SetActive(true);
				_mapStatusText.text = mapStatus;
			}
		}

		// PRIVATE METHODS

		private bool GetLocalPlayerKCC(out KCC localPlayerKCC, out PlayerRef localPlayer)
		{
			localPlayer    = _localPlayer;
			localPlayerKCC = _localPlayerKCC;

			if (localPlayerKCC != null)
				return true;

			if (Runner == null)
				return false;

			localPlayer = Runner.LocalPlayer;
			_localPlayer = localPlayer;

			if (localPlayer.IsValid == false)
				return false;

			if (Runner.TryGetPlayerObject(localPlayer, out NetworkObject localPlayerObject) == false || localPlayerObject == null)
				return false;

			localPlayerKCC = localPlayerObject.GetComponentNoAlloc<KCC>();
			_localPlayerKCC = localPlayerKCC;

			return localPlayerKCC != null;
		}
	}
}
