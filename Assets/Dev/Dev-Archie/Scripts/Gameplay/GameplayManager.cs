using UnityEngine;
using TMPro;
using System.Collections;
using ODGJ.Dispatch;

namespace ODGJ.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }
        
        [Header("Settings")]
        [Tooltip("Durasi satu malam (dalam detik). Contoh: 180 = 3 Menit")]
        [SerializeField] private float sessionDuration = 180f; 
        
        [Header("Top Right UI References")]
        [SerializeField] private TextMeshProUGUI txtNight;
        [SerializeField] private TextMeshProUGUI txtTimer;
        
        [Header("Endgame Canvas")]
        [SerializeField] private UIEndgameResult endgameUI;
        
        // Data Rekapan Session Ini
        private int _successCount = 0;
        private int _failCount = 0;
        private int _sessionGold = 0;
        
        private float _timeLeft;
        public bool IsGameOver { get; private set; }

        private void Awake() {
            Instance = this;
            _timeLeft = sessionDuration;
            IsGameOver = false;
            
            // Ambil data Hari ke-X dari PlayerPrefs
            int currentNight = PlayerPrefs.GetInt("CurrentNight", 1);
            if(txtNight != null) txtNight.text = currentNight.ToString();
        }

        private void OnEnable() {
            // Numpang dengerin event kalau hantu kelar bertugas
            DispatchEvents.OnDispatchFinished += RecordDispatch;
        }

        private void OnDisable() {
            DispatchEvents.OnDispatchFinished -= RecordDispatch;
        }

        private void Update() {
            if (IsGameOver) return; // Kalau udah beres, stop countdown
            
            if (_timeLeft > 0) {
                _timeLeft -= Time.deltaTime;
                UpdateTimerUI();
                
                if (_timeLeft <= 0) {
                    EndGame();
                }
            }
        }
        
        private void UpdateTimerUI() {
            int minutes = Mathf.FloorToInt(_timeLeft / 60);
            int seconds = Mathf.FloorToInt(_timeLeft % 60);
            txtTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        private void RecordDispatch(DispatchReport report) {
            // Catat hasil cuma buat session ini aja (gak nyampur ke total duit)
            if (report.IsSuccess) _successCount++;
            else _failCount++;
            
            _sessionGold += report.MoneyDelta;
        }

        private void EndGame() {
            IsGameOver = true;

             txtTimer.text = "00:00";
            
            // 1. Paksa tutup Canvas Dispatcher kalau player lagi baca misi
            if (UIDispatchController.Instance != null) {
                UIDispatchController.Instance.ClosePanel(); 
            }
            
            // 2. Paksa tutup Canvas Result (dan bersihin antrean biar ga tiba2 nongol)
            if (UIResultPopup.Instance != null) {
                UIResultPopup.Instance.ForceClose(); 
            }

            // 3. Hancurkan sisa misi di layar (Opsional, biar bersih)
            // Bisa lu kembangin sendiri nanti kalau mau
            
            // 4. Tampilkan Layar Endgame
            endgameUI.ShowResult(_successCount, _failCount, _sessionGold);
        }
    }
}