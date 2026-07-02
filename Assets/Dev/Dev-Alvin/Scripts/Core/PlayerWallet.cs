using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Mengelola uang pemain secara persistent menggunakan PlayerPrefs (Singleton & DDOL).
    /// </summary>
    public class PlayerWallet : MonoBehaviour
    {
        public static PlayerWallet Instance { get; private set; }

        [SerializeField] private int startingMoney = 500;

        public int Money { get; private set; }

        // Bikin key konstan buat string di PlayerPrefs biar gak typo
        private const string GoldPrefsKey = "PlayerGold";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // SINKRONISASI: Ambil data duit lama dari PlayerPrefs.
            // Kalau data gak ditemuin (baru pertama kali main), otomatis pakai nilai startingMoney.
            Money = PlayerPrefs.GetInt(GoldPrefsKey, startingMoney);
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[Wallet] Tidak bisa menambahkan jumlah negatif atau nol via AddMoney: {amount}");
            }
            else
            {
                Money += amount;
                SaveMoneyToPrefs(); // Simpan ke memori tiap ada perubahan
                Debug.Log($"[Wallet] Menambahkan {amount} koin. Saldo sekarang: {Money}");
            }
        }

        public void TrySpend(int ammount)
        {
            if (CanSpend(ammount))
            {
                Money -= ammount;
                SaveMoneyToPrefs(); // Simpan ke memori tiap ada perubahan
                Debug.Log($"[Wallet] Mengurangi {ammount} koin. Saldo sekarang: {Money}");
            }
            else
            {
                Debug.LogWarning($"[Wallet] Tidak cukup koin untuk mengurangi {ammount}. Saldo sekarang: {Money}");
            }
        }

        private bool CanSpend(int ammount)
        {
            return Money >= ammount;
        }

        // Fungsi internal buat nge-save data ke local storage perangkat
        private void SaveMoneyToPrefs()
        {
            PlayerPrefs.SetInt(GoldPrefsKey, Money);
            PlayerPrefs.Save();
        }

        private void OnEnable()
        {
            DispatchEvents.OnDispatchFinished += HandleDispatchFinished;
        }

        private void OnDisable()
        {
            DispatchEvents.OnDispatchFinished -= HandleDispatchFinished;
        }

        private void HandleDispatchFinished(DispatchReport report)
        {
            // FIX BUG: Percabangan biar penalti (nilai minus) bisa memotong duit player
            if (report.MoneyDelta >= 0)
            {
                // Kalau sukses, tambahkan duit lewat fungsi normal
                AddMoney(report.MoneyDelta);
            }
            else
            {
                // Kalau gagal (minus), langsung potong nilainya disini bypass fungsi AddMoney
                Money += report.MoneyDelta; 
                
                // Jaga-jaga biar duit player gak minus di bawah nol (clamp)
                if (Money < 0) Money = 0; 

                SaveMoneyToPrefs(); // Simpan ke memori
                
                string sign = report.MoneyDelta >= 0 ? "+" : "";
                Debug.Log($"[Wallet] {report.Request.clientName}: {sign}{report.MoneyDelta} koin (Penalti). " +
                          $"Saldo sekarang: {Money}");
            }
        }
    }
}