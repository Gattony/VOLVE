using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InitalizeAds : MonoBehaviour ,IUnityAdsInitializationListener 
{
    [SerializeField] private string androidGameId;
    [SerializeField] private string iosGameId;
    [SerializeField] private bool isTesting;

    private string gameId;

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Initialized");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        #if UNITY_IOS
             gameId = iosGameId;
        #elif UNITY_ANDROID
                gameId = androidGameId;
#elif UNITY_EDITOR
        gameId= androidGameId;
#endif 
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, isTesting, this);
        }
    }
}
