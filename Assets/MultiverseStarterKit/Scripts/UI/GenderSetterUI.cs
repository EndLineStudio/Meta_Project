using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public partial class GenderSetterUI : SimpleRPMMenu
{
    [SerializeField]
    private TMP_Dropdown genderDropdown;
    [SerializeField]
    private Button nextButton;

    private void OnEnable()
    {
        string[] genderOptions = new string[] {"Select", "male", "female" };
        genderDropdown.AddOptions(new List<string>(genderOptions));
    }
    private void Start()
    {
        genderDropdown.onValueChanged.AddListener(SetLocalAvatar);

        int curGender = PlayerPrefs.GetInt(RPMPlayerData.Keys.genderKey);
        if (curGender > 0)
        {
            genderDropdown.value = curGender;
        }

        SetLocalAvatar(genderDropdown.value);
    }
    public void LateUpdate()
    {
        CheckGenderDropdown();
    }

    public void OnSetLocalAvatar()
    {
        int genderIndex = genderDropdown.value;
        SetLocalAvatar(genderIndex);
    }
    private void CheckGenderDropdown()
    {
        bool isGenderSelected = genderDropdown.value != 0;
        nextButton.interactable = isGenderSelected;
    }
    private void SetLocalAvatar(int genderIndex)
    {
        if (genderIndex == 0)
        {
            PlayerPrefs.DeleteKey(RPMPlayerData.Keys.localAvatarKey);
            PlayerPrefs.DeleteKey(RPMPlayerData.Keys.genderKey);
        }
        else
        {
            PlayerPrefs.SetInt(RPMPlayerData.Keys.localAvatarKey, genderIndex);
            PlayerPrefs.SetInt(RPMPlayerData.Keys.genderKey, genderIndex);
        }

        Debug.Log($"Gender set to: "+ genderIndex);
    }

    private void OnDisable()
    {
        genderDropdown.onValueChanged.RemoveListener(SetLocalAvatar);
    }
    private void OnDestroy()
    {
        genderDropdown.onValueChanged.RemoveListener(SetLocalAvatar);
    }
}
