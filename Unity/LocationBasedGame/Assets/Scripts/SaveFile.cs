using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System;
using Random = System.Random;

public class SaveFile : MonoBehaviour
{
    public static SaveFile current;
    public PlayerInfo playerInfo;
    public Player player = new Player("Player");
    public bool AutoSave = true;
    public bool darkMode = false;
    private string file = "/savedata.json";
    private string json = "playerInfo";
    private JsonData playerData;
    private JsonData playerSaveJson;
    private float timer = 0f;
    private float interval = 3600f; //in seconds

    void Awake()
    {
        current = this;
        ReadShopItems();
    }
    void Start()
    {

        file = Application.persistentDataPath + file;

    }

    void Update()
    {
        if (AutoSave)
        {
            if (timer < Time.time)
            {
                timer = Time.time + interval;
                WriteJson();
            }
        }
    }

    public void SaveToJson()
    {
        //playerInfo.Prepare();
        string json = JsonUtility.ToJson(playerInfo);

        try
        {
            File.WriteAllText(file, json);
            Log(file + " has been saved!");
        }
        catch (System.Exception e)
        {
            Log(e.ToString());
        }
    }
    public void ReadJson()
    {
        TextAsset file = Resources.Load(json) as TextAsset;
        string content = file.ToString();
        playerData = JsonMapper.ToObject(content);

        if (playerData != null)
        {
            print("veri alımı başladı");
            player.openedBox = (int)playerData["playerInfo"]["openedBox"];
            player.name = (string)playerData["playerInfo"]["name"];
            player.catchSceneCount = (int)playerData["playerInfo"]["catchSceneCount"];
            player.spawnedAround = (int)playerData["playerInfo"]["spawnedAround"];
            player.points = (int)playerData["playerInfo"]["points"];
            print("veri alımı tamamlandı");
            print(player.openedBox + player.name);
        }
    }
    public List<ShopManager.ShopItem> ReadShopItems()
    {


        TextAsset file = Resources.Load(json) as TextAsset;
        string content = file.ToString();
        print(content);
        List<ShopManager.ShopItem> items = new List<ShopManager.ShopItem>();
        JsonData itemData = JsonMapper.ToObject(content);
        if (itemData["shopItems"].IsArray)
        {
            for (int i = 0; i < itemData["shopItems"].Count; i++)
            {
                ShopManager.ShopItem item = new ShopManager.ShopItem();
                
                TextAsset binData = Resources.Load<TextAsset>((string)itemData["shopItems"][i]["companyLogo"]);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(binData.bytes);
                if (binData == null) continue;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                item.companyName = (string)itemData["shopItems"][i]["companyName"];
                item.companyLogo = sprite;
                item.couponDescription = (string)itemData["shopItems"][i]["couponDescription"];
                item.cost = (int)itemData["shopItems"][i]["cost"];
                item.stock = (int)itemData["shopItems"][i]["stock"];
                item.id = (string)itemData["shopItems"][i]["id"];
                items.Add(item);
            }
        }
        player.itemList = items;
        return items;
   
    }
    public void WriteJson()
    {
        string path = Path.Combine(Application.streamingAssetsPath, json + ".json");
        playerSaveJson = JsonUtility.ToJson(player);
        print(Application.dataPath);
        if (Application.platform==RuntimePlatform.Android)
        {
            File.WriteAllText(path, playerSaveJson.ToString()); 
        }
        else
        {
            File.WriteAllText(Application.dataPath+"/Resources/" + json + ".json", playerSaveJson.ToString());
        }
    }

    public void ReadFromJson()
    {
        string f = File.ReadAllText(file);
        f.Trim();

        playerInfo = JsonUtility.FromJson<PlayerInfo>(f);
        playerInfo.Unzip();

        Log(file + " has been loaded!");
        Log("Contents: \n" + JsonUtility.ToJson(playerInfo));
    }

    string GetJsonFileLocation()
    {
        return file;
    }

    void Log(string t)
    {
        print(t);
    }
}
public class Player
{
    public string name;
    public int points;
    public int openedBox;
    public int catchSceneCount;
    public int spawnedAround;
    public List<ShopManager.ShopItem> itemList = new List<ShopManager.ShopItem>();
    public List<CouponsManager.Coupons> couponList = new List<CouponsManager.Coupons>();

    public Player(string name, int points = 0, int openedBox = 0, int catchSceneCount = 0, int spawnedAround = 0)
    {
        this.name = name;
        this.points = points;
        this.openedBox = openedBox;
        this.catchSceneCount = catchSceneCount;
        this.spawnedAround = spawnedAround;
    }
    public void AddPoints(int point)
    {
        points += point;

    }
    public void AddCoupons(int count)
    {
       
        for (int i = 0; i < count; i++)
        {
            CouponsManager.Coupons coupon = new CouponsManager.Coupons();
            TextAsset binData = Resources.Load<TextAsset>("poi-icon-outdoors");
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(binData.bytes);
            if (binData == null) continue;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            coupon.companyName = "Coupon  " + i;
            coupon.couponDescription = "Description  " + i;
            coupon.id = "id"+i;
            coupon.stock = 1;
            coupon.cost = 0;
            coupon.companyLogo = sprite;
            couponList.Add(coupon);
            
        }
    }

}
public class PlayerInfo
{
    private List<PokemonInfo> pokemons;
    public string name;
    public int point;
    public float experience;
    public float expNeeded;
    public string pstring;
    private Random random = new Random();



    public PlayerInfo(string name, int point = 0, float experience = 0)
    {
        this.name = name;

        this.point = point;
        this.experience = experience;

        this.expNeeded = (this.point + 1) * 50f;
        this.pokemons = new List<PokemonInfo>();
    }

    public void Prepare()
    {
        pstring = "";
        PokemonInfo[] p = this.pokemons.ToArray();

        for (int i = 0; i < p.Length; i++)
        {
            pstring += "/" + JsonUtility.ToJson(p[i]);
        }
    }

    public void Unzip()
    {
        string[] s = pstring.Split('/');
        this.pokemons = new List<PokemonInfo>();

        for (int i = 1; i < s.Length; i++)
        {
            PokemonInfo p = JsonUtility.FromJson<PokemonInfo>(s[i]);
            this.pokemons.Add(p);
        }
    }

    public void AddPokemon(PokemonType type, int point, int cookies, float experience, float expNeeded)
    {
        PokemonInfo p = new PokemonInfo(type, point, cookies, experience, expNeeded);
        PokemonInfo r = this.GetPokemonByType(type);
        if (r == null)
        {
            this.pokemons.Add(p);
        }
        else
        {
            r.AddCookies(cookies + 1);
            if (r.point < point)
            {
                r.point = point;
            }
        }

        this.Prepare();
    }

    public PokemonInfo GetPokemonByType(PokemonType type)
    {
        PokemonInfo[] p = this.pokemons.ToArray();

        for (int i = 0; i < p.Length; i++)
        {
            if (p[i].type == type)
            {
                return p[i];
            }
        }

        return null;
    }
}

public class PokemonInfo
{
    public PokemonType type;
    public int point;
    public int cookies;
    public float experience;
    public float expNeeded;

    public PokemonInfo(PokemonType type, int point, int cookies, float experience, float expNeeded)
    {
        this.type = type;
        this.point = point;
        this.cookies = cookies;
        this.experience = experience;
        this.expNeeded = (this.point + 1) * 50f;
    }

    public void AddExperience(float exp)
    {
        experience += exp;
        if (experience > expNeeded)
        {
            float rest = experience - expNeeded;
            experience = rest;
            point++;
        }
    }

    public void AddCookies(int cookies)
    {
        this.cookies += cookies;
    }
}
