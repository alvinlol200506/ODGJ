using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseCanvas;
    
    public void Restart(String gameplay)
    {
        SceneManager.LoadScene(gameplay);
    }

    public void OpenPause()
    {
        
        pauseCanvas.alpha = 1f;
        pauseCanvas.blocksRaycasts = true;
    }

    public void ClosePause()
    {
        
        pauseCanvas.alpha = 0f;
        pauseCanvas.blocksRaycasts = false;
    }

    public void GoToMainMenu(String mainmenu)
    {
        SceneManager.LoadScene(mainmenu);
    }
}
