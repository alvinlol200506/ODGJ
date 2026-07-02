using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ODGJ.Dispatch;

namespace ODGJ.Lobby
{
    // Struct pembungkus buat nyatuin SO Hantu sama Icon UI buatan lu
    [System.Serializable]
    public class GhostLobbyData
    {
        public GhostData ghostData;
        public Sprite unlockedIcon;
        public Sprite lockedIcon;
        public int unlockPrice = 200; // Harga buat buka hantu ini
    }

    public class PadepokanManager : MonoBehaviour
    {
        [Header("Databases")]
        [SerializeField] private List<GhostLobbyData> rosterList;
        [SerializeField] private List<GhostData> defaultUnlockedGhost; // Hantu pertama yg gratis (misal: Pocong)

        [Header("Spawners")]
        [SerializeField] private UIGhostIcon iconPrefab;
        [SerializeField] private Transform ghostListContainer;

        [Header("Gold Text UI")]
        [SerializeField] private TextMeshProUGUI goldTMP;

        [Header("Center Panel UI")]
        [SerializeField] private Image centerGhostSprite;
        [SerializeField] private TextMeshProUGUI centerGhostName;

        [Header("Description Panel UI")]
        [SerializeField] private TextMeshProUGUI descContentText;

        [Header("Stats Panel UI")]
        [SerializeField] private TextMeshProUGUI statsContentText;
        [SerializeField] private GameObject cueText;
        [SerializeField] private float cueBlinkInterval = 0.2f;
        [SerializeField] private float blinkDuration = 1f;
        
        private Coroutine _cueBlinkCoroutine;
        
        [Header("Action Button UI (Upgrade / Unlock)")]
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI actionButtonText;
        [SerializeField] private Image actionButtonImage;
        
        [Header("Warna Tombol")]
        [SerializeField] private Color colorUnlock = Color.yellow;
        [SerializeField] private Color colorUpgrade = Color.green;

        private GhostLobbyData _currentSelected;

        private void Start()
        {
            if (goldTMP != null && PlayerWallet.Instance != null)
            {
                goldTMP.text = PlayerWallet.Instance.Money.ToString();
            }
            else
            {
                Debug.LogWarning("[PadepokanManager] Gold TMP or PlayerWallet NOT FOUND!");
            }

            cueText.SetActive(false);

            // Buka hantu default pas game pertama kali jalan
            if (defaultUnlockedGhost != null) 
            {
                foreach (var ghost in defaultUnlockedGhost)
                {
                    PlayerPrefs.SetInt("Unlock_" + ghost.name, 1);
                }
            }

            SpawnIcons();
            
            // Otomatis pilih hantu pertama di list saat masuk padepokan
            if (rosterList.Count > 0) SelectGhost(rosterList[0]);
        }

        private void SpawnIcons()
        {
            // Bersihin container dulu (jaga-jaga kalau dipanggil ulang)
            foreach (Transform child in ghostListContainer) 
            {
                Destroy(child.gameObject);
            }

            // Spawn ulang semua icon
            foreach (var data in rosterList)
            {
                UIGhostIcon icon = Instantiate(iconPrefab, ghostListContainer);
                bool isUnlocked = IsGhostUnlocked(data.ghostData.name);
                icon.Setup(this, data, isUnlocked);
            }
        }

        public void SelectGhost(GhostLobbyData data)
        {
            // Stop coroutine blink jika sedang jalan
            if (_cueBlinkCoroutine != null)
            {
                StopCoroutine(_cueBlinkCoroutine);
                _cueBlinkCoroutine = null;
            }
            cueText.SetActive(false);
            
            _currentSelected = data;
            bool isUnlocked = IsGhostUnlocked(data.ghostData.name);

            // 1. Update Center UI
            centerGhostName.text = data.ghostData.ghostName;
            centerGhostSprite.sprite = data.ghostData.portrait;
            
            // 3. Logic percabangan (Udah punya vs Belum punya)
            if (isUnlocked)
            {
                descContentText.text = data.ghostData.description;
                
                statsContentText.text = $"MAG: {data.ghostData.stats.magic}\n" +
                                        $"SEN: {data.ghostData.stats.sense}\n" +
                                        $"CHA: {data.ghostData.stats.charisma}\n" +
                                        $"RES: {data.ghostData.stats.resilience}\n" +
                                        $"PRA: {data.ghostData.stats.practicality}";
                
                actionButtonText.text = "UPGRADE";
                actionButtonImage.color = colorUpgrade;
                
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(UpgradeGhost);
            }
            else
            {
                descContentText.text = "???\n\n(Hantu ini masih disegel, butuh sesajen untuk membuka kontaknya.)";
                statsContentText.text = "Hantu belum terbuka";
                
                actionButtonText.text = $"UNLOCK\n({data.unlockPrice} Koin)";
                actionButtonImage.color = colorUnlock;
                
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(UnlockCurrentGhost);
            }
        }

        // Mengecek ke memori save (PlayerPrefs) apakah file SO ini udah ke-unlock
        private bool IsGhostUnlocked(string ghostID)
        {
            return PlayerPrefs.GetInt("Unlock_" + ghostID, 0) == 1;
        }

        private void UnlockCurrentGhost()
        {
            if (PlayerWallet.Instance.Money < _currentSelected.unlockPrice) {
                Debug.Log("Duit kurang miskin!");
                // Cek apakah coroutine udah jalan, kalau belum baru mulai
                if (_cueBlinkCoroutine == null)
                {
                    cueText.SetActive(true);
                    _cueBlinkCoroutine = StartCoroutine(CueBlinkRoutine());
                }
                return; 
            }
            // Simpan status unlock ke memori
            PlayerPrefs.SetInt("Unlock_" + _currentSelected.ghostData.name, 1);
            PlayerPrefs.Save();

            PlayerWallet.Instance.TrySpend(_currentSelected.unlockPrice);
            Debug.Log($"{_currentSelected.ghostData.ghostName} Berhasil di Unlock!");

            // Refresh UI layar
            SpawnIcons(); 
            SelectGhost(_currentSelected); 
        }

        private void UpgradeGhost()
        {
            Debug.Log($"Mencoba Upgrade hantu {_currentSelected.ghostData.ghostName}...");
            // TODO: Logic nambahin stat SO hantu ditaruh di sini
        }

        private System.Collections.IEnumerator CueBlinkRoutine()
        {
            float elapsed = 0f;
            while (elapsed < blinkDuration)
            {
                cueText.SetActive(!cueText.activeSelf);
                yield return new WaitForSeconds(cueBlinkInterval);
                elapsed += cueBlinkInterval;
            }
            cueText.SetActive(false);
            _cueBlinkCoroutine = null; // Reset coroutine reference setelah selesai
        }
    }
}