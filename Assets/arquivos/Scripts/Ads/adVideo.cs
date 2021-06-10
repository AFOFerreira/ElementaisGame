using UnityEngine.Events;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

//Camila Silva
//Video com Banner popup
public class adVideo : MonoBehaviour
{
     private RewardedAd rewardedAd;//Video
     private InterstitialAd InterstitialAd;//popUp
    private string APP_ID = "ca-app-pub-3119375714600951~1420686511";//Id do aplicativo


    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {
        MobileAds.Initialize(initStatus => {});

        bannerPopUp();//inicializar o banner
        Video();//inicializar o video
    }

  //Eventos para o video
  public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        //display_Video();
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
       
       Display_InterstitialAD();//mostrar o banner quando o video fechar
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardedAdRewarded event received for "
                        + amount.ToString() + " " + type);
    }

    //função para mostrar o video
    public void display_Video()
    {
        if (this.rewardedAd.IsLoaded()) {
            this.rewardedAd.Show();
    }
}

    //mostrar o banner popUp
    public void Display_InterstitialAD(){
		if(InterstitialAd.IsLoaded()){//Se o video for carregado

			InterstitialAd.Show();

		}
	}

    //Função para chamar o video
    public void Video(){
        
        string adUnitId;
        #if UNITY_ANDROID
            adUnitId = "ca-app-pub-3119375714600951/3562200904";//id do video
        #elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
        #else
            adUnitId = "unexpected_platform";
        #endif

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        //this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    //função para chamar o banner PopUp
    public void bannerPopUp()
    {
       //Id do video criado no Admob
		string interstitial_ID = "ca-app-pub-3119375714600951/8239812511";//id do banner popup
		
		InterstitialAd = new InterstitialAd(interstitial_ID);

		//Para o real 
		AdRequest adRequest = new AdRequest.Builder().Build();

		//Para teste
		//AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();

		//carregar video 
		InterstitialAd.LoadAd(adRequest);

    }

}
