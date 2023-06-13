using UnityEngine;
using Fusion;

public partial class SimpleNetworkStartBridge : SimpleRPMMenu
{
    [SerializeField]
    private NetworkDebugStart starter;
    public void StartHost()
    {
        starter.DefaultRoomName = PlayerPrefs.GetString(RPMPlayerData.Keys.roomNameKey);

        if (string.IsNullOrWhiteSpace(starter.DefaultRoomName))
        {
            starter.DefaultRoomName = SimpleRandomInputField.GenerateRandomText(RPMPlayerData.Keys.roomNameLength);
            PlayerPrefs.SetString(RPMPlayerData.Keys.roomNameKey, starter.DefaultRoomName);
        }

        starter.StartHost();
    }
    public void StartClient()
    {
        starter.StartClient();
    }
    public void Shutdown()
    {
        foreach (var runner in NetworkRunner.Instances)
        {
            runner.Shutdown();
        }
    }
}
