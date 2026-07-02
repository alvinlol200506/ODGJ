using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Test harness untuk core loop — TANPA Canvas. Pakai tombol IMGUI (OnGUI)
    /// jadi langsung bisa diklik di Game view tanpa setup UI apa pun.
    /// Semua proses dicetak lewat Debug.Log.
    ///
    /// Cara pakai:
    ///   1. Buat beberapa asset Ghost Data & Request Data (klik kanan > Create > ODGJ).
    ///   2. Drop script ini ke sebuah GameObject kosong di scene.
    ///   3. Isi array 'ghosts' & 'requests' di Inspector.
    ///   4. Play, lalu klik tombol di kiri-atas Game view.
    /// </summary>
    public class DispatchDebugTester : MonoBehaviour
    {
        [Header("Data untuk dites")]
        [SerializeField] private GhostData[] ghosts;
        [SerializeField] private RequestData[] requests;

        [Header("Pilihan aktif (index)")]
        [SerializeField] private int selectedGhost;
        [SerializeField] private int selectedRequest;

        private void OnEnable()
        {
            DispatchEvents.OnDispatchStarted += HandleStarted;
            DispatchEvents.OnDispatchProgress += HandleProgress;
            DispatchEvents.OnDispatchFinished += HandleFinished;
        }

        private void OnDisable()
        {
            DispatchEvents.OnDispatchStarted -= HandleStarted;
            DispatchEvents.OnDispatchProgress -= HandleProgress;
            DispatchEvents.OnDispatchFinished -= HandleFinished;
        }

        // ---------- Eksekusi ----------

        /// <summary>Kirim hantu & misi yang sedang dipilih. Bisa juga dipanggil dari tombol UI nanti.</summary>
        public void DispatchSelected()
        {
            if (!HasValidSelection()) return;

            GhostData ghost = ghosts[selectedGhost];
            RequestData request = requests[selectedRequest];

            float preview = DispatchManager.Instance.CalculateSuccessChance(ghost, request);
            Debug.Log($"[Tester] Mengirim '{ghost.ghostName}' untuk '{request.clientName}'. " +
                      $"Preview peluang sukses: {preview:P0}");

            DispatchManager.Instance.TryStartDispatch(ghost, request);
        }

        private bool HasValidSelection()
        {
            if (ghosts == null || ghosts.Length == 0 || requests == null || requests.Length == 0)
            {
                Debug.LogError("[Tester] Isi dulu array 'ghosts' & 'requests' di Inspector.");
                return false;
            }
            selectedGhost = Mathf.Clamp(selectedGhost, 0, ghosts.Length - 1);
            selectedRequest = Mathf.Clamp(selectedRequest, 0, requests.Length - 1);
            return true;
        }

        // ---------- Event handlers (cetak proses) ----------

        private void HandleStarted(GhostData ghost, RequestData request)
            => Debug.Log($"[Event] START: {ghost.ghostName} dikirim ke {request.clientName} " +
                         $"(durasi {request.dispatchDuration}s)");

        private float _lastLoggedProgress;
        private void HandleProgress(float progress, GhostData ghost, RequestData request)
        {
            // Log tiap kelipatan 25% biar console tidak banjir.
            if (progress - _lastLoggedProgress >= 0.25f || progress >= 1f)
            {
                _lastLoggedProgress = progress;
                Debug.Log($"[Event] PROGRESS {ghost.ghostName}: {progress:P0}");
            }
        }

        private void HandleFinished(DispatchReport report)
        {
            _lastLoggedProgress = 0f;
            string result = report.IsSuccess ? "<color=green>SUKSES</color>" : "<color=red>GAGAL</color>";
            Debug.Log($"[Event] FINISHED: {report.Ghost.ghostName} -> {report.Request.clientName} = {result} " +
                      $"(peluang {report.SuccessChance:P0}, uang {report.MoneyDelta:+0;-0})");
        }

        // ---------- Dummy buttons (IMGUI, no Canvas) ----------

        private void OnGUI()
        {
            const int w = 320;
            GUILayout.BeginArea(new Rect(10, 10, w, 400), GUI.skin.box);
            GUILayout.Label("<b>DISPATCH DEBUG TESTER</b>");

            if (ghosts != null && ghosts.Length > 0)
            {
                GUILayout.Label($"Hantu: {ghosts[Mathf.Clamp(selectedGhost, 0, ghosts.Length - 1)].ghostName}");
                if (GUILayout.Button("Ganti Hantu >>"))
                    selectedGhost = (selectedGhost + 1) % ghosts.Length;
            }

            if (requests != null && requests.Length > 0)
            {
                GUILayout.Label($"Misi: {requests[Mathf.Clamp(selectedRequest, 0, requests.Length - 1)].clientName}");
                if (GUILayout.Button("Ganti Misi >>"))
                    selectedRequest = (selectedRequest + 1) % requests.Length;
            }

            GUILayout.Space(8);
            if (GUILayout.Button("KIRIM HANTU (Dispatch)"))
                DispatchSelected();

            GUILayout.Space(8);
            GUILayout.Label($"Dispatch aktif: {DispatchManager.Instance.ActiveDispatchCount}");

            // ---------- Save / Unlock ----------
            GUILayout.Space(10);
            GUILayout.Label("<b>SAVE / UNLOCK</b>");

            PlayerWallet wallet = PlayerWallet.Instance;
            GUILayout.Label(wallet != null
                ? $"Uang: {wallet.Money}"
                : "Uang: (taruh PlayerWallet di scene)");

            GhostUnlockManager unlockMgr = GhostUnlockManager.Instance;
            if (unlockMgr != null && ghosts != null && ghosts.Length > 0)
            {
                GhostData g = ghosts[Mathf.Clamp(selectedGhost, 0, ghosts.Length - 1)];
                string status = unlockMgr.IsUnlocked(g) ? "TERBUKA" : "terkunci";
                GUILayout.Label($"'{g.ghostName}': {status}  |  Total unlock: {unlockMgr.UnlockedCount}");

                if (GUILayout.Button("Unlock Hantu Terpilih"))
                    unlockMgr.Unlock(g);
            }
            else
            {
                GUILayout.Label("(taruh GhostUnlockManager di scene buat tes unlock)");
            }

            if (GUILayout.Button("RESET SAVE (hapus uang & unlock)"))
            {
                SaveSystem.ClearAll();
                Debug.Log("[Tester] Save di-reset. Play ulang untuk lihat efeknya.");
            }

            GUILayout.EndArea();
        }
    }
}
