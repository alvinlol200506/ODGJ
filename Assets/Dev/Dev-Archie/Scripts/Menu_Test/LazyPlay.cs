using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LazyPlay : MonoBehaviour
{
    public void GoToGameplay(String scene)
    {
        ODGJ.Dispatch.AudioManager.Instance.PlaySFX(ODGJ.Dispatch.AudioManager.Instance.buttonClick);
        SceneManager.LoadScene(scene);
    }
    
    public void hapusData()
    {
        ODGJ.Dispatch.AudioManager.Instance.PlaySFX(ODGJ.Dispatch.AudioManager.Instance.buttonClick);
        PlayerPrefs.DeleteAll();
        Debug.Log("Data PlayerPrefs telah dihapus.");
    }
}
