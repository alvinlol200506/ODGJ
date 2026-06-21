using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// 5 stat utama hantu. Dipakai sebagai key untuk komparasi stat vs requirement misi.
    /// MAG = Magic, SEN = Sense, CHA = Charisma, RES = Resilience, PRA = Practicality.
    /// </summary>
    public enum GhostStatType
    {
        MAG, // Magic        - kekuatan gaib / santet
        SEN, // Sense        - kepekaan / deteksi
        CHA, // Charisma     - daya pengaruh / tipu daya
        RES, // Resilience   - ketahanan / tidak gampang kabur
        PRA  // Practicality - kepraktisan / efisiensi kerja
    }

    /// <summary>
    /// Kumpulan 5 stat hantu (skala 1-10). Struct supaya ringan & gampang di-copy.
    /// Diakses lewat Get(GhostStatType) biar calculation engine tetap rapi.
    /// </summary>
    [System.Serializable]
    public struct GhostStats
    {
        [Range(1, 10)] public int magic;        // MAG
        [Range(1, 10)] public int sense;        // SEN
        [Range(1, 10)] public int charisma;     // CHA
        [Range(1, 10)] public int resilience;   // RES
        [Range(1, 10)] public int practicality; // PRA

        /// <summary>Ambil nilai stat berdasarkan tipe-nya.</summary>
        public int Get(GhostStatType type)
        {
            return type switch
            {
                GhostStatType.MAG => magic,
                GhostStatType.SEN => sense,
                GhostStatType.CHA => charisma,
                GhostStatType.RES => resilience,
                GhostStatType.PRA => practicality,
                _ => 0
            };
        }
    }
}
