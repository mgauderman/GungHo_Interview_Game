using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    public void OnPlayButtonPressed()
    {
        gameManager.StartGame();
    }
    public void OnQuitButtonPressed()
    {
        gameManager.QuitGame();
    }
}
