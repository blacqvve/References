using UnityEngine;
using System.Collections;
using System;
using GoogleMobileAds.Api;

public class reklamdeneme : MonoBehaviour
{
    private InterstitialAd reklamObjesi;

    void Start()
    {
        MobileAds.Initialize(reklamDurumu => { });
        YeniReklamAl(null, null);
    }

    // Ekranda test amaçlı "Reklamı Göster" butonu göstermeye yarar, bu fonksiyonu silerseniz buton yok olur
    void Update()
    {

            StartCoroutine(ReklamiGoster());
       
    }

    IEnumerator ReklamiGoster()
    {
        while (!reklamObjesi.IsLoaded())
            yield return null;

        reklamObjesi.Show();
    }

    public void YeniReklamAl(object sender, EventArgs args)
    {
        if (reklamObjesi != null)
            reklamObjesi.Destroy();

        reklamObjesi = new InterstitialAd("ca-app-pub-3940256099942544/1033173712");
        reklamObjesi.OnAdClosed += YeniReklamAl; // Kullanıcı reklamı kapattıktan sonra çağrılır

        AdRequest reklamIstegi = new AdRequest.Builder().Build();
        reklamObjesi.LoadAd(reklamIstegi);
    }

    void OnDestroy()
    {
        if (reklamObjesi != null)
            reklamObjesi.Destroy();
    }
}