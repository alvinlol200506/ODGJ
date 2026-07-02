using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ODGJ.Dispatch
{
    public class UIMissionCard : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image timerFillImage;
        [SerializeField] private Button missionButton;

        [Header("Settings")]
        [Tooltip("Berapa lama misi ini nunggu di UI sebelum hilang (detik)")]
        [SerializeField] private float lifetime = 30f; 

        private RequestData _requestData;
        private float _timeLeft;

        public void Setup(RequestData request)
        {
            _requestData = request;
            titleText.text = request.name; // Atau requestDescription
            _timeLeft = lifetime;

            missionButton.onClick.AddListener(OnClickMission);
        }

        private void Update()
        {
            if((UIDispatchController.Instance != null && UIDispatchController.Instance.IsOpen) || UIResultPopup.Instance._isShowing)
                return;

            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                timerFillImage.fillAmount = _timeLeft / lifetime;

                if (_timeLeft <= 0)
                {
                    ExpireMission();
                }
            }
        }

        private void OnClickMission()
        {
            // Panggil Dispatch Controller buat buka popup
            UIDispatchController.Instance.OpenDispatchPanel(this, _requestData);
        }

        public void DestroyMission()
        {
            Destroy(gameObject);
        }

        private void ExpireMission()
        {
            // Kalau misal panel lagi kebuka dan misi ini expired, tutup panelnya
            UIDispatchController.Instance.CheckAndCloseIfExpired(this);
            DestroyMission();
        }
    }
}