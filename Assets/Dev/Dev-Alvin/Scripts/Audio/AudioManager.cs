using UnityEngine;

namespace ODGJ.Dispatch
{
    /// <summary>
    /// Manager audio terpusat (singleton, DDOL). Dua channel: BGM (loop) & SFX (one-shot).
    /// Otomatis nyambung ke core loop lewat Observer Pattern — dengerin DispatchEvents
    /// lalu mainkan SFX yang sesuai TANPA DispatchManager tahu soal audio.
    ///
    /// Slot AudioSource boleh dibiarkan kosong: kalau null, dibuat otomatis saat Awake.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources (auto-create kalau kosong)")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("BGM")]
        [SerializeField] private AudioClip defaultBgm;

        [Header("SFX Dispatch (dimainkan otomatis via event)")]
        [SerializeField] private AudioClip sfxDispatchStart;
        [SerializeField] private AudioClip sfxSuccess;
        [SerializeField] private AudioClip sfxFail;

        [Header("Volume (0..1)")]
        [Range(0f, 1f)] [SerializeField] private float bgmVolume = 0.7f;
        [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.playOnAwake = false;
                bgmSource.loop = true;
            }
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            ApplyBgmVolume();
        }

        private void Start()
        {
            if (defaultBgm != null)
                PlayBGM(defaultBgm);
        }

        private void OnEnable()
        {
            DispatchEvents.OnDispatchStarted += HandleDispatchStarted;
            DispatchEvents.OnDispatchFinished += HandleDispatchFinished;
        }

        private void OnDisable()
        {
            DispatchEvents.OnDispatchStarted -= HandleDispatchStarted;
            DispatchEvents.OnDispatchFinished -= HandleDispatchFinished;
        }

        // ---------- Auto SFX dari event ----------

        private void HandleDispatchStarted(GhostData ghost, RequestData request)
            => PlaySFX(sfxDispatchStart);

        private void HandleDispatchFinished(DispatchReport report)
            => PlaySFX(report.IsSuccess ? sfxSuccess : sfxFail);

        // ---------- Public API (buat UI: tombol, klik, dsb.) ----------

        public void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (clip == null || bgmSource == null) return;
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }

        public void StopBGM()
        {
            if (bgmSource != null) bgmSource.Stop();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void SetBgmVolume(float value)
        {
            bgmVolume = Mathf.Clamp01(value);
            ApplyBgmVolume();
        }

        public void SetSfxVolume(float value) => sfxVolume = Mathf.Clamp01(value);

        private void ApplyBgmVolume()
        {
            if (bgmSource != null) bgmSource.volume = bgmVolume;
        }
    }
}
