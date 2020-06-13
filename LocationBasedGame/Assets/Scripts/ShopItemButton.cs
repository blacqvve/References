using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopItemButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text companyNameLabel, couponDescription, stockNumber,costNumber,Id;
    [SerializeField]
    private Image companyLogo;

    private ShopManager.ShopItem item;
    private ShopManager shopManager;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate {
            ShopManager Shop;
            Shop = GameObject.FindObjectOfType<ShopManager>();
            Shop.BuyFromShop(Id.text);

        });
    }
    public void Setup(ShopManager.ShopItem Item,ShopManager ShopManager)
    {
        item = Item;
        companyNameLabel.text = item.companyName;
        costNumber.text = "Tutar: "+item.cost.ToString()+"  Puan";
        stockNumber.text = item.stock.ToString();
        couponDescription.text = item.couponDescription;
        companyLogo.sprite = item.companyLogo;
        Id.text = item.id;
        shopManager = ShopManager;

    }
}
