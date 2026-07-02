using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Contoh subscriber yang DECOUPLED dari DispatchManager: dia cuma dengerin
    /// OnDispatchFinished lalu update uang pemain. Tidak ada referensi langsung
    /// ke manager — bukti Observer Pattern jalan. Pola yang sama dipakai nanti
    /// untuk UI hasil, SFX sukses/gagal, dsb.
    /// </summary>
    public class PlayerWallet : MonoBehaviour
    {
        public static PlayerWallet Instance { get; private set; }

        [SerializeField] private int startingMoney = 500;

        public int Money { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            Money = startingMoney;
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[Wallet] Tidak bisa menambahkan jumlah negatif atau nol: {amount}");
            }
            else
            {
                Money += amount;
                Debug.Log($"[Wallet] Menambahkan {amount} koin. Saldo sekarang: {Money}");
            }
        }

        public void TrySpend(int ammount)
        {
            if (CanSpend(ammount))
            {
                Money -= ammount;
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

        private void OnEnable()
        {
            DispatchEvents.OnDispatchFinished += HandleDispatchFinished;
        }

        private void OnDisable()
        {
            // Wajib unsubscribe biar tidak leak / NullReference saat scene reload.
            DispatchEvents.OnDispatchFinished -= HandleDispatchFinished;
        }

        private void HandleDispatchFinished(DispatchReport report)
        {
            Money += report.MoneyDelta;
            string sign = report.MoneyDelta >= 0 ? "+" : "";
            Debug.Log($"[Wallet] {report.Request.clientName}: {sign}{report.MoneyDelta} koin. " +
                      $"Saldo sekarang: {Money}");
        }
    }
}
