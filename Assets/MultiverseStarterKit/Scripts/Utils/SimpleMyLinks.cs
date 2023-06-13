using UnityEngine;
using UnityEngine.UI;

public class SimpleMyLinks : MonoBehaviour
{
    [SerializeField]
    private Button youtube;
    [SerializeField]
    private string ytLinks;

    [SerializeField]
    private Button twitter;
    [SerializeField]
    private string twLinks;

    [SerializeField]
    private Button itch;
    [SerializeField]
    private string itchLinks;

    private void OnEnable()
    {
        youtube.onClick.AddListener(OnYoutubeButtonClicked);
        twitter.onClick.AddListener(OnTwitterButtonClicked);
        itch.onClick.AddListener(OnItchButtonClicked);
    }
    private void OnYoutubeButtonClicked()
    {
        SimpleOpenBrowser.OpenBrowser(ytLinks);
    }
    private void OnTwitterButtonClicked()
    {
        SimpleOpenBrowser.OpenBrowser(twLinks);
    }
    private void OnItchButtonClicked()
    {
        SimpleOpenBrowser.OpenBrowser(itchLinks);
    }
    private void OnDisable()
    {
        youtube.onClick.RemoveListener(OnYoutubeButtonClicked);
        twitter.onClick.RemoveListener(OnTwitterButtonClicked);
        itch.onClick.RemoveListener(OnItchButtonClicked);
    }
}
