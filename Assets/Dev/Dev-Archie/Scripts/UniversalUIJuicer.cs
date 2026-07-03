using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class UniversalUIJuicer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public enum TargetTransform { LocalMoveX, LocalMoveY, LocalRotateZ, PulsingScale }

    [System.Serializable]
    public class IdleStackSettings
    {
        public string stackName = "New Animation Stack";
        public TargetTransform effectType;
        public float intensity = 10f;         
        public float duration = 2f;
        public Ease easeType = Ease.InOutQuad;
        public LoopType loopType = LoopType.Yoyo;
    }

    [Header("Automation Settings")]
    [Tooltip("Centang TRUE untuk scene gameplay biasa. UNCHECK/FALSE jika objek ini diatur oleh LevelChooseIntro manager bray!")]
    [SerializeField] private bool playOnStart = true; 

    [Header("Highlighted / Hover Interaction")]
    [SerializeField] private bool useHoverInteraction = true;
    [SerializeField] private float hoverScaleMultiplier = 1.15f;
    [SerializeField] private float clickScaleMultiplier = 0.95f;
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    [Header("Stacking Idle Customization")]
    [SerializeField] private bool useIdleAnimations = true;
    [SerializeField] private List<IdleStackSettings> idleAnimationStacks = new List<IdleStackSettings>();

    private Vector3 baseScale;
    private Vector3 basePosition;
    private Vector3 baseRotation;

    private List<Tween> activeIdleTweens = new List<Tween>();
    private List<Tween> scaleIdleTweens = new List<Tween>();
    private Tween hoverTween;

    private void Awake()
    {
        RectTransform rect = GetComponent<RectTransform>();
        baseScale = rect != null ? Vector3.one : transform.localScale;

        basePosition = rect != null ? (Vector3)rect.anchoredPosition : transform.localPosition;
        baseRotation = transform.localEulerAngles;
    }

    // ====================================================================
    // SOLUSI: Fungsi Start aktif kembali dengan filter otomatis
    // ====================================================================
    private void Start()
    {
        if (useIdleAnimations && playOnStart)
        {
            StartAllIdleEffects();
        }
    }

    public void StartAllIdleEffects()
    {
        ClearActiveIdleTweens();

        foreach (var config in idleAnimationStacks)
        {
            Tween t = null;

            switch (config.effectType)
            {
                case TargetTransform.LocalMoveX:
                    RectTransform rectX = transform as RectTransform;
                    if (rectX != null)
                        t = rectX.DOAnchorPosX(basePosition.x + config.intensity, config.duration);
                    else
                        t = transform.DOLocalMoveX(basePosition.x + config.intensity, config.duration);
                    break;

                case TargetTransform.LocalMoveY:
                    RectTransform rectY = transform as RectTransform;
                    if (rectY != null)
                        t = rectY.DOAnchorPosY(basePosition.y + config.intensity, config.duration);
                    else
                        t = transform.DOLocalMoveY(basePosition.y + config.intensity, config.duration);
                    break;

                case TargetTransform.LocalRotateZ:
                    t = transform.DOLocalRotate(new Vector3(baseRotation.x, baseRotation.y, baseRotation.z + config.intensity), config.duration);
                    break;

                case TargetTransform.PulsingScale:
                    Vector3 targetScale = new Vector3(baseScale.x * config.intensity, baseScale.y * config.intensity, baseScale.z);
                    t = transform.DOScale(targetScale, config.duration);
                    break;
            }

            if (t != null)
            {
                t.SetEase(config.easeType)
                 .SetLoops(-1, config.loopType)
                 .SetUpdate(true);

                activeIdleTweens.Add(t);

                if (config.effectType == TargetTransform.PulsingScale)
                {
                    scaleIdleTweens.Add(t);
                }
            }
        }
    }

    // === HOVER / HIGHLIGHTED INTERACTION ===

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!useHoverInteraction) return;

        PauseScaleIdleTweens();
        hoverTween?.Kill();

        Vector3 targetHoverScale = new Vector3(baseScale.x * hoverScaleMultiplier, baseScale.y * hoverScaleMultiplier, baseScale.z);
        hoverTween = transform.DOScale(targetHoverScale, hoverDuration).SetEase(hoverEase).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!useHoverInteraction) return;

        hoverTween?.Kill();

        hoverTween = transform.DOScale(baseScale, hoverDuration).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
        {
            ResumeScaleIdleTweens();
        });
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!useHoverInteraction) return;

        hoverTween?.Kill();
        Vector3 targetClickScale = new Vector3(baseScale.x * clickScaleMultiplier, baseScale.y * clickScaleMultiplier, baseScale.z);
        hoverTween = transform.DOScale(targetClickScale, 0.05f).SetUpdate(true);
    }

    // === UTILITY SEPARATOR LOGIC ===

    private void PauseScaleIdleTweens()
    {
        foreach (var tween in scaleIdleTweens)
        {
            if (tween != null && tween.IsActive()) tween.Pause();
        }
    }

    private void ResumeScaleIdleTweens()
    {
        foreach (var tween in scaleIdleTweens)
        {
            if (tween != null && tween.IsActive()) tween.Play();
        }
    }

    private void ClearActiveIdleTweens()
    {
        foreach (var tween in activeIdleTweens)
        {
            if (tween != null) tween.Kill();
        }
        activeIdleTweens.Clear();
        scaleIdleTweens.Clear();
    }

    private void OnDestroy()
    {
        hoverTween?.Kill();
        ClearActiveIdleTweens();
        transform.DOKill();
    }
}