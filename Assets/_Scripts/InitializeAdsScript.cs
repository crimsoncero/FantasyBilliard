using UnityEngine;
using UnityEngine.Advertisements;

public class InitializeAdsScript : MonoBehaviour
{

    string gameId = "5333361";
    bool testMode = true;

    void Awake()
    {
        Advertisement.Initialize(gameId, testMode, GameManager.Instance);
    }
}