using System;
using System.Collections.Generic;
using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Melacak hantu mana yang sudah ter-unlock (singleton, DDOL).
    /// Load dari SaveSystem saat Awake, dan auto-save tiap ada unlock baru.
    /// Identitas hantu pakai <see cref="GhostData.UniqueId"/> (stabil untuk save/load).
    /// </summary>
    public class GhostUnlockManager : MonoBehaviour
    {
        public static GhostUnlockManager Instance { get; private set; }

        [Header("Hantu starter (selalu unlock dari awal)")]
        [SerializeField] private List<GhostData> defaultUnlocked = new List<GhostData>();

        private readonly HashSet<string> _unlockedIds = new HashSet<string>();

        /// <summary>Dipancarkan saat sebuah hantu baru saja di-unlock (buat UI/SFX).</summary>
        public static event Action<GhostData> OnGhostUnlocked;

        public int UnlockedCount => _unlockedIds.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Load();
        }

        private void Load()
        {
            _unlockedIds.Clear();
            foreach (string id in SaveSystem.LoadUnlockedGhosts())
                _unlockedIds.Add(id);

            // Pastikan hantu starter selalu terbuka (misal pemain baru pertama main).
            bool changed = false;
            foreach (GhostData ghost in defaultUnlocked)
                if (ghost != null && _unlockedIds.Add(ghost.UniqueId))
                    changed = true;

            if (changed) Save();
        }

        public bool IsUnlocked(GhostData ghost)
            => ghost != null && _unlockedIds.Contains(ghost.UniqueId);

        /// <summary>Buka hantu. Return false kalau null atau sudah terbuka sebelumnya.</summary>
        public bool Unlock(GhostData ghost)
        {
            if (ghost == null) return false;
            if (!_unlockedIds.Add(ghost.UniqueId)) return false; // sudah unlock

            Save();
            Debug.Log($"[Unlock] Hantu '{ghost.ghostName}' terbuka! Total: {UnlockedCount}");
            OnGhostUnlocked?.Invoke(ghost);
            return true;
        }

        /// <summary>Ambil subset hantu yang sudah terbuka dari daftar semua hantu.</summary>
        public List<GhostData> GetUnlocked(IEnumerable<GhostData> allGhosts)
        {
            var result = new List<GhostData>();
            if (allGhosts == null) return result;
            foreach (GhostData ghost in allGhosts)
                if (IsUnlocked(ghost))
                    result.Add(ghost);
            return result;
        }

        private void Save() => SaveSystem.SaveUnlockedGhosts(_unlockedIds);
    }
}
