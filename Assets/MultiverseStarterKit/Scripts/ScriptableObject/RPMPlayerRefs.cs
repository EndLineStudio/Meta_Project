using UnityEngine;

[CreateAssetMenu(fileName = "RPMPlayerRefs", menuName = "Fusion KCC x ReadyPlayerMe/RPM Player Refs")]
public class RPMPlayerRefs : ScriptableObject
{
    public string playerNameKey = "playername";
    public int playerNameLength = 8;
    public string genderKey = "gender";
    public string localAvatarKey = "localavatar";
    public string runtimeAvatarKey = "runtimeavatar";
    public int runtimeAvatarUrlLength = 58;
    public string openUrl = "https://readyplayer.me/hub";
    public string validationStart = "https://";
    public string validationContain = "models.readyplayer.me/";
    public string validationEnd = ".glb";
    public string roomNameKey = "roomname";
    public int roomNameLength = 4;
    public string regionNameKey = "regionname";
}
