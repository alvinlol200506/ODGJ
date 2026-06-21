using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Manager sentral siklus misi (singleton). Tanggung jawab:
    ///   1. Terima hantu + misi yang dipilih, validasi.
    ///   2. Jalankan delay pengiriman (Coroutine).
    ///   3. Calculation Engine: hitung % sukses dari stat hantu vs requirement misi (+ RNG).
    ///   4. Broadcast event start/progress/finished (TIDAK pernah sentuh UI langsung).
    ///
    /// Mendukung beberapa dispatch sekaligus (tiap hantu jalan di coroutine sendiri),
    /// cocok untuk genre dispatch/management. Satu hantu tidak bisa dikirim ganda.
    /// </summary>
    public class DispatchManager : MonoBehaviour
    {
        private static DispatchManager _instance;

        /// <summary>Akses global. Auto-create kalau belum ada di scene (enak buat testing).</summary>
        public static DispatchManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<DispatchManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("[DispatchManager]");
                        _instance = go.AddComponent<DispatchManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Tuning Calculation Engine")]
        [Tooltip("Batas bawah/atas peluang sukses. Selalu sisakan elemen kejutan (RNG).")]
        [Range(0f, 0.5f)] public float minSuccessChance = 0.05f;
        [Range(0.5f, 1f)] public float maxSuccessChance = 0.95f;

        [Tooltip("Peluang dasar kalau misi tidak punya requirement sama sekali.")]
        [Range(0f, 1f)] public float noRequirementChance = 0.9f;

        // Hantu yang sedang bertugas — biar tidak dikirim dobel.
        private readonly HashSet<GhostData> _busyGhosts = new HashSet<GhostData>();

        public int ActiveDispatchCount { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public bool IsGhostBusy(GhostData ghost) => ghost != null && _busyGhosts.Contains(ghost);

        /// <summary>
        /// Coba mulai dispatch. Return false kalau input invalid atau hantu masih sibuk.
        /// </summary>
        public bool TryStartDispatch(GhostData ghost, RequestData request)
        {
            if (ghost == null || request == null)
            {
                Debug.LogWarning("[Dispatch] Ghost/Request null — dibatalkan.");
                return false;
            }

            if (IsGhostBusy(ghost))
            {
                Debug.LogWarning($"[Dispatch] {ghost.ghostName} masih bertugas — tidak bisa dikirim lagi.");
                return false;
            }

            _busyGhosts.Add(ghost);
            ActiveDispatchCount++;

            // Observer: kasih tahu dunia bahwa pengiriman dimulai.
            DispatchEvents.RaiseStarted(ghost, request);

            StartCoroutine(DispatchRoutine(ghost, request));
            return true;
        }

        /// <summary>Simulasi waktu kirim hantu lalu resolve hasil.</summary>
        private IEnumerator DispatchRoutine(GhostData ghost, RequestData request)
        {
            float duration = Mathf.Max(0.1f, request.dispatchDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                DispatchEvents.RaiseProgress(Mathf.Clamp01(elapsed / duration), ghost, request);
                yield return null;
            }

            // --- Resolve hasil ---
            float chance = CalculateSuccessChance(ghost, request);
            bool success = Random.value <= chance;
            int moneyDelta = success ? request.rewardMoney : -request.penaltyMoney;

            _busyGhosts.Remove(ghost);
            ActiveDispatchCount--;

            var report = new DispatchReport(ghost, request, chance, success, moneyDelta);
            DispatchEvents.RaiseFinished(report);
        }

        /// <summary>
        /// CALCULATION ENGINE. Bandingkan stat hantu vs tiap requirement misi.
        /// Tiap requirement dinilai sebagai rasio ghostStat / requiredValue (di-clamp 0..1),
        /// lalu dirata-rata jadi peluang sukses. Hasil di-clamp ke [min, max] biar selalu ada RNG.
        /// Fungsi ini PUBLIC supaya UI bisa nampilin preview peluang sebelum pemain commit.
        /// </summary>
        public float CalculateSuccessChance(GhostData ghost, RequestData request)
        {
            if (ghost == null || request == null) return 0f;

            var reqs = request.requirements;
            if (reqs == null || reqs.Length == 0)
                return Mathf.Clamp(noRequirementChance, minSuccessChance, maxSuccessChance);

            float total = 0f;
            for (int i = 0; i < reqs.Length; i++)
            {
                int ghostValue = ghost.GetStat(reqs[i].stat);
                int required = Mathf.Max(1, reqs[i].requiredValue); // hindari bagi 0
                total += Mathf.Clamp01((float)ghostValue / required);
            }

            float average = total / reqs.Length;
            return Mathf.Clamp(average, minSuccessChance, maxSuccessChance);
        }
    }
}
