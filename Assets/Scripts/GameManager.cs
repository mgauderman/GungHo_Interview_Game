using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string startLevelName = "Level_1";
    [SerializeField]
    private string nextLevelName = "Main_Menu";
    [SerializeField]
    private float transitionDelay = 3f;
    [SerializeField]
    private Image fadeImage;
    [SerializeField]
    private bool fadeIn;

    private bool levelHasEnded;

    void Start()
    {
        levelHasEnded = false;
        if (fadeIn)
        {
            StartCoroutine(FadeIn(transitionDelay));
        }
        else
        {
            fadeImage.enabled = false;
        }
    }

    public void StartGame()
    {
        if (!levelHasEnded)
        {
            levelHasEnded = true;
            StartCoroutine(FadeOut(transitionDelay));
            Invoke("LoadStartLevel", transitionDelay);
        }
    }

    public void CompleteLevel()
    {
        if (!levelHasEnded) 
        {
            levelHasEnded = true;
            StartCoroutine(FadeOut(transitionDelay));
            Invoke("LoadNextLevel", transitionDelay);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }

    private void LoadStartLevel()
    {
        SceneManager.LoadScene(startLevelName);
    }

    IEnumerator FadeOut(float fadeTime)
    {
        fadeImage.enabled = true;
        float r = fadeImage.color.r;
        float g = fadeImage.color.g;
        float b = fadeImage.color.b;
        fadeImage.color = new Color(r, g, b, 0f);
        while (fadeImage.color.a < 1f)
        {
            fadeImage.color = new Color(r, g, b, fadeImage.color.a + Time.deltaTime * (1 / fadeTime));
            yield return null;
        }
        fadeImage.color = new Color(r, g, b, 1f);
    }

    IEnumerator FadeIn(float fadeTime)
    {
        fadeImage.enabled = true;
        float r = fadeImage.color.r;
        float g = fadeImage.color.g;
        float b = fadeImage.color.b;
        fadeImage.color = new Color(r, g, b, 1f);
        while (fadeImage.color.a > 0f)
        {
            fadeImage.color = new Color(r, g, b, fadeImage.color.a - Time.deltaTime * (1 / fadeTime));
            yield return null;
        }
        fadeImage.color = new Color(r, g, b, 0f);
        fadeImage.enabled = false;
    }
}
