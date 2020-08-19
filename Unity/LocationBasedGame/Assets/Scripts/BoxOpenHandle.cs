using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxOpenHandle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] particleSystems;
    [SerializeField]
    private Text companyName, couponDescription, couponCode;
    [SerializeField]
    private Image couponLogo;
    [SerializeField]
    private GameObject subLoading,sellButton,popup;
    int point = 0;
    DatabaseManager databaseManager;
  
    string catchedCouponId = null;
    // Start is called before the first frame update
    private System.Random random = new System.Random();

    private void Awake()
    {
        databaseManager = FindObjectOfType<DatabaseManager>() ;
    }
    void OnEnable()
    {
        GameObject paket = GameObject.FindGameObjectWithTag("Pokemon");
        foreach (var item in particleSystems)
        {
            var em = item.emission;
            em.enabled = false;
            StartCoroutine(CatchingPhase(paket));
            System.Random random = new System.Random();
            point = random.Next(100, 500);
            couponCode.text = point.ToString();
        }
    }
    private void Update()
    {
        if (Input.touchCount>0)
        {
            subLoading.SetActive(true);
            if (catchedCouponId!=null)
            {
                databaseManager.SendAddPlayerCoupons(catchedCouponId);
                databaseManager.Error += Instance_Error;
                databaseManager.OnAddPlayerCouponFinished += Instance_OnAddPlayerCouponFinished;
            }
            else
            {
                subLoading.SetActive(true);
                databaseManager.SendAddPoints(point);
                databaseManager.OnAddPointsFinished += Instance_OnAddPointsFinished;
                databaseManager.Error += PointError ;
            }
           
        }
    }

   

    private void Instance_OnAddPlayerCouponFinished()
    {
        subLoading.SetActive(false);
        gameObject.SetActive(false);
        PokemonSpawner.Run();
    }

    private void Instance_Error(string obj)
    {
       //TODO:Error düzenlenecek
    }
    public void OpenSellPopup()
    {
      
        popup.GetComponentInChildren<Text>().text = "Kazandýðýnýz kuponu " + point + " puana çevirmek istediðinize emin misiniz?";
        popup.SetActive(true);
    }
    public void SellConfirm(bool choise)
    {
        if (choise==true)
        {
            subLoading.SetActive(true);
         
            databaseManager.SendAddPoints(point);
            databaseManager.Error += PointError;
            databaseManager.OnAddPointsFinished += Instance_OnAddPointsFinished;
        }
        else
        {
            popup.SetActive(false);
        }
    }

    private void Instance_OnAddPointsFinished()
    {
        subLoading.SetActive(false);
        popup.GetComponentInChildren<Text>().text = "Kuponu puana çevirme iþlemi baþarýlý." + point + " puan hesabýnýza eklenmiþtir.3 saniye sonra haritaya otomatik yönlendirileceksiniz";
        Button[] buttons = popup.GetComponentsInChildren<Button>();
        foreach (var item in buttons)
        {
            item.gameObject.SetActive(false);
        }
        PokemonSpawner.Run();
    }

    private void PointError(string obj)
    {
        Debug.LogWarning(obj);
    }

    public void EnableParticles()
    {
        foreach (var item in particleSystems)
        {
            var em = item.emission;
            em.enabled = true;
        }
    }
   public  IEnumerator CatchingPhase(GameObject pokemon)
    {
        
        pokemon.SetActive(false);
        print("Panel Active");
        Debug.Log("START");
        bool caught = (UnityEngine.Random.Range(0, 1) < 0.5f);
        int point = random.Next(5, 100);
      
        if (caught)
        {
            System.Random random = new System.Random();
            int zar = random.Next(1, 6);
            if (zar==3)
            {
                
                databaseManager.SendGetShopItems();
                databaseManager.OnGetShopItemsFinished += Instance_OnGetShopItemsFinished;
            }
            else
            {
                sellButton.SetActive(false);

                databaseManager.SendAddPoints(point);
                databaseManager.Error += PointError;
                databaseManager.OnAddPointsFinished += Instance_OnAddPointsFinished;
            }
          
            yield break;
        }
        else
        {
            Debug.Log(pokemon + "has escaped!");
        }
        pokemon.SetActive(true);
        yield break;
    }

    private void Instance_OnGetShopItemsFinished()
    {
        if (PlayerData.Instance.shopItems!=null)
        {
            System.Random random = new System.Random();
            ShopManager.ShopItem selectedItem = new ShopManager.ShopItem();
            int rnd = random.Next(0, PlayerData.Instance.shopItems.Count);
            selectedItem = PlayerData.Instance.shopItems[rnd];
            if (selectedItem != null)
            {
                couponCode.text = selectedItem.couponCode;
                couponDescription.text = selectedItem.couponDescription;
                couponLogo.sprite = selectedItem.companyLogo;
                companyName.text = selectedItem.companyName;
                catchedCouponId = selectedItem.id;
            }
        }
      
    }
}
