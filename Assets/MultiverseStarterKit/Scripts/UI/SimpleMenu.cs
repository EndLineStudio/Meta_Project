using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Example;

public sealed class SimpleMenu : NetworkBehaviour
{
    [SerializeField]
    private GUISkin skin;
    [SerializeField]
    private bool showMenuUI;
    [SerializeField]
    private GameObject controllUI;
    [SerializeField]
    private bool showControllUI;

    private Player localPlayer;
    private GUIStyle defaultStyle;
    private GUIStyle selectedStyle;

    [SerializeField]
    private string[] testAvatarUrls; // Array of test avatar URLs

    private bool showCustomMenu;
    private bool isCameraInCloseView = false;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.cKey.wasPressedThisFrame == true)
        {
            showCustomMenu = !showCustomMenu;
        }
#if !UNITY_EDITOR && UNITY_WEBGL
            if (Keyboard.current.oKey.wasPressedThisFrame == true)
            {
				OnCreateAvatar();
            }
#endif
        if (Keyboard.current.rKey.wasPressedThisFrame == true)
        {
            OnRetryLoadAvatar();
        }
        if (Keyboard.current.lKey.wasPressedThisFrame == true)
        {
            OnTestLoadAvatarUrl();
        }
        if (Keyboard.current.mKey.wasPressedThisFrame == true)
        {
            OnSetLocalAvatarMasculine();
        }
        if (Keyboard.current.fKey.wasPressedThisFrame == true)
        {
            OnSetLocalAvatarFeminine();
        }
        if (Keyboard.current.hKey.wasPressedThisFrame == true)
        {
            SetCamera();
        }
        if (Keyboard.current.uKey.wasPressedThisFrame == true)
        {
            ToggleControllUI();
        }
    }
    private void OnGUI()
    {
        if (showMenuUI == false)
            return;

        Initialize();

        if (Runner == null || Runner.IsRunning == false)
            return;

        if (showControllUI)
        {
            controllUI.SetActive(true);
        }

        float verticalSpace = 5.0f;
        float horizontalSpace = 5.0f;

        GUILayout.BeginVertical();
        GUILayout.Space(verticalSpace);
        GUILayout.BeginHorizontal();
        GUILayout.Space(horizontalSpace);

        if (GUILayout.Button("[C]", showCustomMenu == true ? selectedStyle : defaultStyle) == true)
        {
            showCustomMenu = !showCustomMenu;
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(horizontalSpace);

        if (showCustomMenu == true)
        {
            GUILayout.BeginVertical();
#if !UNITY_EDITOR && UNITY_WEBGL
                if (GUILayout.Button($"[O] Avatar Creator", defaultStyle) == true)
                {
                    OnCreateAvatar();
                }
#endif

            if (GUILayout.Button($"[L] Test Load Avatar", defaultStyle) == true)
            {
                OnTestLoadAvatarUrl();
            }
            if (GUILayout.Button($"[R] Retry Load Avatar", defaultStyle) == true)
            {
                OnRetryLoadAvatar();
            }
            if (GUILayout.Button($"[M] Default Masculine", defaultStyle) == true)
            {
                OnSetLocalAvatarMasculine();
            }
            if (GUILayout.Button($"[F] Default Feminine", defaultStyle) == true)
            {
                OnSetLocalAvatarFeminine();
            }
            if (GUILayout.Button($"[H] Change View", defaultStyle) == true)
            {
                SetCamera();
            }
            if (GUILayout.Button($"[U] Show/Hide Controll UI", defaultStyle) == true)
            {
                ToggleControllUI();
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void Initialize()
    {
        if (defaultStyle == null)
        {
            defaultStyle = new GUIStyle(skin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };

            if (Application.isMobilePlatform == true && Application.isEditor == false)
            {
                defaultStyle.fontSize = 20;
                defaultStyle.padding = new RectOffset(20, 20, 20, 20);
            }

            selectedStyle = new GUIStyle(defaultStyle);
            selectedStyle.normal.textColor = Color.green;
            selectedStyle.focused.textColor = Color.green;
            selectedStyle.hover.textColor = Color.green;
        }
    }

#if !UNITY_EDITOR && UNITY_WEBGL
    private void OnCreateAvatar() 		// Open Ready Player Me Avatar Creation
    {
            Player player = GetLocalPlayer();
            if (player == null)
                return;

			WebInterface.SetIFrameVisibility(true);
    }
#endif
    private void OnRetryLoadAvatar()         // Retrying load avatar
    {
        Player player = GetLocalPlayer();
        if (player == null)
            return;

        string existingAvatarUrl = PlayerPrefs.GetString(RPMPlayerData.Keys.runtimeAvatarKey);
        if (!string.IsNullOrEmpty(existingAvatarUrl))
        {
            Debug.Log(existingAvatarUrl);

            StartCoroutine(RetryLoadAvatar(existingAvatarUrl));
        }
    }
    private IEnumerator RetryLoadAvatar(string existingAvatarUrl)
    {
        Player player = GetLocalPlayer();
        if (player == null)
            yield break;

        if (player.TryGetComponent(out SimpleRPMPlayer rpmPlayer))
        {
            rpmPlayer.PlayerData.NetworkAvatarUrl = string.Empty;

            yield return new WaitForSeconds(0.1f);

            PlayerPrefs.SetString(RPMPlayerData.Keys.runtimeAvatarKey, existingAvatarUrl);

            yield return new WaitForSeconds(0.1f);

            rpmPlayer.PlayerData.OnRuntimeAvatarSet(existingAvatarUrl);
            rpmPlayer.PlayerUI.SetAvatarLoadUIStatus(1);
        }
    }
    private void OnTestLoadAvatarUrl()      // Test load avatar from a list of avatar url
    {
        Player player = GetLocalPlayer();
        if (player == null)
            return;

        if (testAvatarUrls != null && testAvatarUrls.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, testAvatarUrls.Length);
            string randomAvatarUrl = testAvatarUrls[randomIndex];

            if (player.TryGetComponent(out SimpleRPMPlayer rpmPlayer))
            {
                rpmPlayer.PlayerData.OnRuntimeAvatarSet(randomAvatarUrl);
                rpmPlayer.PlayerUI.SetAvatarLoadUIStatus(1);
                PlayerPrefs.SetString(RPMPlayerData.Keys.runtimeAvatarKey, randomAvatarUrl);

            }
        }
    }
    private void OnSetLocalAvatarMasculine()        // Test load masculine local avatar
    {
        Player player = GetLocalPlayer();
        if (player == null)
            return;

        if (player.TryGetComponent(out SimpleRPMPlayer rpmPlayer))
        {
            rpmPlayer.PlayerData.OnLocalAvatarSet(1);
            rpmPlayer.PlayerUI.SetAvatarLoadUIStatus(1);
            PlayerPrefs.SetInt(RPMPlayerData.Keys.localAvatarKey, 1);
        }
    }
    private void OnSetLocalAvatarFeminine() // Test load feminine local avatar
    {
        Player player = GetLocalPlayer();
        if (player == null)
            return;

        if (player.TryGetComponent(out SimpleRPMPlayer rpmPlayer))
        {
            rpmPlayer.PlayerData.OnLocalAvatarSet(2);
            rpmPlayer.PlayerUI.SetAvatarLoadUIStatus(1);
            PlayerPrefs.SetInt(RPMPlayerData.Keys.localAvatarKey, 2);
        }
    }
    private void SetCamera() // Toggle change camera view
    {
        Player player = GetLocalPlayer();
        if (player == null)
            return;

        if (player.TryGetComponent(out SimpleCameraChange cameraChange))
        {
            isCameraInCloseView = !isCameraInCloseView;

            if (isCameraInCloseView)
                cameraChange.SetCameraCloseView();
            else
                cameraChange.SetCameraOriginalView();
        }
    }
    private void ToggleControllUI()
    {
        Player player = GetLocalPlayer();
        if (player == null)
            return;

        showControllUI = !showControllUI;

        if (showControllUI)
            controllUI.SetActive(true);
        else
            controllUI.SetActive(false);
    } // Toggle controll UI active
    private Player GetLocalPlayer()
    {
        if (Runner == null)
            return default;

        PlayerRef localPlayerRef = Runner.LocalPlayer;
        if (localPlayerRef.IsValid == false)
            return default;

        if (localPlayer == null)
        {
            localPlayer = null;

            List<Player> players = Runner.SimulationUnityScene.GetComponents<Player>();
            for (int i = 0, count = players.Count; i < count; ++i)
            {
                Player player = players[i];
                if (player.Object != null && player.Object.InputAuthority == localPlayerRef)
                {
                    localPlayer = player;
                    break;
                }
            }
        }

        return localPlayer;
    }
}