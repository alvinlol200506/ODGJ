using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseCanvas;
    
    public void Restart(String gameplay)
    {
        Time.timeScale = 1f;
        if (ODGJ.Dispatch.AudioManager.Instance != null)
        {
            ODGJ.Dispatch.AudioManager.Instance.PlaySFX(ODGJ.Dispatch.AudioManager.Instance.buttonClick);
        }
        else
        {
            Debug.LogWarning("[Pause] AudioManager NOT FOUND!");
        }
        SceneManager.LoadScene(gameplay);
    }

    public void OpenPause()
    {
        if (ODGJ.Dispatch.AudioManager.Instance != null)
        {
            ODGJ.Dispatch.AudioManager.Instance.PlaySFX(ODGJ.Dispatch.AudioManager.Instance.buttonClick);
        }
        else
        {
            Debug.LogWarning("[Pause] AudioManager NOT FOUND!");
        }
        Time.timeScale = 0f;
        pauseCanvas.alpha = 1f;
        pauseCanvas.blocksRaycasts = true;
    }

    public void ClosePause()
    {
        if (ODGJ.Dispatch.AudioManager.Instance != null)
        {
            ODGJ.Dispatch.AudioManager.Instance.PlaySFX(ODGJ.Dispatch.AudioManager.Instance.buttonClick);
        }
        else
        {
            Debug.LogWarning("[Pause] AudioManager NOT FOUND!");
        }
        Time.timeScale = 1f;
        pauseCanvas.alpha = 0f;
        pauseCanvas.blocksRaycasts = false;
    }

    public void GoToMainMenu(String mainmenu)
    {
        if (ODGJ.Dispatch.AudioManager.Instance != null)
        {
            ODGJ.Dispatch.AudioManager.Instance.PlaySFX(ODGJ.Dispatch.AudioManager.Instance.buttonClick);
        }
        else
        {
            Debug.LogWarning("[Pause] AudioManager NOT FOUND!");
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainmenu);
    }
}
