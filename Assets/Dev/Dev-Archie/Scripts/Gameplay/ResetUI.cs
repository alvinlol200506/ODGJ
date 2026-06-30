using UnityEngine;

public class ResetUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup dispatchCanvas;
    [SerializeField] private CanvasGroup pauseCanvas;

    void Start()
    {
        if(dispatchCanvas != null && pauseCanvas != null)
        {
            //reset canvas dispatch
            dispatchCanvas.alpha = 0f;
            dispatchCanvas.blocksRaycasts = false;

            //reset canvas pause
            pauseCanvas.alpha = 0f;
            pauseCanvas.blocksRaycasts = false;
        }
    }
}
