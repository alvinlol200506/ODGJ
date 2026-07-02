using System.Collections.Generic;
using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Lapisan persistence terpusat (satu-satunya yang nyentuh PlayerPrefs).
    /// Untuk sekarang yang disimpan HANYA: uang pemain + daftar hantu yang sudah unlock.
    ///
    /// Kenapa PlayerPrefs (bukan file JSON)? Karena jalan di SEMUA platform termasuk
    /// WebGL — penting kalau nanti kita submit versi browser ke itch.io. Save-to-file
    /// tidak reliable di WebGL.
    ///
    /// Uang & unlock disimpan di key TERPISAH supaya PlayerWallet dan GhostUnlockManager
    /// bisa nulis slice-nya masing-masing tanpa saling timpa.
    /// </summary>
    public static class SaveSystem
    {
        // Key uang sengaja sama dengan versi lama ("PlayerGold") biar save lama tetap kebaca.
        private const string MoneyKey = "PlayerGold";
        private const string UnlockedGhostsKey = "UnlockedGhosts";

        // ---------- Uang ----------

        public static void SaveMoney(int money)
        {
            PlayerPrefs.SetInt(MoneyKey, money);
            PlayerPrefs.Save();
        }

        public static int LoadMoney(int defaultMoney)
            => PlayerPrefs.GetInt(MoneyKey, defaultMoney);

        // ---------- Hantu ter-unlock ----------
        // JsonUtility tidak bisa serialize List langsung, jadi dibungkus class.

        [System.Serializable]
        private class GhostIdList
        {
            public List<string> ids = new List<string>();
        }

        public static void SaveUnlockedGhosts(IEnumerable<string> ghostIds)
        {
            var wrapper = new GhostIdList();
            wrapper.ids.AddRange(ghostIds);
            PlayerPrefs.SetString(UnlockedGhostsKey, JsonUtility.ToJson(wrapper));
            PlayerPrefs.Save();
        }

        public static List<string> LoadUnlockedGhosts()
        {
            string json = PlayerPrefs.GetString(UnlockedGhostsKey, "");
            if (string.IsNullOrEmpty(json))
                return new List<string>();

            var wrapper = JsonUtility.FromJson<GhostIdList>(json);
            return wrapper?.ids ?? new List<string>();
        }

        // ---------- Utilitas ----------

        public static bool HasSave()
            => PlayerPrefs.HasKey(MoneyKey) || PlayerPrefs.HasKey(UnlockedGhostsKey);

        /// <summary>Hapus semua save (buat tombol reset / testing).</summary>
        public static void ClearAll()
        {
            PlayerPrefs.DeleteKey(MoneyKey);
            PlayerPrefs.DeleteKey(UnlockedGhostsKey);
            PlayerPrefs.Save();
        }
    }
}
