using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Data-driven identitas hantu (Kuntilanak, Tuyul, dsb.) + 5 stat utamanya.
    /// Dibuat sebagai ScriptableObject supaya nilai balancing terpisah dari logic,
    /// dan tim Designer (Marvel) bisa atur stat tanpa nyentuh kode.
    /// </summary>
    [CreateAssetMenu(fileName = "Ghost_", menuName = "ODGJ/Ghost Data", order = 0)]
    public class GhostData : ScriptableObject
    {
        [Header("Identitas")]
        [Tooltip("ID unik & STABIL untuk save/load unlock. Jangan diubah setelah dipakai. " +
                 "Kalau dikosongkan, otomatis pakai ghostName.")]
        public string ghostId = "";

        public string ghostName = "Hantu Baru";

        [TextArea(2, 4)]
        public string description;

        [Tooltip("Diisi tim Art nanti. Boleh dikosongkan untuk core logic.")]
        public Sprite portrait;

        [Header("Stat (skala 1-10)")]
        public GhostStats stats = new GhostStats
        {
            magic = 5,
            sense = 5,
            charisma = 5,
            resilience = 5,
            practicality = 5
        };

        /// <summary>ID unik untuk save/load. Fallback ke ghostName kalau ghostId kosong.</summary>
        public string UniqueId => string.IsNullOrEmpty(ghostId) ? ghostName : ghostId;

        /// <summary>Shortcut baca stat tunggal.</summary>
        public int GetStat(GhostStatType type) => stats.Get(type);
    }
}
