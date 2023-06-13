using UnityEngine;
using Example;
using ReadyPlayerMe.Core;
using ReadyPlayerMe.Core.Data;
using Fusion;
using System.Collections.Generic;

public class AvatarUrlWebView : NetworkBehaviour
{
    private const string TAG = nameof(AvatarUrlWebView);
    private Player _localPlayer;

    [SerializeField]
    private RuntimeAvatarSetterUI avatarSetterUI;

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        CoreSettings partner = CoreSettingsHandler.CoreSettings; // Get the partner's CoreSettings     
        WebInterface.SetupRpmFrame(partner.Subdomain); // Set up the RPM frame using the partner's subdomain
#endif
    }
    public void OnAvatarUrlGenerated(string generatedUrl)
    {
        if (!string.IsNullOrEmpty(generatedUrl))
        {
            if (avatarSetterUI.gameObject.activeInHierarchy)
            {
                avatarSetterUI.OnsetAvatarUrlWebView(generatedUrl);
            }
            else
            {
                Player player = GetLocalPlayer();
                if (player == null)
                    return;

                if (player.TryGetComponent(out SimpleRPMPlayer simpleRPMPlayer))
                {
                    simpleRPMPlayer.PlayerData.OnRuntimeAvatarSet(generatedUrl);
                    simpleRPMPlayer.PlayerUI.SetAvatarLoadUIStatus(1);
                    SetAvatarUrl(generatedUrl);
                }
            }
        }
    }
    private void SetAvatarUrl(string avatarurl)
    {
        RPMPlayerData.SetData(RPMPlayerData.Keys.runtimeAvatarKey, avatarurl);
    }
    private Player GetLocalPlayer()
    {
        if (Runner == null)
            return default; // Return default value if Runner is null

        PlayerRef localPlayerRef = Runner.LocalPlayer; // Get the reference to the local player from the Runner

        if (localPlayerRef.IsValid == false)
            return default; // Return default value if the local player reference is not valid

        if (_localPlayer == null)
        {
            _localPlayer = null;

            List<Player> players = Runner.SimulationUnityScene.GetComponents<Player>(); // Get all Player components in the scene

            for (int i = 0, count = players.Count; i < count; ++i)
            {
                Player player = players[i];
                if (player.Object != null && player.Object.InputAuthority == localPlayerRef)
                {
                    _localPlayer = player; // Assign the player as the local player if it matches the local player reference
                    break;
                }
            }
        }

        return _localPlayer; // Return the local player
    }
}
