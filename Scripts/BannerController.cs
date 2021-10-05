using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class BannerController : MonoBehaviour
{
    private BannerView bannerView;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void RequestBanner()
    {
        bannerView = RewardedAdController.banner;
        bannerView.Show();
    }
}
