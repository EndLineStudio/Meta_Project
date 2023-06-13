using Fusion;

public class SimpleRPMPlayer : NetworkBehaviour
{
    public SimplePlayerData PlayerData => playerData;
    private SimplePlayerData playerData;

    public SimplePlayerUI PlayerUI => playerUI;
    private SimplePlayerUI playerUI;

    public SimpleAvatarLoader PlayerAvatarLoader => playerAvatarLoader;
    private SimpleAvatarLoader playerAvatarLoader;

    private void Awake()
    {
        playerData = GetComponentInChildren<SimplePlayerData>();
        playerUI = GetComponentInChildren<SimplePlayerUI>();
        playerAvatarLoader = GetComponentInChildren<SimpleAvatarLoader>();
    }
}
