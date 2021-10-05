using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public static class RewardedAdController
{
    static GameController gameCont;
    static private RewardedAd rewardedAd;
    static public BannerView banner;
    static AdRequest request;

    static public BannerView Banner
    {
        set { banner = value; }
        get { return banner; }
    }

    static public AdRequest Request
    {
        get{ return request; }
        set { request = value; }
    }

    static public void StartRewardedAd()
    {
        CreateAndLoadRewardedAd();
        InitialBanner();
    }

    static void InitialBanner()
    {
        #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-9743847943087662/2038403307";
        #elif UNITY_IPHONE
                                //TODO string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
                                string adUnitId = "unexpected_platform";
        #endif

        BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
        banner = bannerView;
    }

    static public void CreateAndLoadRewardedAd()
    {
        #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-9743847943087662/9533749948";
        #elif UNITY_IPHONE
                //TODO string adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
                string adUnitId = "unexpected_platform";
        #endif

        rewardedAd = new RewardedAd(adUnitId);

        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);
    }

    static public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        CreateAndLoadRewardedAd();
    }

    static public bool IsReady()
    {
        return rewardedAd.IsLoaded();
    }

    static public void ShowAd()
    {
        rewardedAd.Show();
    }

    static public void HandleUserEarnedReward(object sender, Reward args)
    {
        gameCont.RewardedAdFinished();
        FireBaseController.TriggerEvent(1, -1, -1);
    }

    static public void SetGameController(GameController gameController)
    {
        gameCont = gameController;
    }
}
