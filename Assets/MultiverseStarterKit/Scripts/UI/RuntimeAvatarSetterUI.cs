using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public partial class RuntimeAvatarSetterUI : SimpleRPMMenu
{
    [SerializeField]
    private TMP_InputField avatarUrlField;
    [SerializeField]
    private Button createButton;
    [SerializeField]
    private TextMeshProUGUI createButtonText;
    [SerializeField]
    private Button nextButton;

    private bool hasOpenBrowser;

    private void OnEnable()
    {
        string latestAvatarUrl = RPMPlayerData.GetData(RPMPlayerData.Keys.runtimeAvatarKey);
        avatarUrlField.SetTextWithoutNotify(latestAvatarUrl);
        CheckAvatarUrl();

        avatarUrlField.interactable = false;
    }
    private void Start()
    {
        string avatarUrl = avatarUrlField.text;

        avatarUrlField.onValueChanged.AddListener(SetAvatarUrl);
        createButton.onClick.AddListener(OnCreateButtonClicked);

        SetAvatarUrl(avatarUrl);
    }
    private void LateUpdate()
    {
        SetPasteOrClearTextUI();
    }

    public void OnsetAvatarUrlWebView(string generatedurl)
    {
        avatarUrlField.SetTextWithoutNotify(generatedurl);
        SetAvatarUrl(generatedurl);
    }

    private void SetAvatarUrl(string avatarurl)
    {
        RPMPlayerData.SetData(RPMPlayerData.Keys.runtimeAvatarKey, avatarurl);
        CheckAvatarUrl();
    }
    private void CheckAvatarUrl()
    {
        (bool isEmpty, bool isValid) = RPMPlayerData.CheckInputFieldData(avatarUrlField, RPMPlayerData.Keys.runtimeAvatarUrlLength);

        string avatarUrl = avatarUrlField.text;
        bool isUrlValid = avatarUrl.StartsWith(RPMPlayerData.Keys.validationStart) && (avatarUrl.Contains(RPMPlayerData.Keys.validationContain) || avatarUrl.EndsWith(RPMPlayerData.Keys.validationEnd));

        nextButton.interactable = isUrlValid && (!isEmpty && isValid);
    }

    public void OnCreateButtonClicked()
    {
        SetPasteOrClearAction();
    }
    private void SetPasteOrClearTextUI()
    {
        string avatarUrl = avatarUrlField.text;

        if (string.IsNullOrEmpty(avatarUrl))
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            createButtonText.text = RPMConstant.CREATE;
#else
            if (hasOpenBrowser)
            {
                createButtonText.text = RPMConstant.PASTE;
            }
            else
            {
                createButtonText.text = RPMConstant.CREATE;
            }
#endif
        }
        else
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            createButtonText.text = RPMConstant.CLEAR;
#else
            createButtonText.text = RPMConstant.CLEAR;
#endif
        }
    }
    private void SetPasteOrClearAction()
    {
        string avatarUrl = avatarUrlField.text;

        if (string.IsNullOrEmpty(avatarUrl))
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            WebInterface.SetIFrameVisibility(true); // get avatar url from web view
#else
            if (hasOpenBrowser)
            {
                StartCoroutine(PasteCoroutine());
            }
            else
            {
                StartCoroutine(OpenBrowserCoroutine());
            }
#endif
        }
        else
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            avatarUrlField.text = string.Empty;
#else
            avatarUrlField.text = string.Empty;
#endif
        }
    }

    private IEnumerator OpenBrowserCoroutine()
    {
        if (hasOpenBrowser)
        {
            yield break;
        }

        SimpleOpenBrowser.OpenBrowser(RPMPlayerData.Keys.openUrl);
        avatarUrlField.text = string.Empty;
        SetAvatarUrl(string.Empty);

        yield return new WaitForSeconds(1);

        hasOpenBrowser = true;
    }
    private IEnumerator PasteCoroutine()
    {
        if (!hasOpenBrowser)
        {
            yield break;
        }

        string clipboard = SimpleGetClipboard.GetClipboardText();
        avatarUrlField.SetTextWithoutNotify(clipboard);
        SetAvatarUrl(clipboard);

        yield return new WaitForSeconds(1);

        hasOpenBrowser = false;
    }

    private void OnDisable()
    {
        avatarUrlField.onValueChanged.RemoveListener(SetAvatarUrl);
        createButton.onClick.RemoveListener(OnCreateButtonClicked);
    }
    private void OnDestroy()
    {
        avatarUrlField.onValueChanged.RemoveListener(SetAvatarUrl);
        createButton.onClick.RemoveListener(OnCreateButtonClicked);
    }
}
