using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CouponItemButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text companyNameLabel, couponDescription, stockNumber, costNumber;
    [SerializeField]
    private Image companyLogo;

    private CouponsManager.Coupons item;
    private CouponsManager shopManager;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void Setup(CouponsManager.Coupons Item, CouponsManager ShopManager)
    {
        item = Item;
        companyNameLabel.text = item.companyName;
        costNumber.text = "Tutar: " + item.cost.ToString() + "  Puan";
        stockNumber.text = "Stokta: "+item.stock.ToString()+"  Adet Var";
        couponDescription.text = item.couponDescription;
        companyLogo.sprite = item.companyLogo;

        shopManager = ShopManager;

    }
}
