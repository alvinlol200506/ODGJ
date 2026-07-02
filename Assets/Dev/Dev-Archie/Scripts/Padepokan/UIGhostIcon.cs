using UnityEngine;
using UnityEngine.UI;
using ODGJ.Dispatch; // Ngambil referensi GhostData

namespace ODGJ.Lobby
{
    public class UIGhostIcon : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Button button;
        
        private PadepokanManager _manager;
        private GhostLobbyData _data;

        public void Setup(PadepokanManager manager, GhostLobbyData data, bool isUnlocked)
        {
            _manager = manager;
            _data = data;

            // Ganti sprite tergantung status unlock
            iconImage.sprite = isUnlocked ? data.unlockedIcon : data.lockedIcon;
            
            // Hapus listener lama biar ga numpuk, trus pasang yang baru
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickIcon);
        }

        private void OnClickIcon()
        {
            _manager.SelectGhost(_data);
        }
    }
}