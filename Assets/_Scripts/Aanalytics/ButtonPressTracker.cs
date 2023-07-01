using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;


public class ButtonPressTracker : MonoBehaviour
{
    public string buttonName; 

    public void OnButtonPress()
    {
     
        Analytics.CustomEvent("ButtonPressed", new Dictionary<string, object>
        {
            { "ButtonName", buttonName }
        });
    }
}
