using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LazyPlay : MonoBehaviour
{
    public void GoToGameplay(String scene)
    {
        SceneManager.LoadScene(scene);
    }
    
    public void hapusData()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Data PlayerPrefs telah dihapus.");
    }
}
