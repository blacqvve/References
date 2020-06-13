using UnityEngine;
using System;
using GoogleMobileAds.Api;


public class rewardedvideo : MonoBehaviour
{
    
    private RewardBasedVideoAd reklamObjesi;
    DatabaseManager databaseManager;


    private void Awake()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
    }
    void Start()
    {
        MobileAds.Initialize(reklamDurumu => { });

        reklamObjesi = RewardBasedVideoAd.Instance;
        reklamObjesi.OnAdClosed -= YeniReklamAl;
        reklamObjesi.OnAdClosed += YeniReklamAl; // Kullanıcı reklamı kapattıktan sonra çağrılır

        YeniReklamAl(null, null);
    }

    // Ekranda test amaçlı "Reklamı Göster" butonu göstermeye yarar, bu fonksiyonu silerseniz buton yok olur
    public void Show()
    {
        
            reklamObjesi.OnAdRewarded -= OyuncuyuOdullendir;
            reklamObjesi.OnAdRewarded += OyuncuyuOdullendir; // Kullanıcı reklamı tamamen izledikten sonra çağrılır

            reklamObjesi.Show();

    }

    public void YeniReklamAl(object sender, EventArgs args)
    {
        AdRequest reklamIstegi = new AdRequest.Builder().Build();
        reklamObjesi.LoadAd(reklamIstegi, "ca-app-pub-3940256099942544/5224354917");
    }

    private void OyuncuyuOdullendir(object sender, Reward odul)
    {
        reklamObjesi.OnAdRewarded -= OyuncuyuOdullendir;

        Debug.Log("Ödül türü: " + odul.Type);
       databaseManager.SendAddPoints((int)odul.Amount);
     
       databaseManager.Error += Instance_Error;
        
    }

    private void Instance_Error(string obj)
    {
      print(obj);
    }


}