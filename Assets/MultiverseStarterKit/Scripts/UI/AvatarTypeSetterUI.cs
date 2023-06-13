using UnityEngine;
using UnityEngine.UI;

public partial class AvatarTypeSetterUI : SimpleRPMMenu
{
    [SerializeField]
    private Toggle localAvatarToggle;
    [SerializeField]
    private Toggle runtimeAvatarToggle;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private GameObject connectAs;
    [SerializeField]
    private GameObject runtimeAvatar;

    private void OnEnable()
    {
        localAvatarToggle.isOn = true;
    }
    private void LateUpdate()
    {
        CheckToggle();
    }
    public void OnSetAvatarType()
    {
        if (localAvatarToggle.isOn)
        {
            connectAs.SetActive(true);
            PlayerPrefs.DeleteKey(RPMPlayerData.Keys.runtimeAvatarKey);
        }

        if (runtimeAvatarToggle.isOn)
        {
            runtimeAvatar.SetActive(true);
            PlayerPrefs.DeleteKey(RPMPlayerData.Keys.localAvatarKey);
        }
    }
    private void CheckToggle()
    {
        nextButton.interactable = localAvatarToggle.isOn || runtimeAvatarToggle.isOn;
    }
}
