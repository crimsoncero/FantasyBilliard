using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private GameObject _UIELEMENTTEXT; // Reference to each of the ui elements (change gameobject to whatever needed to control it


    [SerializeField] private Vector3 _genericFlashTiming = new Vector3(1f, 2f, 1f);  // Generic timing for flashing elements - In, Hold, Out.


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
}
