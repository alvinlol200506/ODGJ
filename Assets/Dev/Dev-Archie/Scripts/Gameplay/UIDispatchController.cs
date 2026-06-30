using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ODGJ.Dispatch
{
    public class UIDispatchController : MonoBehaviour
    {
        public static UIDispatchController Instance { get; private set; }

        [Header("Canvas & Groups")]
        [SerializeField] private CanvasGroup dispatchCanvas;
        
        [Header("Mission Details UI")]
        [SerializeField] private TextMeshProUGUI missionTitleText;
        [SerializeField] private TextMeshProUGUI missionDescText;
        [SerializeField] private TextMeshProUGUI requirementsText;

        [Header("Ghost Slot UI")]
        [SerializeField] private GameObject ghostSlotContainer;
        [SerializeField] private Image selectedGhostSprite;
        [SerializeField] private TextMeshProUGUI selectedGhostName;
        
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
            
            ClosePanel(); // Pastikan ketutup di awal
        }

        public void OpenDispatchPanel(UIMissionCard card, RequestData request)
        {
            _currentMissionCard = card;
            _currentRequest = request;

            // Update Text UI
            missionTitleText.text = request.clientName;
            missionDescText.text = request.requestDescription;
            
            // Format Requirement Text
            string reqStr = "";
            foreach(var req in request.requirements)
            {
                reqStr += $"{req.stat}: {req.requiredValue}\n";
            }
            requirementsText.text = string.IsNullOrEmpty(reqStr) ? "Tidak ada syarat khusus" : reqStr;

            ClearSelectedGhost(); // Kosongin slot hantu pas baru buka
            
            dispatchCanvas.alpha = 1f;
            dispatchCanvas.blocksRaycasts = true;
            dispatchCanvas.interactable = true;
        }

        public void SelectGhost(GhostData ghost)
        {
            if (dispatchCanvas.alpha == 0) return; // Kalau panel gak kebuka, cuekin

            _currentGhost = ghost;
            
            ghostSlotContainer.SetActive(true);
            selectedGhostName.text = ghost.ghostName;
            if(ghost.portrait != null) selectedGhostSprite.sprite = ghost.portrait;

            // Munculin tombol Batal & Santet
            btnBatal.gameObject.SetActive(true);
            btnSantet.gameObject.SetActive(true);
        }

        private void ClearSelectedGhost()
        {
            _currentGhost = null;
            ghostSlotContainer.SetActive(false);
            
            // Sembunyiin tombol
            btnBatal.gameObject.SetActive(false);
            btnSantet.gameObject.SetActive(false);
        }

        private void DeploySantet()
        {
            if (_currentGhost != null && _currentRequest != null)
            {
                bool success = DispatchManager.Instance.TryStartDispatch(_currentGhost, _currentRequest);
                if (success)
                {
                    _currentMissionCard.DestroyMission(); // Hapus misi dari UI List
                    ClosePanel(); // Tutup panel
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

        private void ClosePanel()
        {
            dispatchCanvas.alpha = 0f;
            dispatchCanvas.blocksRaycasts = false;
            dispatchCanvas.interactable = false;
            _currentRequest = null;
            _currentMissionCard = null;
            ClearSelectedGhost();
        }
    }
}