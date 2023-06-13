using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class PlayerNameSetterUI : SimpleRPMMenu
{
    [SerializeField]
    private TMP_InputField playerNameInputField;
    [SerializeField]
    private Button nextButton;

    private void OnEnable()
    {
        playerNameInputField.characterLimit = RPMPlayerData.Keys.playerNameLength;
        playerNameInputField.contentType = TMP_InputField.ContentType.Alphanumeric;

        RPMPlayerData.LoadInputFieldData(playerNameInputField, RPMPlayerData.Keys.playerNameKey, RPMPlayerData.Keys.playerNameLength);
        CheckPlayerName();
    }
    private void Start()
    {
        playerNameInputField.onValueChanged.AddListener(SetPlayerName);
        SetPlayerName(playerNameInputField.text);
    }
    private void CheckPlayerName()
    {
        (bool isEmpty, bool isValid) = RPMPlayerData.CheckInputFieldData(playerNameInputField, RPMPlayerData.Keys.playerNameLength);
        nextButton.interactable = !isEmpty && isValid;
    }
    private void SetPlayerName(string playerName)
    {
        RPMPlayerData.SetData(RPMPlayerData.Keys.playerNameKey, playerName);
        CheckPlayerName();
    }

    private void OnDisable()
    {
        playerNameInputField.onValueChanged.RemoveListener(SetPlayerName);
    }
    private void OnDestroy()
    {
        playerNameInputField.onValueChanged.RemoveListener(SetPlayerName);
    }
}
