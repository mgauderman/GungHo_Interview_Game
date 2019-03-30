using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string startLevelName;

    public void StartGame()
    {
        SceneManager.LoadScene(startLevelName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
