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
        

   


    public void P1BannerFlash()
    {
        FlashAlpha(_p1TurnBanner, _genericFlashTiming);
    }

    public void P2BannerFlash()
    {
        FlashAlpha(_p2TurnBanner, _genericFlashTiming);
    }

    public void P1ExtraBannerFlash()
    {
        FlashAlpha(_p1ExtraTurnBanner, _genericFlashTiming);
    }

    public void P2ExtraBannerFlash()
    {
        FlashAlpha(_p2ExtraTurnBanner, _genericFlashTiming);
    }

    public void P1PenaltyBannerFlash()
    {
        FlashAlpha(_p1PenaltyBanner, _genericFlashTiming);
    }

    public void P2PenaltyBannerFlash()
    {
        FlashAlpha(_p2PenaltyBanner, _genericFlashTiming);
    }

    private void FlashAlpha(Image banner, Vector3 flashTimings)
    {
        Sequence seq = DOTween.Sequence();

        Color originalColor = banner.color;


        banner.gameObject.SetActive(true);


        seq.Append(banner.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 1f), flashTimings.x).SetEase(Ease.OutBack));


        seq.AppendInterval(flashTimings.y);


        seq.Append(banner.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0f), flashTimings.z).SetEase(Ease.InBack))
            .OnComplete(() => banner.gameObject.SetActive(false));
    }

    private void UpdateCDText(TextMeshProUGUI text,float val)
    {
        string cooldownText = val.ToString();

        text.text = cooldownText;
        text.enabled = (val > 0);

    }
}



 


   

