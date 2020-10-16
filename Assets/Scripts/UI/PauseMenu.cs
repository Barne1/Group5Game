using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool visible = false;
    private float previousTimeScale = 1f;

    public void Toggle()
    {
        visible = !visible;

        if (visible)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = previousTimeScale;
        }
        
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }
    }
    
    public void GoToMenu()
    {
        Time.timeScale = previousTimeScale;
        const string mainMenuName = "sce_MainMenu";
        SceneManager.LoadScene(mainMenuName);
    }
}
