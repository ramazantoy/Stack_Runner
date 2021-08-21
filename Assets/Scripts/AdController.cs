using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdController : MonoBehaviour
{
    public static AdController Current;
    public BannerView bannerView;//banner reklam
   public  InterstitialAd interstitial;//geçiş reklamı
    private float _interstitialAdtime=60;//belli zaman aralıklarında interstitial reklam üretmek amacıyla
   public RewardedAd rewardedAd;
    public void InitializeAds()//reklamları oluşturmak amacıyla bir fonksiyon
    {
        MobileAds.Initialize(initStatus => { });

        this.RequestBanner();//banner reklam isteme
        this.RequestInterstitial();//geçiş reklamı isteme
        this.RequestRewardedAdRequest();//video reklam için istek
        Current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (_interstitialAdtime > 0)//60 saniyeyi hesaplamak amacıyla
        {
            _interstitialAdtime -= Time.deltaTime;
        }
    }
    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);//aşağı orta kısım

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }
    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        /*
        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;  //reklam yüklediğinde çalışan fonksiyon
        */
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        /* 
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;// uygulama kapandığında çalışan fonksiyon
        */
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }
    public bool IsReadyInterstitialAd()
    {
        if(_interstitialAdtime<0 && interstitial.IsLoaded())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HandleOnAdClosed(object sender, EventArgs e)//reklam kapatıldığında
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        RequestInterstitial();
        _interstitialAdtime = 60;//reklam kapatıldığında zamanlayıcının ayarlanması
    }

    private void HandleOnAdOpened(object sender, EventArgs e)//reklam açıldığında
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<AudioListener>().enabled = false;
    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)//Reklam yüklenmesi başarısız olursa
    {
        RequestInterstitial();
    }
    public void RequestRewardedAdRequest()//video reklam yükleme isteği
    {
        string adUnitId;
#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif

        this.rewardedAd = new RewardedAd(adUnitId);
        /*
        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;*/
        // Called when an ad request failed to load.
        /*
        // Called when an ad request failed to show.
     this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;*/
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;

        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);

    }

    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        RequestRewardedAdRequest();//reklam isteme
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        LevelController.Current.RewardAdVideoButton.gameObject.SetActive(false);//ödül butonunun kapanması
        LevelController.Current.giveMoney(LevelController.Current.score);
    }

    private void HandleRewardedAdOpening(object sender, EventArgs e)
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<AudioListener>().enabled = false;
    }

    private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestRewardedAdRequest();
    }
}
