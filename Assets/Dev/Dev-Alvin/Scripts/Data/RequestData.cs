using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Satu syarat stat untuk sebuah orderan: stat apa & minimal berapa.
    /// Contoh: { stat = MAG, requiredValue = 7 } artinya butuh Magic minimal 7.
    /// </summary>
    [System.Serializable]
    public struct StatRequirement
    {
        public GhostStatType stat;
        [Range(1, 10)] public int requiredValue;
    }

    /// <summary>
    /// Data-driven detail orderan gaib dari klien.
    /// Menyimpan durasi dispatch + syarat stat yang dibutuhkan untuk sukses,
    /// plus reward/penalty untuk di-update ke resource pemain.
    /// </summary>
    [CreateAssetMenu(fileName = "Request_", menuName = "ODGJ/Request Data", order = 1)]
    public class RequestData : ScriptableObject
    {
        [Header("Identitas Orderan")]
        public string clientName = "Klien Misterius";

        [TextArea(2, 4)]
        public string requestDescription;

        [TextArea(2, 3)]
        [Tooltip("Kalimat petunjuk/tease dari dukun biar player tahu hantu mana yang cocok berdasarkan stat.")]
        public string cue; // PARAMETER BARU UTK PETUNJUK STAT

        [Header("Hasil Teks")]
        [TextArea(2, 3)]
        public string winText = "Misi berhasil diselesaikan!";
        [TextArea(2, 3)]
        public string loseText = "Misi gagal total!";

        [Header("Dispatch")]
        [Tooltip("Durasi pengiriman hantu dalam detik (disimulasikan via Coroutine).")]
        [Min(0.1f)]
        public float dispatchDuration = 5f;

        [Header("Syarat Sukses")]
        [Tooltip("Semua requirement dirata-rata di calculation engine. Kosong = misi gampang.")]
        public StatRequirement[] requirements;

        [Header("Ekonomi")]
        public int rewardMoney = 100;  // didapat kalau sukses
        public int penaltyMoney = 50;  // dipotong kalau gagal
    }
}