using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenUIOnEnterCollider : MonoBehaviour
{
    public GameObject colliderUIPanel;

    private bool isPlayerInsideCollider = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideCollider = true;
            colliderUIPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideCollider = false;
            colliderUIPanel.SetActive(false);
            // spaceShooterUIPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInsideCollider && Input.GetKeyDown(KeyCode.P))
        {
            OpenSpaceShooterUI();
        }
    }

    private void OpenSpaceShooterUI()
    {
        LevelManager.Intance.SpaceShooterScene.Invoke();
        // spaceShooterUIPanel.SetActive(true);
    }
}
