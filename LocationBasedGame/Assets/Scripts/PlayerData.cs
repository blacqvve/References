using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData>
{
    public float playerPoints;
    public string playerCookie;
    public string playerCity;
    public int playerOpenedBox;
    public int playerSpawnedBox;
    public int playerCatchSceneCount;
    public List<ShopManager.ShopItem> shopItems = new List<ShopManager.ShopItem>();
    public List<CouponsManager.Coupons> couponItems = new List<CouponsManager.Coupons>();

}
