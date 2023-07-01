using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private Canvas _uiCanvas;

    [Header("UI Cooldown Texts")]
    [SerializeField] private TextMeshProUGUI _p1AbilityCooldownText;
    [SerializeField] private TextMeshProUGUI _p2AbilityCooldownText;

    [Header("UI Status Banners")]
    [SerializeField] private Image _p1TurnBanner;
    [SerializeField] private Image _p2TurnBanner;
    [SerializeField] private Image _p1ExtraTurnBanner;
    [SerializeField] private Image _p2ExtraTurnBanner;
    [SerializeField] private Image _p1PenaltyBanner;
    [SerializeField] private Image _p2PenaltyBanner;
    [SerializeField] private Image _p1WinBanner;
    [SerializeField] private Image _p2WinBanner;

    [SerializeField] private Vector3 _genericFlashTiming = new Vector3(1f, 0.8f, 1f);


    public void ToggleUI(bool active)
    {
        _uiCanvas.enabled = active;
    }
  
    public void UpdateUI()
    {
        UpdateCDText(_p1AbilityCooldownText, AbilityManager.Instance.ArcaneBarrierCurrentCD);
        UpdateCDText(_p2AbilityCooldownText, AbilityManager.Instance.ShadowShotCurrentCD);
    }
        


    public Sequence P1BannerFlash()
    {
       return FlashAlpha(_p1TurnBanner, _genericFlashTiming);
       
    }

    public Sequence P2BannerFlash()
    {
        return FlashAlpha(_p2TurnBanner, _genericFlashTiming);
    }

    public Sequence P1ExtraBannerFlash()
    {
        return FlashAlpha(_p1ExtraTurnBanner, _genericFlashTiming);
    }

    public Sequence P2ExtraBannerFlash()
    {
        return FlashAlpha(_p2ExtraTurnBanner, _genericFlashTiming);
    }

    public Sequence P1PenaltyBannerFlash()
    {
        return FlashAlpha(_p1PenaltyBanner, _genericFlashTiming);
    }

    public Sequence P2PenaltyBannerFlash()
    {
        return FlashAlpha(_p2PenaltyBanner, _genericFlashTiming);
    }

    public Sequence P1WinFlash()
    {
        return FlashAlpha(_p1WinBanner, _genericFlashTiming);
    }
    public Sequence P2WinFlash()
    {
        return FlashAlpha(_p2WinBanner, _genericFlashTiming);
    }

    private Sequence FlashAlpha(Image banner, Vector3 flashTimings)
    {
        Sequence seq = DOTween.Sequence();

        Color originalColor = banner.color;


        banner.gameObject.SetActive(true);


        seq.Append(banner.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 1f), flashTimings.x).SetEase(Ease.OutBack));


        seq.AppendInterval(flashTimings.y);


        seq.Append(banner.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0f), flashTimings.z).SetEase(Ease.InBack))
            .OnComplete(() => banner.gameObject.SetActive(false));

        return seq;
    }

    private void UpdateCDText(TextMeshProUGUI text,float val)
    {
        string cooldownText = val.ToString();

        text.text = cooldownText;
        text.enabled = (val > 0);

    }
}



 


   

