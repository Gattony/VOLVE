using UnityEngine;

public class RunCounter : MonoBehaviour
{
    public static RunCounter Instance;

    private int runsSinceLastAd = 0;
    private const int runsBeforeAd = 1;

    private void Awake()
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.interestialAds.ShowInterstitialAd();
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

 public void OnPlayerDied()
   {
        runsSinceLastAd++;

        if (runsSinceLastAd >= runsBeforeAd)
        {
            if (AdManager.Instance != null)
            {
                AdManager.Instance.interestialAds.ShowInterstitialAd();
            }
            runsSinceLastAd = 0; 
        }
    }
}
