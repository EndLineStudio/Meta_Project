using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Intance;

    public System.Action MainScene;
    public System.Action SpaceShooterScene;

    private void Awake()
    {
        if (Intance == null)
            Intance = this;

        MainScene += LoadMainGame;
        SpaceShooterScene += LoadSpaceShooterGame;

    }

    public GameObject MainGameLevel;
    public GameObject SpaceShooterLevel;
    public GameObject UICamera;

    private void LoadMainGame()
    {
        MainGameLevel.SetActive(true);
        SpaceShooterLevel.SetActive(false);
    }

    private void LoadSpaceShooterGame()
    {
        UICamera.SetActive(false);
        MainGameLevel.SetActive(false);
        SpaceShooterLevel.SetActive(true);
    }
}
