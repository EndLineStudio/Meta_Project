using UnityEngine;

public class keyboardmanager : MonoBehaviour
{
    public GameObject uiPanel;
    public KeyCode loadKey = KeyCode.P;

    private bool isUIVisible = false;

    private void Start()
    {
        uiPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(loadKey))
        {
            if (!isUIVisible)
            {
                LoadUI();
            }
            else
            {
                UnloadUI();
            }
        }
    }

    private void LoadUI()
    {
        uiPanel.SetActive(true);
        isUIVisible = true;
    }

    private void UnloadUI()
    {
        uiPanel.SetActive(false);
        isUIVisible = false;
    }
}
