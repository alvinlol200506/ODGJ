using UnityEngine;
using UnityEngine.SceneManagement;

namespace ODGJ.Lobby
{
    public class LobbySceneController : MonoBehaviour
    {
        public void MulaiMalamBerikutnya()
        {
            // Ambil data hari saat ini, tambahin 1, terus disave!
            int currentNight = PlayerPrefs.GetInt("CurrentNight", 0);
            PlayerPrefs.SetInt("CurrentNight", currentNight + 1);
            PlayerPrefs.Save();

            // Load ke scene gameplay (sesuaikan nama scenenya)
            SceneManager.LoadScene("Gameplay");
        }
    }
}