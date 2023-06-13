using UnityEngine;

public static class RPMPlayerData
{
    private static RPMPlayerRefs rpmPlayerRefs;
    public static RPMPlayerRefs Keys
    {
        get
        {
            if (rpmPlayerRefs == null)
            {
                rpmPlayerRefs = Resources.Load<RPMPlayerRefs>("RPMPlayerRefs");
            }
            return rpmPlayerRefs;
        }
    }
    private static RPMAvatarRefs rpmAvatarRefs;
    public static RPMAvatarRefs Objects
    {
        get
        {
            if (rpmAvatarRefs == null)
            {
                rpmAvatarRefs = Resources.Load<RPMAvatarRefs>("RPMAvatarRefs");
            }
            return rpmAvatarRefs;
        }
    }
    public static void LoadInputFieldData(TMPro.TMP_InputField inputField, string playerPrefsKey, int charLength)
    {
        if (PlayerPrefs.HasKey(playerPrefsKey))
        {
            string data = PlayerPrefs.GetString(playerPrefsKey);
            inputField.SetTextWithoutNotify(data);
        }
        else
        {
            string randomData = SimpleRandomInputField.GenerateRandomText(charLength);
            inputField.SetTextWithoutNotify(randomData);
        }
    }
    public static (bool isEmpty, bool isValid) CheckInputFieldData(TMPro.TMP_InputField inputField, int charLength)
    {
        string inputFieldText = inputField.text;

        bool isEmpty = string.IsNullOrEmpty(inputFieldText);
        bool isValid = inputFieldText.Length == charLength;

        return (isEmpty, isValid);
    }
    public static void SetData(string playerPrefsKey, string setValue)
    {
        if (string.IsNullOrEmpty(setValue))
        {
            PlayerPrefs.DeleteKey(playerPrefsKey);
        }
        else
        {
            PlayerPrefs.SetString(playerPrefsKey, setValue);
        }

        Debug.Log(playerPrefsKey + " updated to: " + setValue);
    }
    public static string GetData(string playerPrefsKey)
    {
        string playerPrefsdata = PlayerPrefs.GetString(playerPrefsKey);
        return playerPrefsdata;
    }
}

public class RPMPlayerDataCache
{
    public string playerName;
    public string runtimeAvatarUrl;
    public int gender;
    public int localAvatar;
    public string roomName;
    public string regionName;
}
