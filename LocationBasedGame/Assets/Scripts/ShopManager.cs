using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public string companyName { get; set; }
        public string couponDescription { get; set; }
        public Sprite companyLogo { get; set; }
        public int stock { get; set; }
        public int cost { get; set; }
        public string id { get; set; }
        public string couponCode { get; set; }
    }
    [SerializeField]
    private Text pointsText;
    [SerializeField]
    private Transform contentPanel;
    [SerializeField]
    private GameObject subLoading, buyPopup;
    List<ShopItem> items = new List<ShopItem>();
    public SimpleObjectPool buttonObjectPool;
    string comingId;
    DatabaseManager databaseManager;
    // Start is called before the first frame update
    private void Awake()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
    }
    void OnEnable()
    {

        subLoading.SetActive(true);
        databaseManager.SendGetShopItems();
        databaseManager.OnGetShopItemsFinished += Finished;
        items = PlayerData.Instance.shopItems;
        pointsText.text = "Puanınız = " + PlayerData.Instance.playerPoints;

    }

    public void BuyFromShop(string id)
    {
        if (!System.String.IsNullOrEmpty(id))
        {
            comingId = id;
            buyPopup.GetComponentInChildren<Text>().text = "Bu kuponu satın almak istediğinize emin misiniz?";
            buyPopup.SetActive(true);
        }
    }
    public void BuyConfirm(string choise)
    {
        if (choise == "Yes")
        {
            subLoading.SetActive(true);
            databaseManager.SendBuyFromShop(comingId);
            databaseManager.Error += Instance_Error;
            databaseManager.OnBuyFromShopCompleate += Instance_OnBuyFromShopCompleate;
        }
        else if (choise == "No")
        {
            buyPopup.SetActive(false);
            comingId = null;
        }
    }

    private void Instance_OnBuyFromShopCompleate()
    {
        subLoading.SetActive(false);
        buyPopup.GetComponentInChildren<Text>().text = "Satın alma başarılı. Satın alınan kuponu 'Kuponlarım' bölümünden görüntüleyebilirsiniz.";
        Button[] popButtons = buyPopup.GetComponentsInChildren<Button>();
        foreach (var item in popButtons)
        {
            item.gameObject.SetActive(false);
        }
        StartCoroutine(ClosePopup());
    }
    IEnumerator ClosePopup()
    {
        yield return new WaitForSecondsRealtime(3f);
        buyPopup.SetActive(false);
    }
    private void Instance_Error(string obj)
    {
        subLoading.SetActive(false);
        print("Buy error:" + obj);
    }

    private void Finished()
    {
        subLoading.SetActive(false);
        items = PlayerData.Instance.shopItems;
        RefreshDisplay();
    }

    private void Start()
    {

        RefreshDisplay();
    }
    public void RefreshDisplay()
    {
        AddButtons();
    }
    private void Update()
    {
        pointsText.text = "Puanınız = " + PlayerData.Instance.playerPoints;
    }
    void AddButtons()
    {
        for (int i = 0; i < items.Count; i++)
        {
            ShopItem item = items[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            ShopItemButton sampleButton = newButton.GetComponent<ShopItemButton>();

            sampleButton.Setup(item, this);
        }
    }
}
