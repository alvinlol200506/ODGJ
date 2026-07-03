using UnityEngine;
using TMPro;
using ODGJ.Dispatch;

public class UIMaincanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldTMP;

    void OnEnable()
    {
        DispatchEvents.OnDispatchFinished += HandledDispatchFinished;
    }

    void OnDisable()
    {
        DispatchEvents.OnDispatchFinished -= HandledDispatchFinished;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateGoldUI();
        AudioManager.Instance.PlayBGM(AudioManager.Instance.defaultBgm);
    }

    private void HandledDispatchFinished(DispatchReport report)
    {
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if(PlayerWallet.Instance != null)
        {
            goldTMP.text = PlayerWallet.Instance.Money.ToString();
        } else
        {
            Debug.LogWarning("[PlayerWallet] NOT FOUND!");
        }
    }
}
