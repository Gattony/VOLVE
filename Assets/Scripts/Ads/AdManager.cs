using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public InitalizeAds initalizeAds;
    public BannerAds bannerAds;
    public InterestialAds interestialAds;
    public RewardedAds rewardedAds;

    public static AdManager Instance { get; private set; }

    private void Awake()
    {
       if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        bannerAds.LoadBannerAd();
        interestialAds.LoadInterestialAd();
        rewardedAds.LoadRewardedAd();
    }
}
