using UnityEngine;
using UnityEngine.UI;

namespace ODGJ.UI
{
    /// <summary>
    /// Script ringan untuk menganimasikan komponen Image (UI) secara berulang.
    /// Tinggal drag and drop script ini ke GameObject yang punya komponen Image.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class SimpleUISpriteAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [Tooltip("Masukkan sprite frame animasinya secara berurutan di sini.")]
        [SerializeField] private Sprite[] frames;
        
        [Tooltip("Berapa frame yang berganti dalam 1 detik (FPS).")]
        [SerializeField] private float framesPerSecond = 12f;
        
        private Image _imageComponent;
        private int _currentFrameIndex = 0;
        private float _timer = 0f;

        private void Awake()
        {
            // Ambil komponen Image secara otomatis
            _imageComponent = GetComponent<Image>();
        }

        private void Update()
        {
            // Validasi: Kalau belum ada frame yang dimasukkan, jangan jalankan animasi
            if (frames == null || frames.Length == 0) return;

            // Tambahkan waktu setiap frame
            _timer += Time.deltaTime;
            
            // Hitung jeda waktu antar frame berdasarkan FPS (1 detik / FPS)
            float frameDuration = 1f / framesPerSecond;

            // Jika timer sudah melebihi durasi satu frame, ganti gambarnya
            if (_timer >= frameDuration)
            {
                // Reset timer (dikurangi, bukan diset 0, agar tidak ada frame drop/ketinggalan milidetik)
                _timer -= frameDuration;
                
                // Pindah ke frame berikutnya. Pakai % (modulo) agar otomatis muter ke awal kalau udah nyampe ujung
                _currentFrameIndex = (_currentFrameIndex + 1) % frames.Length;
                
                // Ubah sprite pada komponen Image
                _imageComponent.sprite = frames[_currentFrameIndex];
            }
        }
    }
}