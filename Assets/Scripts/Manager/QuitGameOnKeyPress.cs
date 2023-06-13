using UnityEngine;

public class QuitGameOnKeyPress : MonoBehaviour
{
    public GameObject uiPanel;
    public KeyCode quitKey = KeyCode.Q;

    private void Update()
    {
        if (Input.GetKeyDown(quitKey))
        {
            QuitGame();
        }
    }

    private void QuitGame()
    {
        uiPanel.SetActive(false);
    }
}
