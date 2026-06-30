using UnityEngine;
using DG.Tweening;
using TMPro;

public class HoverText : MonoBehaviour
{
    [SerializeField] RectTransform textRect;

    [Header("Hover Settings")]
    [SerializeField] float hoverDuration = 2f;
    [SerializeField] float hoverDistance = 20f;

    [Header("Slide Settings")]
    [SerializeField] float slideDuration = 2f;
    [SerializeField] float slideDistance = 20f;

    

    [Header("Rotation Settings")]
    [SerializeField] float rotationAngle = 5f;
    [SerializeField] float rotationDuration = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textRect.DOAnchorPosY(textRect.anchoredPosition.y + hoverDistance, hoverDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        textRect.DOAnchorPosX(textRect.anchoredPosition.x + slideDistance, slideDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        textRect.localRotation = Quaternion.Euler(0, 0, -rotationAngle);

        textRect.DORotate(new Vector3(0, 0, rotationAngle), rotationDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
