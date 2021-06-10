using UnityEngine;
using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

//script para a criação de Ads no Elementais, utilizando o Admob com o Unity 
public class AdMobManager : MonoBehaviour
{
    private RewardedAd rewardedAd;//Video
    private InterstitialAd InterstitialAd;//popUp
    //Referencia para o banner
    private BannerView bannerView;
    //referencia para o video
    //private RewardBasedVideoAd rewardVideoAd;

    //ID do Aplicativo no Admob
    private string APP_ID = "ca-app-pub-3119375714600951~1420686511";
    string bannerTestId = "ca-app-pub-3940256099942544/6300978111";
    string videoTestId = "ca-app-pub-3940256099942544/5224354917";

    //// criar instancia
    // public static AdMobManager instance;

    // Start is called before the first frame update
    void Start()
    {
        //inicializar o ID do aplicativo criado no Admob
        MobileAds.Initialize(initStatus => { });
        bannerPopUp();//inicializar o banner
        Video();//inicializar o video
    }

    //função para chamar o banner
    public void RequestBanner()
    {
        //Id do banner criado no Admob
         string banner_ID = "ca-app-pub-3119375714600951/6401835867";
        //Id do banner para teste
        //string banner_ID = "ca-app-pub-3940256099942544/6300978111";
        //Criar um banner de 320x50 no topo da tela

        AdSize adSize = new AdSize(300,50);
        bannerView = new BannerView(banner_ID, adSize, AdPosition.Bottom);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        //this.bannerView.On += this.HandleOnAdLeavingApplication;
        //Para o real 
         AdRequest adRequest = new AdRequest.Builder().Build();
        //Para teste
        //AdRequest adRequest = new AdRequest.Builder().AddTestDevice("33BE2250B43518CCDA7DE426D04EE231").Build();
        //carregar o banner
        bannerView.LoadAd(adRequest);
    }
    private void HandleRewardBasedVideoLoaded(object sender, EventArgs e)
    {

    }

    //função para mostrar o banner na tela
    public void Display_banner()
    {
        bannerView.Show();
    }

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
        if (this.rewardedAd.IsLoaded())
        {
            this.rewardedAd.Show();
        }
    }

    //mostrar o banner popUp
    public void Display_InterstitialAD()
    {
        if (InterstitialAd.IsLoaded())
        {//Se o video for carregado
            InterstitialAd.Show();
        }
    }

    //Função para chamar o video
    public void Video()
    {

        string adUnitId;
#if UNITY_ANDROID
         adUnitId = "ca-app-pub-3119375714600951/3562200904";//id do video
       // adUnitId = "ca-app-pub-3940256099942544/5224354917";//id para teste
#elif UNITY_IPHONE
            adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            adUnitId = "unexpected_platform";
#endif


        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToLoad;
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
        //string interstitial_ID = "ca-app-pub-3940256099942544/1033173712";//id do banner popup para teste

        InterstitialAd = new InterstitialAd(interstitial_ID);

        //Para o real 
        AdRequest adRequest = new AdRequest.Builder().Build();

        //Para teste
        //AdRequest adRequest = new AdRequest.Builder().AddTestDevice("2077ef9a63d2b398840261c8221a0c9b").Build();

        //carregar video 
        InterstitialAd.LoadAd(adRequest);

    }

    //Handle events
    //função para quando carregar 
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Display_banner();
    }
    //função para quando falhar o carregamento 
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        RequestBanner();
    }
    //Ad foi aberto
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
    }
    //ad foi fechado
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
    }
    //Ad deixou a aplicação
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    //Função que destroi o banner da tela
    public void DestroyBanner(){
        bannerView.Destroy();
    }
}
