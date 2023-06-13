using UnityEngine;
using Fusion;
using Fusion.KCC;

[OrderBefore(typeof(KCC))]
public partial class SimplePlayerData : SimpleRPMPlayer
{
    #region Networked
    [Networked(OnChanged = nameof(PlayerNameChanged))]
    public NetworkString<_8> NetworkPlayerName { get; set; }

    [Networked(OnChanged = nameof(RuntimeAvatarChanged))]
    public NetworkString<_64> NetworkAvatarUrl { get; set; }

    [Networked(OnChanged = nameof(LocalAvatarChanged))]
    [HideInInspector]
    public int GenderIndex { get; set; } = 0;

    [Networked(OnChanged = nameof(RoomNameChanged))]
    public NetworkString<_4> NetworkRoomName { get; set; }

    [Networked(OnChanged = nameof(RegionNameChanged))]
    public NetworkString<_4> NetworkRegionName { get; set; }

    [Networked(OnChanged = nameof(UserGenderChanged))]
    [HideInInspector]
    public int UserGenderIndex { get; set; } = 0;
    #endregion

    public RPMPlayerDataCache PlayerCache => playerCache;
    private RPMPlayerDataCache playerCache;

    public RPMEnums.LoadedAvatar loadedAvatar = RPMEnums.LoadedAvatar.NoAvatar;
    public RPMEnums.UserGender userGender = RPMEnums.UserGender.NoAvatar;

    private bool onSetRuntimeAvatar;
    public bool OnSetRuntimeAvatar => onSetRuntimeAvatar;

    private bool onSetLocalAvatar;
    public bool OnSetLocalAvatar => onSetLocalAvatar;

    private int localAvatar;
    private string runtimeAvatarUrl;

    public override void Spawned()
    {
        name = Object.InputAuthority.ToString();

        if (Object.HasInputAuthority == true)
        {
            playerCache = new RPMPlayerDataCache()
            {
                playerName = PlayerPrefs.GetString(RPMPlayerData.Keys.playerNameKey),
                runtimeAvatarUrl = PlayerPrefs.GetString(RPMPlayerData.Keys.runtimeAvatarKey),
                gender = PlayerPrefs.GetInt(RPMPlayerData.Keys.genderKey, GenderIndex),
                localAvatar = PlayerPrefs.GetInt(RPMPlayerData.Keys.localAvatarKey, GenderIndex),
                roomName = PlayerPrefs.GetString(RPMPlayerData.Keys.roomNameKey),
                regionName = PlayerPrefs.GetString(RPMPlayerData.Keys.regionNameKey)
            };

            Rpc_SetInitialPlayerData(PlayerCache.playerName, PlayerCache.gender, PlayerCache.roomName, PlayerCache.regionName);

            localAvatar = PlayerCache.localAvatar;
            onSetLocalAvatar = localAvatar > 0;

            runtimeAvatarUrl = PlayerCache.runtimeAvatarUrl;
            onSetRuntimeAvatar = !string.IsNullOrEmpty(runtimeAvatarUrl);
        }
    }
    public override void Despawned(NetworkRunner runner, bool hasState) { }
    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority == true)
        {
            if (onSetLocalAvatar)
            {
                Rpc_SetLocalAvatar(localAvatar);
            }

            if (onSetRuntimeAvatar)
            {
                Rpc_SetRuntimeAvatar(runtimeAvatarUrl);
            }
        }
    }

    public void OnRuntimeAvatarSet(string newAvatarUrl)
    {
        PlayerCache.runtimeAvatarUrl = newAvatarUrl;
        runtimeAvatarUrl = PlayerCache.runtimeAvatarUrl;
        onSetRuntimeAvatar = true;
    }
    public void OnLocalAvatarSet(int newLocalAvatar)
    {
        PlayerCache.localAvatar = newLocalAvatar;
        localAvatar = PlayerCache.localAvatar;
        onSetLocalAvatar = true;
    }

    #region INITIAL RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetInitialPlayerData(string playername, int usergender, string roomname, string regionname)
    {
        NetworkPlayerName = playername;
        NetworkRoomName = roomname;
        NetworkRegionName = regionname;

        UserGenderIndex = usergender;
        userGender = (RPMEnums.UserGender)UserGenderIndex;
    }
    #endregion

    #region PLAYER NAME RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetPlayerName(string playername)
    {
        NetworkPlayerName = playername;
    }
    static void PlayerNameChanged(Changed<SimplePlayerData> changed) => changed.Behaviour.PlayerUI.SetPlayerName(changed.Behaviour.NetworkPlayerName.Value);
    #endregion

    #region ROOM NAME RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetRoomName(string roomname)
    {
        NetworkRoomName = roomname;
    }
    static void RoomNameChanged(Changed<SimplePlayerData> changed) => changed.Behaviour.PlayerUI.SetRoomName(changed.Behaviour.NetworkRoomName.Value);
    #endregion

    #region REGION NAME RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetRegionName(string roomname)
    {
        NetworkRegionName = roomname;
    }
    static void RegionNameChanged(Changed<SimplePlayerData> changed) => changed.Behaviour.PlayerUI.SetRegionName(changed.Behaviour.NetworkRegionName.Value);
    #endregion

    #region USER GENDER RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetUserGender(int genderindex)
    {
        UserGenderIndex = genderindex;
        userGender = (RPMEnums.UserGender)UserGenderIndex;
    }
    static void UserGenderChanged(Changed<SimplePlayerData> changed) => changed.Behaviour.PlayerAvatarLoader.SetUserGender(changed.Behaviour.UserGenderIndex);
    #endregion

    #region LOCAL AVATAR RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetLocalAvatar(int genderindex)
    {
        GenderIndex = genderindex;
        loadedAvatar = (RPMEnums.LoadedAvatar)GenderIndex;

        onSetLocalAvatar = false;
    }
    static void LocalAvatarChanged(Changed<SimplePlayerData> changed) => changed.Behaviour.PlayerAvatarLoader.SetLocalAvatar(changed.Behaviour.GenderIndex);
    #endregion

    #region RUNTIME AVATAR RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void Rpc_SetRuntimeAvatar(string avatarurl)
    {
        NetworkAvatarUrl = avatarurl;

        onSetRuntimeAvatar = false;
    }
    static void RuntimeAvatarChanged(Changed<SimplePlayerData> changed) => changed.Behaviour.PlayerAvatarLoader.SetRuntimeAvatar(changed.Behaviour.NetworkAvatarUrl.Value);
    #endregion
}
