using UnityEngine;

public class OpenUIOnEnterCollider : MonoBehaviour
{
    public GameObject colliderUIPanel;
    public GameObject spaceShooterUIPanel;

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
            spaceShooterUIPanel.SetActive(false);
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
        colliderUIPanel.SetActive(false);
        spaceShooterUIPanel.SetActive(true);
    }
}
