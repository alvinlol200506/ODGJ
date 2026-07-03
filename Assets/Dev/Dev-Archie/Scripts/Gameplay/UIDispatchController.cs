using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ODGJ.Dispatch
{
    public class UIDispatchController : MonoBehaviour
    {
        public static UIDispatchController Instance { get; private set; }

        public bool IsOpen { get; private set; }

        [Header("Canvas & Groups")]
        [SerializeField] private CanvasGroup dispatchCanvas;
        
        [Header("Mission Details UI")]
        [SerializeField] private TextMeshProUGUI missionAssignerText;
        [SerializeField] private TextMeshProUGUI missionTitleText;
        [SerializeField] private TextMeshProUGUI missionDescText;
        [SerializeField] private TextMeshProUGUI requirementsText;

        [Header("Ghost Slot UI")]
        [Tooltip("Container utama tetep nyala biar frame-nya kelihatan")]
        [SerializeField] private GameObject ghostSlotContainer; 
        [SerializeField] private Image selectedGhostSprite;
        [SerializeField] private TextMeshProUGUI selectedGhostName;
        [SerializeField] private TextMeshProUGUI ghostStatText; // Tambahan referensi buat Teks Stat Hantu
        
        [Header("Buttons")]
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnBatal;
        [SerializeField] private Button btnSantet;

        private RequestData _currentRequest;
        private GhostData _currentGhost;
        private UIMissionCard _currentMissionCard;

        private void Awake()
        {
            Instance = this;
            
            btnClose.onClick.AddListener(ClosePanel);
            btnBatal.onClick.AddListener(ClearSelectedGhost);
            btnSantet.onClick.AddListener(DeploySantet);
            
            ClosePanel(); // Reset pas game mulai
        }

        public void OpenDispatchPanel(UIMissionCard card, RequestData request)
        {
            _currentMissionCard = card;
            _currentRequest = request;
            IsOpen = true;

            // Update UI Misi
            missionAssignerText.text = request.clientName;
            missionTitleText.text = request.name;
            missionDescText.text = request.requestDescription;
            
            requirementsText.text = request.cue;

            // Kembalikan ke kondisi awal (hantu kosong, tombol hide)
            ClearSelectedGhost(); 
            
            dispatchCanvas.alpha = 1f;
            dispatchCanvas.blocksRaycasts = true;
            dispatchCanvas.interactable = true;
        }

        public void SelectGhost(GhostData ghost)
        {
            if (!IsOpen) return;

            _currentGhost = ghost;
            
            // Pastikan containernya nyala (jaga-jaga kalau sempet mati)
            ghostSlotContainer.SetActive(true);

            // Update & Munculin visual hantu
            selectedGhostName.text = ghost.ghostName;
            selectedGhostName.gameObject.SetActive(true);

            if(ghost.portrait != null) 
            {
                selectedGhostSprite.sprite = ghost.portrait;
                selectedGhostSprite.gameObject.SetActive(true);
            }

            // Update & Munculin Text Stat Hantu di tengah
            if(ghostStatText != null)
            {
                ghostStatText.text = $"Magic: {ghost.stats.magic}\n" +
                                     $"Sense: {ghost.stats.sense}\n" +
                                     $"Charisma: {ghost.stats.charisma}\n" +
                                     $"Resilience: {ghost.stats.resilience}\n" +
                                     $"Practicality: {ghost.stats.practicality}";
                ghostStatText.gameObject.SetActive(true);
            }

            // Munculin tombol Batal & Santet
            btnBatal.gameObject.SetActive(true);
            btnSantet.gameObject.SetActive(true);
        }

        private void ClearSelectedGhost()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
            }
            else
            {
                Debug.LogWarning("[UIDispatchController] AudioManager NOT FOUND!");
            }
            _currentGhost = null;
            
            // Jangan matiin ghostSlotContainer biar kotak layoutnya tetep keliatan.
            // Matiin aja isi-isinya:
            if(selectedGhostSprite != null) selectedGhostSprite.gameObject.SetActive(false);
            if(selectedGhostName != null) selectedGhostName.gameObject.SetActive(false);
            if(ghostStatText != null) ghostStatText.gameObject.SetActive(false);
            
            if(btnBatal != null) btnBatal.gameObject.SetActive(false);
            if(btnSantet != null) btnSantet.gameObject.SetActive(false);
        }

        private void DeploySantet()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
            }
            else
            {
                Debug.LogWarning("[UIDispatchController] AudioManager NOT FOUND!");
            }
            if (_currentGhost != null && _currentRequest != null)
            {
                bool success = DispatchManager.Instance.TryStartDispatch(_currentGhost, _currentRequest);
                if (success)
                {
                    _currentMissionCard.DestroyMission();
                    ClosePanel();
                }
            }
        }

        public void CheckAndCloseIfExpired(UIMissionCard expiredCard)
        {
            if (_currentMissionCard == expiredCard && dispatchCanvas.alpha > 0)
            {
                ClosePanel();
            }
        }

        public void ClosePanel()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
            }
            else
            {
                Debug.LogWarning("[UIDispatchController] AudioManager NOT FOUND!");
            }
            IsOpen = false;

            dispatchCanvas.alpha = 0f;
            dispatchCanvas.blocksRaycasts = false;
            dispatchCanvas.interactable = false;
            _currentRequest = null;
            _currentMissionCard = null;
            ClearSelectedGhost();

            if(UIResultPopup.Instance != null)
            {
                UIResultPopup.Instance.TriggerQueue();
            }
        }
    }
}