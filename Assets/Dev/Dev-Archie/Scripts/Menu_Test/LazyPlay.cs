using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LazyPlay : MonoBehaviour
{
    public void GoToGameplay(String gameplay)
    {
        SceneManager.LoadScene(gameplay);
    }
}
