using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UiManager : Singleton<UiManager>
{
    [SerializeField] private GameObject _UIELEMENTTEXT; // Reference to each of the ui elements (change gameobject to whatever needed to control it


    [SerializeField] private Vector3 _genericFlashTiming = new Vector3(1f, 2f, 1f);  // Generic timing for flashing elements - In, Hold, Out.


    #region API Calls
    public IEnumerator P1BannerFlash()
    {
        yield return StartCoroutine(FlashElement(_UIELEMENTTEXT,_genericFlashTiming));
    }

    #endregion


    #region Generic Methods


    /// <summary>
    /// Generic method that flashes a given UI element 
    /// </summary>
    /// <param name="banner"></param>
    /// <returns></returns>
    private IEnumerator FlashElement(GameObject element, Vector3 flashTimings)
    {

        yield return StartCoroutine(FlashInElement(element, flashTimings.x));  // Flash In.
                                                                               // Wait
        yield return new WaitForSeconds(flashTimings.y);                       // Hold
                                                                               // Wait
        yield return StartCoroutine(FlashOutElement(element, flashTimings.z)); // Flash Out
                                                                               // Wait

    }

    /// <summary>
    /// Flashes in a ui element.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashInElement(GameObject element, float time)
    {
        bool isTweenOver = false; // Boolean to check that the tween is over

        Tween tween = element.transform.DOMove(Vector3.zero, time); // Replace this tween with the real one.

        tween.OnComplete(() => isTweenOver = true); // When the tween completes, flip the boolean to true.

        yield return new WaitUntil(() => isTweenOver == true); // Wait until the tween is over.
    }

    /// <summary>
    /// Flashes out a ui element.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashOutElement(GameObject element, float time)
    {
        bool isTweenOver = false; // Boolean to check that the tween is over

        Tween tween = element.transform.DOMove(Vector3.zero, time); // Replace this tween with the real one.

        tween.OnComplete(() => isTweenOver = true); // When the tween completes, flip the boolean to true.

        yield return new WaitUntil(() => isTweenOver == true); // Wait until the tween is over.
    }

    #endregion
}
