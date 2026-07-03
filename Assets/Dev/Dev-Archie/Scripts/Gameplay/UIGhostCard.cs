using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ODGJ.Dispatch
{
    public class UIGhostCard : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Image bgStatusImage;
        [SerializeField] private Image ghostSprite;
        [SerializeField] private Button ghostButton;

        [Header("Colors")]
        [SerializeField] private Color availableColor = Color.green;
        [SerializeField] private Color busyColor = Color.red;

        public GhostData GhostInfo { get; private set; }

        public void Setup(GhostData ghost)
        {
            GhostInfo = ghost;
            nameText.text = ghost.ghostName;
            if(ghost.portrait != null) ghostSprite.sprite = ghost.portrait;
            
            SetStatus(true);
            ghostButton.onClick.AddListener(OnClickGhost);
        }

        private void OnEnable()
        {
            // Dengerin event manager
            DispatchEvents.OnDispatchStarted += HandleDispatchStarted;
            DispatchEvents.OnDispatchFinished += HandleDispatchFinished;
        }

        private void OnDisable()
        {
            DispatchEvents.OnDispatchStarted -= HandleDispatchStarted;
            DispatchEvents.OnDispatchFinished -= HandleDispatchFinished;
        }

        private void HandleDispatchStarted(GhostData ghost, RequestData request)
        {
            if (ghost == GhostInfo) SetStatus(false); // Hantu ini mulai kerja
        }

        private void HandleDispatchFinished(DispatchReport report)
        {
            if (report.Ghost == GhostInfo) SetStatus(true); // Hantu ini selesai kerja
        }

        private void SetStatus(bool isAvailable)
        {
            statusText.text = isAvailable ? "Available" : "Sibuk";
            bgStatusImage.color = isAvailable ? availableColor : busyColor;
            
            // Opsional: Bikin tombol gak bisa diklik kalau sibuk
            // ghostButton.interactable = isAvailable; 
        }

        private void OnClickGhost()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClick);
            }
            else
            {
                Debug.LogWarning("[UIGhostCard] AudioManager NOT FOUND!");
            }
            // Cek dulu apakah hantunya sibuk
            if (DispatchManager.Instance.IsGhostBusy(GhostInfo))
            {
                Debug.Log("Hantu lagi sibuk bro!");
                return;
            }

            // Kirim hantu ini ke Dispatch Panel kalau panelnya lagi kebuka
            UIDispatchController.Instance.SelectGhost(GhostInfo);
        }
    }
}