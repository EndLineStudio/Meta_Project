using UnityEngine;
using System.Collections;
using TMPro;

public partial class SimplePlayerUI : SimpleRPMPlayer
{
    [Header("Player Name UI")]
    [SerializeField]
    private Canvas playerNameCanvas;
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    [Header("HUD Elements")]
    [SerializeField]
    private Canvas playerUICanvas;
    [SerializeField]
    private GameObject loadPanel;
    [SerializeField]
    private GameObject completePanel;
    [SerializeField]
    private GameObject failedPanel;
    [SerializeField]
    private TextMeshProUGUI roomNameText;
    [SerializeField]
    private TextMeshProUGUI regionNameText;

    [Space]
    [Header("Avatar Load UI Text")]
    [SerializeField]
    private TextMeshProUGUI avatarLoadTextUI;
    [SerializeField]
    private string avatarLoadText = "";
    [SerializeField]
    private TextMeshProUGUI loadCompletedTextUI;
    [SerializeField]
    private string loadCompletedText = "";
    [SerializeField]
    private TextMeshProUGUI loadFailedTextUI;
    [SerializeField]
    private string loadFailedText = "";

    private string networkPlayerName;
    private string networkRoomName;
    private string networkRegionName;

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority == true)
        {
            SetAvatarLoadUIStatus(1);
        }
    }
    private void LateUpdate()
    {
        if (Object.HasInputAuthority == false)
        {
            playerNameCanvas.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void SetPlayerName(string playername)
    {
        if (!string.IsNullOrEmpty(playername))
        {
            networkPlayerName = playername;
            DisplayPlayerName();
        }
    }
    private void DisplayPlayerName()
    {
        playerNameText.text = networkPlayerName;

        if (Object.HasInputAuthority == true)
        {
            playerNameCanvas.gameObject.SetActive(false);
        }
    }

    public void SetRoomName(string roomname)
    {
        if (!string.IsNullOrEmpty(roomname))
        {
            networkRoomName = roomname;
            DisplayRoomName();
        }
    }
    private void DisplayRoomName()
    {
        roomNameText.text = networkRoomName;
    }

    public void SetRegionName(string regionname)
    {
        if (!string.IsNullOrEmpty(regionname))
        {
            networkRegionName = regionname;
            DisplayRegionName();
        }
    }
    private void DisplayRegionName()
    {
        regionNameText.text = networkRegionName;
    }

    public void SetAvatarLoadUIStatus(int state)
    {
        if (Object.HasInputAuthority == true)
        {
            switch (state)
            {
                case 1:
                    loadPanel.SetActive(true);
                    avatarLoadTextUI.text = avatarLoadText;
                    completePanel.SetActive(false);
                    failedPanel.SetActive(false);
                    break;

                case 2:
                    failedPanel.SetActive(false);
                    StartCoroutine(OnAvatarLoadStatus(state));
                    break;

                case 3:
                    completePanel.SetActive(false);
                    StartCoroutine(OnAvatarLoadStatus(state));
                    break;

                default:
                    // Handle any other cases or provide a default behavior
                    break;
            }
        }
    }
    private IEnumerator OnAvatarLoadStatus(int state)
    {
        yield return new WaitForSeconds(3);

        loadPanel.SetActive(false);

        if (Object.HasInputAuthority == true)
        {
            if (state == 2)
            {
                completePanel.SetActive(true);
                loadCompletedTextUI.text = loadCompletedText;
            }
            else if (state == 3)
            {
                failedPanel.SetActive(true);
                loadFailedTextUI.text = loadFailedText;
            }
            else
            {
                //
            }
        }
        
        yield return new WaitForSeconds(3);

        completePanel.SetActive(false);
        failedPanel.SetActive(false);
    }
}

