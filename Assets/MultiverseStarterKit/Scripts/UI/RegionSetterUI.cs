using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public partial class RegionSetterUI : SimpleRPMMenu
{
    [SerializeField]
    private TMP_Dropdown dropdown;
    private UnityAction<int> dropdownListener;
    private void OnEnable()
    {
        string[] options = new string[] { "us", "eu", "asia" };

        dropdown.AddOptions(new List<string>(options));

        dropdownListener = (index) =>
        {
            string region = dropdown.options[index].text;
            Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion = region;

            RPMPlayerData.SetData(RPMPlayerData.Keys.regionNameKey, region);
            Debug.Log($"Setting region to {region}");
        };

        dropdown.onValueChanged.AddListener(dropdownListener);

        string curRegion = Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion;

        if (string.IsNullOrEmpty(curRegion))
        {
            curRegion = RPMPlayerData.GetData(RPMPlayerData.Keys.regionNameKey);
        }
        else
        {
            RPMPlayerData.SetData(RPMPlayerData.Keys.regionNameKey, curRegion);
            Debug.Log($"Initial region is {curRegion}");

            int curIndex = dropdown.options.FindIndex((op) => op.text == curRegion);
            dropdown.value = curIndex != -1 ? curIndex : 0;
        }
    }
    private void OnDisable()
    {
        dropdown.onValueChanged.RemoveListener(dropdownListener);
    }
    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(dropdownListener);
    }
}