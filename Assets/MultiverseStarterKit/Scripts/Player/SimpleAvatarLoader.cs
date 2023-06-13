using UnityEngine;
using System;
using ReadyPlayerMe.AvatarLoader;

public sealed class SimpleAvatarLoader : SimpleRPMPlayer
{
    #region General Settings
    [SerializeField]
    private AvatarConfig avatarConfig;
    [Space]
    [Header("RPM Default Avatar")]
    [SerializeField]
    private GameObject baseAvatarObject;
    [SerializeField]
    private Transform baseArmature;
    [SerializeField]
    private Animator baseAnimator;

    [Space]
    [Header("Renderer")]
    [SerializeField]
    private SkinnedMeshRenderer rendererAvatar;
    [SerializeField]
    private SkinnedMeshRenderer rendererAvatarTransparent;

    [Space]
    [Header("Other Elements")]
    [SerializeField]
    private Transform referencesParent;

    [Space]
    [Header("Component Extras")]
    [SerializeField]
    private bool usingEyeAnimation;
    [SerializeField]
    private bool usingVoiceHandler;

    private string avatarUrl;
    private GameObject avatar;
    private AvatarObjectLoader avatarObjectLoader;
    private readonly Vector3 avatarPositionOffset = new(0, 0, 0);
    private int localAvatarType;
    private int userGender;
    private bool avatarLoaded;
    private bool onInitializedAvatar;
    #endregion

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (avatarLoaded || avatarUrl == null || onInitializedAvatar)
        {
            return;
        }

        LoadAvatar(avatarUrl);
        avatarLoaded = true;
    }

    #region RPC Receiver
    public void SetUserGender(int gendertype)
    {
        onInitializedAvatar = true;
        userGender = gendertype;

        SetupInitialGender();
    }
    public void SetLocalAvatar(int gendertype)
    {
        if (gendertype > 0)
        {
            localAvatarType = gendertype;
            LoadLocalAvatar(localAvatarType);
        }
    }
    public void SetRuntimeAvatar(string avatarurl)
    {
        if (!string.IsNullOrEmpty(avatarurl))
        {
            avatarLoaded = false;
            avatarUrl = avatarurl;
        }
    }
    #endregion

    #region Runtime Avatar Load
    private void LoadAvatar(string avatarurl)
    {
        if (Object.HasInputAuthority == true)
        {
            PlayerUI.SetAvatarLoadUIStatus(1);
        }

        avatarObjectLoader = new AvatarObjectLoader
        {
            AvatarConfig = avatarConfig
        };

        avatarObjectLoader.OnCompleted += OnLoadCompleted;
        avatarObjectLoader.OnFailed += OnLoadFailed;

        string loadAvatar = avatarurl.Trim(' ');
        avatarObjectLoader.LoadAvatar(loadAvatar);
    }
    private void OnLoadCompleted(object sender, CompletionEventArgs args)
    {
        SetupAvatar(args.Avatar);
    }
    private void OnLoadFailed(object sender, FailureEventArgs args)
    {
        if (Object.HasInputAuthority == true)
        {
            PlayerUI.SetAvatarLoadUIStatus(3);
        }

        SetupLocalAvatar(userGender);
    }
    #endregion

    #region Local Avatar Load
    private void LoadLocalAvatar(int gendertype)
    {
        if (Object.HasInputAuthority == true)
        {
            PlayerUI.SetAvatarLoadUIStatus(1);
        }

        SetupLocalAvatar(gendertype);
    }
    #endregion

    #region Avatar Processed
    private void SetupInitialGender()
    {
        switch (userGender)
        {
            case 1:
                SetupMasculineArmature();
                SetupInitialAvatarRenderer(RPMPlayerData.Objects.masculineAvatarObject);
                break;

            case 2:
                SetupFeminineArmature();
                SetupInitialAvatarRenderer(RPMPlayerData.Objects.feminineAvatarObject);
                break;

            default:
                // Handle any other cases or provide a default behavior
                break;
        }
    }
    private void SetupInitialAvatarRenderer(GameObject referenceAvatarObject)
    {
        rendererAvatarTransparent.gameObject.SetActive(false);

        SkinnedMeshRenderer[] allChildRenderer = referenceAvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer child in allChildRenderer)
        {
            if (child.gameObject.name == RPMConstant.AVATAR)
            {
                SkinnedMeshRenderer AvatarRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();

                rendererAvatar.sharedMaterial = AvatarRenderer.sharedMaterial;
                rendererAvatar.sharedMesh = AvatarRenderer.sharedMesh;
            }

            if (child.gameObject.name == RPMConstant.AVATAR_TRANSPARENT)
            {
                rendererAvatarTransparent.gameObject.SetActive(true);

                SkinnedMeshRenderer AvatarTransparentRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();

                rendererAvatarTransparent.sharedMaterial = AvatarTransparentRenderer.sharedMaterial;
                rendererAvatarTransparent.sharedMesh = AvatarTransparentRenderer.sharedMesh;
            }
        }

        onInitializedAvatar = false;
    }

    private void SetupAvatar(GameObject targetAvatar)
    {
        if (avatar != null)
        {
            Destroy(avatar);
        }

        avatar = targetAvatar;
        avatar.transform.parent = referencesParent.transform;
        avatar.transform.SetLocalPositionAndRotation(avatarPositionOffset, Quaternion.Euler(0, 0, 0));
        avatar.SetActive(false);

        OutfitGender gender = avatar.GetComponent<AvatarData>().AvatarMetadata.OutfitGender;

        switch (gender)
        {
            case OutfitGender.Masculine:
                SetupMasculineArmature();
                PlayerData.loadedAvatar = RPMEnums.LoadedAvatar.RuntimeMasculine;
                break;

            case OutfitGender.Feminine:
                SetupFeminineArmature();
                PlayerData.loadedAvatar = RPMEnums.LoadedAvatar.RuntimeFeminine;
                break;

            default:
                // Handle any other cases or provide a default behavior
                break;
        }

        SetupAvatarRenderer(avatar);
    }
    private void SetupLocalAvatar(int localAvatar)
    {
        switch (localAvatar)
        {
            case 1:
                SetupMasculineArmature();
                SetupAvatarRenderer(RPMPlayerData.Objects.masculineAvatarObject);
                break;

            case 2:
                SetupFeminineArmature();
                SetupAvatarRenderer(RPMPlayerData.Objects.feminineAvatarObject);
                break;

            default:
                // Handle any other cases or provide a default behavior
                break;
        }
    }

    private void SetupMasculineArmature()
    {
        foreach (Transform childTransformBase in baseArmature.GetComponentsInChildren<Transform>())
        {
            string childName = childTransformBase.gameObject.name;
            Transform armatureTransform = RPMPlayerData.Objects.masculineAvatarObject.transform.Find("Armature");

            Transform[] matchingChildTransformsMasculine = Array.FindAll(armatureTransform.GetComponentsInChildren<Transform>(), t => t.gameObject.name == childName);

            foreach (Transform matchingChildTransformMasculine in matchingChildTransformsMasculine)
            {
                childTransformBase.SetPositionAndRotation(matchingChildTransformMasculine.position, matchingChildTransformMasculine.rotation);
                childTransformBase.localScale = matchingChildTransformMasculine.localScale;
            }
        }

        baseAnimator.avatar = RPMPlayerData.Objects.masculineAvatar;
    }
    private void SetupFeminineArmature()
    {
        foreach (Transform childTransformBase in baseArmature.GetComponentsInChildren<Transform>())
        {
            string childName = childTransformBase.gameObject.name;
            Transform armatureTransform = RPMPlayerData.Objects.feminineAvatarObject.transform.Find("Armature");

            Transform[] matchingChildTransformsFeminine = Array.FindAll(armatureTransform.GetComponentsInChildren<Transform>(), t => t.gameObject.name == childName);

            foreach (Transform matchingChildTransformFeminine in matchingChildTransformsFeminine)
            {
                childTransformBase.SetPositionAndRotation(matchingChildTransformFeminine.position, matchingChildTransformFeminine.rotation);
                childTransformBase.localScale = matchingChildTransformFeminine.localScale;
            }
        }

        baseAnimator.avatar = RPMPlayerData.Objects.feminineAvatar;
    }

    private void SetupAvatarRenderer(GameObject referenceAvatarObject)
    {
        rendererAvatarTransparent.gameObject.SetActive(false);

        SkinnedMeshRenderer[] allChildRenderer = referenceAvatarObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer child in allChildRenderer)
        {
            if (child.gameObject.name == RPMConstant.AVATAR)
            {
                SkinnedMeshRenderer AvatarRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();

                rendererAvatar.sharedMaterial = AvatarRenderer.sharedMaterial;
                rendererAvatar.sharedMesh = AvatarRenderer.sharedMesh;
            }

            if (child.gameObject.name == RPMConstant.AVATAR_TRANSPARENT)
            {
                rendererAvatarTransparent.gameObject.SetActive(true);

                SkinnedMeshRenderer AvatarTransparentRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();

                rendererAvatarTransparent.sharedMaterial = AvatarTransparentRenderer.sharedMaterial;
                rendererAvatarTransparent.sharedMesh = AvatarTransparentRenderer.sharedMesh;
            }
        }

        SetupExtras();
    }
    private void SetupExtras()
    {
        if (usingEyeAnimation && !baseAvatarObject.TryGetComponent(out EyeAnimationHandler eyeAnimationHandler))
        {
            baseAvatarObject.AddComponent<EyeAnimationHandler>();
        }

        if (usingVoiceHandler && !baseAvatarObject.TryGetComponent(out VoiceHandler voiceHandler))
        {
            baseAvatarObject.AddComponent<VoiceHandler>();
        }

        SetupCompleted();
    }
    private void SetupCompleted()
    {
        if (Object.HasInputAuthority == true)
        {
            PlayerUI.SetAvatarLoadUIStatus(2);
        }
    }
    #endregion
}