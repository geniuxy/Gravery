using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] private UI_DarkScreen darkScreen;

    private void Start()
    {
        continueButton.SetActive(SaveManager.instance.IsArchiveExist());
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadScreenWithFadeFX(1.75f));
    }

    public void NewGame()
    {
        SaveManager.instance.DeleteArchive();
        StartCoroutine(LoadScreenWithFadeFX(1.75f));
    }

    public void ExitGame()
    {
        Debug.Log("123");
        // Application.Quit();
    }

    private IEnumerator LoadScreenWithFadeFX(float _delay)
    {
        darkScreen.FadeOut();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(sceneName);
    }
}