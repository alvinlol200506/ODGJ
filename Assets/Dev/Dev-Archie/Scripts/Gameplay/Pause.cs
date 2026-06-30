using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseCanvas;
    
    public void Restart(String gameplay)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplay);
    }

    public void OpenPause()
    {
        Time.timeScale = 0f;
        pauseCanvas.alpha = 1f;
        pauseCanvas.blocksRaycasts = true;
    }

    public void ClosePause()
    {
        Time.timeScale = 1f;
        pauseCanvas.alpha = 0f;
        pauseCanvas.blocksRaycasts = false;
    }

    public void GoToMainMenu(String mainmenu)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainmenu);
    }
}
