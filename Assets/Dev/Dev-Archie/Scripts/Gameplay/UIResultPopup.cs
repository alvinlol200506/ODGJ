using System.Collections.Generic; // Wajib ditambahin buat pakai Queue
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ODGJ.Dispatch
{
    public class UIResultPopup : MonoBehaviour
    {
        public static UIResultPopup Instance { get; private set; }

        [Header("Canvas & Groups")]
        [SerializeField] private CanvasGroup popupCanvas;

        [Header("UI Text References")]
        [SerializeField] private TextMeshProUGUI txtStatus;
        [SerializeField] private TextMeshProUGUI txtDetails;
        [SerializeField] private TextMeshProUGUI txtReward;

        [Header("Buttons")]
        [SerializeField] private Button btnOke;

        [Header("Styling Colors")]
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color failureColor = Color.red;

        // --- SISTEM ANTREAN (QUEUE) ---
        private Queue<DispatchReport> _reportQueue = new Queue<DispatchReport>();
        public bool _isShowing = false; // Flag buat ngecek popup lagi kebuka atau nggak

        private void Awake()
        {
            Instance = this;

            btnOke.onClick.AddListener(OnOkButtonClicked);
            ClosePopup();
        }

        private void OnEnable()
        {
            DispatchEvents.OnDispatchFinished += HandleDispatchFinished;
        }

        private void OnDisable()
        {
            DispatchEvents.OnDispatchFinished -= HandleDispatchFinished;
        }

        /// <summary>
        /// Nangkep data selesai, tapi jangan langsung ditampilin. Masukin ke antrean dulu.
        /// </summary>
        private void HandleDispatchFinished(DispatchReport report)
        {
            _reportQueue.Enqueue(report); // Tambahin report ke barisan antrean

            // Kalau UI lagi nganggur (gak nampilin apa-apa), langsung panggil data terdepan
            if (!_isShowing && (UIDispatchController.Instance == null || !UIDispatchController.Instance.IsOpen))
            {
                DisplayNextReport();
            }
        }

        public void TriggerQueue()
        {
            if (!_isShowing && _reportQueue.Count > 0)
            {
                DisplayNextReport();
            }
        }

        /// <summary>
        /// Mengambil data paling depan dari antrean dan menampilkannya di UI.
        /// </summary>
        private void DisplayNextReport()
        {
            // Amankan kalau tiba-tiba antrean kosong
            if (_reportQueue.Count == 0)
            {
                ClosePopup();
                return;
            }

            _isShowing = true;

            // Dequeue = Ambil data paling depan, sekaligus buang data itu dari barisan antrean
            DispatchReport report = _reportQueue.Dequeue();

            // Update isi teks UI
            if (report.IsSuccess)
            {
                txtStatus.text = "RITUAL SUKSES!";
                txtStatus.color = successColor;
                txtReward.text = $"+{report.Request.rewardMoney} Koin";
                txtReward.color = successColor;
                txtDetails.text = $"{report.Ghost.ghostName} berhasil menyelesaikan keluhan dari {report.Request.clientName}. Pasien auto puas!";
            }
            else
            {
                txtStatus.text = "RITUAL GAGAL!";
                txtStatus.color = failureColor;
                txtReward.text = $"-{report.Request.penaltyMoney} Koin";
                txtReward.color = failureColor;
                txtDetails.text = $"{report.Ghost.ghostName} blunder! Masalah {report.Request.clientName} malah makin runyam dan bikin geger warga.";
            }

            // Munculin Canvas
            popupCanvas.alpha = 1f;
            popupCanvas.blocksRaycasts = true;
            popupCanvas.interactable = true;
        }

        /// <summary>
        /// Dipanggil saat player klik tombol "Oke"
        /// </summary>
        private void OnOkButtonClicked()
        {
            // Cek apakah masih ada hantu lain yang juga udah beres ngerjain tugas
            if (_reportQueue.Count > 0)
            {
                // Langsung tampilin hasil hantu berikutnya
                DisplayNextReport();
            }
            else
            {
                // Kalau antrean udah bener-bener habis, baru tutup panelnya
                ClosePopup();
            }
        }

        private void ClosePopup()
        {
            _isShowing = false;
            popupCanvas.alpha = 0f;
            popupCanvas.blocksRaycasts = false;
            popupCanvas.interactable = false;
        }

        public void ForceClose()
        {
            _reportQueue.Clear(); // Hapus sisa antrean report biar ga muncul abis game over
            ClosePopup();
        }
    }
}