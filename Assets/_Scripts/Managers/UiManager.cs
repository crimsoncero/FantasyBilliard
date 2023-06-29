using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private Image _p1TurnBanner;
    [SerializeField] private Image _p2TurnBanner;
    [SerializeField] private Image _p1ExtraTurnBanner;
    [SerializeField] private Image _p2ExtraTurnBanner;
    [SerializeField] private Image _p1PenaltyBanner;
    [SerializeField] private Image _p2PenaltyBanner;
    [SerializeField] private Vector3 _genericFlashTiming = new Vector3(1f, 0.8f, 1f);  

    
   
    public void FlashAlpha(Image banner, Vector3 flashTimings)
    {
        Sequence seq = DOTween.Sequence();

        Color originalColor = banner.color;

        
        banner.gameObject.SetActive(true);

        
        seq.Append(banner.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 1f), flashTimings.x).SetEase(Ease.OutBack));

        
        seq.AppendInterval(flashTimings.y);

        
        seq.Append(banner.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0f), flashTimings.z).SetEase(Ease.InBack))
            .OnComplete(() => banner.gameObject.SetActive(false));
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

}


    #region API Calls
    //public Sequence P1BannerFlash()
    //{
    //    return FlashElement(_UIELEMENTTEXT, _genericFlashTiming);

    //}


    //#endregion


    //#region Generic Methods


    ///// <summary>
    ///// Generic method that flashes a given UI element 
    ///// </summary>
    ///// <param name="banner"></param>
    ///// <returns></returns>
    //private Sequence FlashElement(GameObject element, Vector3 flashTimings)
    //{
    //    Sequence seq = DOTween.Sequence();

    //    Tween flashIn;
    //    Tween flashOut;

    //    seq.Append(flashIn);
    //    seq.AppendInterval(flashTimings.y);
    //    seq.Append(flashOut);

    //    return seq;

    //}

   

    #endregion

