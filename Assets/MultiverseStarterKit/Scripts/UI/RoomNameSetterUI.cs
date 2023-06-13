using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class RoomNameSetterUI : SimpleRPMMenu
{
    [SerializeField]
    private TMP_InputField roomNameInputField;
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button joinButton;

    private void OnEnable()
    {
        roomNameInputField.characterLimit = RPMPlayerData.Keys.roomNameLength;
        roomNameInputField.contentType = TMP_InputField.ContentType.Alphanumeric;

        RPMPlayerData.LoadInputFieldData(roomNameInputField, RPMPlayerData.Keys.roomNameKey, RPMPlayerData.Keys.roomNameLength);
        CheckRoomName();
    }
    private void Start()
    {
        roomNameInputField.onValueChanged.AddListener(SetRoomName);
        SetRoomName(roomNameInputField.text);
    }

    private void CheckRoomName()
    {
        (bool isEmpty, bool isValid) = RPMPlayerData.CheckInputFieldData(roomNameInputField, RPMPlayerData.Keys.roomNameLength);

        hostButton.interactable = !isEmpty && isValid;
        joinButton.interactable = !isEmpty && isValid;
    }
    private void SetRoomName(string roomName)
    {
        RPMPlayerData.SetData(RPMPlayerData.Keys.roomNameKey, roomName);
        CheckRoomName();
    }

    private void OnDisable()
    {
        roomNameInputField.onValueChanged.RemoveListener(SetRoomName);
    }
    private void OnDestroy()
    {
        roomNameInputField.onValueChanged.RemoveListener(SetRoomName);
    }
}
