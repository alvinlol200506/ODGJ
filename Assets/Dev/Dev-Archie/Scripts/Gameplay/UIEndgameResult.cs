using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace ODGJ.Gameplay
{
    public class UIEndgameResult : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI txtResultDetails; // Buat "Jumlah Pasien Berhasil & Gagal"
        [SerializeField] private TextMeshProUGUI txtTotalGold;     // Buat "Total Pendapatan: XX"
        [SerializeField] private Button btnOke;
        
        [Header("Scene Settings")]
        [SerializeField] private string padepokanSceneName = "Padepokan";

        private void Awake() {
            btnOke.onClick.AddListener(BackToPadepokan);
            HideCanvas();
        }

        public void ShowResult(int success, int fail, int gold) {
            // Format tulisan narik dari data rekapan
            txtResultDetails.text = $"Jumlah Pasien Berhasil: {success}\nJumlah Pasien Gagal: {fail}";
            
            string sign = gold >= 0 ? "+" : "";
            txtTotalGold.text = $"Total Pendapatan: {sign}{gold}";
            
            // Munculin layar endgame
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        
        private void BackToPadepokan() {
            // Balik ke lobby
            SceneManager.LoadScene(padepokanSceneName);
        }

        private void HideCanvas() {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}