using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using System;

public class DatabaseManager :MonoBehaviour
{
    protected DatabaseManager() { }
    private string uri = "https://api.discountboxapp.com/api/user/v1/";
    //private string uri = "http://localhost:5020/api/user/v1/";
    public event Action OnRegisterFinished = delegate { };
    public event Action OnGetPointsFinished = delegate { };
    public event Action OnLoginFinished = delegate { };
    public event Action OnGetCityFinished = delegate { };
    public event Action OnAddPointsFinished = delegate { };
    public event Action OnUpdateCityFinished = delegate { };
    public event Action OnGetShopItemsFinished = delegate { };
    public event Action OnGetPlayerCouponsFinished = delegate { };
    public event Action OnBuyFromShopCompleate = delegate { };
    public event Action OnAddPlayerCouponFinished = delegate { };
    public event Action<bool> Waiting = delegate {};
    public event Action<string> Error = delegate { };
    public void GetCity(string lat, string lon, string lang)
    {
        StartCoroutine(GetRequest("https://api.bigdatacloud.net/data/reverse-geocode-client?latitude=" + lat + "&longitude=" + lon + "&localityLanguage=" + lang + ""));

    }
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();
            JsonData locationData;
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                locationData = JsonMapper.ToObject(webRequest.downloadHandler.text);
                print(locationData["principalSubdivision"].ToString());
                PlayerData.Instance.playerCity = locationData["principalSubdivision"].ToString();
                OnGetCityFinished();
            }
        }
    }
    protected void SendFinished()
    {
        OnRegisterFinished();
    }
    #region Register
    public void SendRegister(string mail, string pass, string gender)
    {

        StartCoroutine(Register(uri, mail, pass, gender));
    }
    IEnumerator Register(string uri, string mail, string pass, string gender)
    {
        WWWForm registerForm = new WWWForm();
        registerForm.AddField("Email", mail);
        registerForm.AddField("Password", pass);
        registerForm.AddField("Gender", gender);

        using (UnityWebRequest registerRequest = UnityWebRequest.Post(uri + "/Register", registerForm))
        {
            UnityWebRequest.ClearCookieCache();
            registerRequest.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));

            yield return registerRequest.SendWebRequest();
            while (!registerRequest.isDone)
            {
               
                Waiting(true);
            }
            if (registerRequest.isNetworkError || registerRequest.isHttpError)
            {
                print("Error:" + registerRequest.downloadHandler.text+"/n"+registerRequest.error);
                Error(registerRequest.downloadHandler.text);
            }
            else
            {
                var s = registerRequest.GetResponseHeaders()["Set-Cookie"];
                Debug.Log(s);
                var identityCookie = s.Substring(s.LastIndexOf(".AspNetCore.Identity.Application")).Split(';')[0];
                Debug.Log(identityCookie);
                print("Register Cevap:" + registerRequest.downloadHandler.text);
                PlayerPrefs.SetString("Cookie", identityCookie);
                SendFinished();

            }
        }
    }
    #endregion
    #region Login
    public void SendLogin(string mail, string pass)
    {
        StartCoroutine(Login(uri + "/Login", mail, pass));
    }
    IEnumerator Login(string uri, string mail, string pass)
    {
        if (!String.IsNullOrEmpty(PlayerPrefs.GetString("Cookie")))
        {
            UnityWebRequest loginRequest = UnityWebRequest.Get(uri);
            UnityWebRequest.ClearCookieCache();
            loginRequest.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            
            yield return loginRequest.SendWebRequest();
            if (loginRequest.isDone)
            {
                Waiting(false);
            }
            if (loginRequest.isNetworkError || loginRequest.isHttpError)
            {
                Debug.Log(loginRequest.error);
                Error(loginRequest.downloadHandler.text);
            }
            else
            {
                Debug.Log("Login complete!");
                OnLoginFinished();
            }
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("Email", mail);
            form.AddField("Password", pass);
            using (UnityWebRequest loginRequest = UnityWebRequest.Post(uri, form))
            {

                UnityWebRequest.ClearCookieCache();
                loginRequest.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
               
                yield return loginRequest.SendWebRequest();
             
                if (loginRequest.isDone)
                {
                    Waiting(false);
                }
                if (loginRequest.isNetworkError || loginRequest.isHttpError)
                {
                    print(loginRequest.error) ;
                    Error(loginRequest.downloadHandler.text);
                }
                else
                {
                    var s = loginRequest.GetResponseHeaders()["Set-Cookie"];
                    Debug.Log(s);
                    var identityCookie = s.Substring(s.LastIndexOf(".AspNetCore.Identity.Application")).Split(';')[0];
                    Debug.Log(identityCookie);
                    PlayerPrefs.SetString("Cookie", identityCookie);
                    OnLoginFinished();
                }
            }
        }

    }
    #endregion
    #region Logout
    public void SendLogout()
    {
        StartCoroutine(Logout("Logout"));
    }
    IEnumerator Logout(string methodName)
    {
        if (!String.IsNullOrEmpty(PlayerPrefs.GetString("Cookie")))
        {
            using (UnityWebRequest logoutRequest = UnityWebRequest.Get(uri + methodName))
            {
                UnityWebRequest.ClearCookieCache();
                logoutRequest.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
                yield return logoutRequest.SendWebRequest();
                if (logoutRequest.isNetworkError || logoutRequest.isHttpError)
                {
                    print(logoutRequest.error);

                }
                else
                {
                    PlayerPrefs.DeleteKey("Cookie");
                    SendFinished();
                }

            }

        }
    }
    #endregion
    #region Points
    public void SendGetPoints()
    {
        StartCoroutine(GetPoints());
    }
    IEnumerator GetPoints()
    {
        using (UnityWebRequest getPointsRequest = UnityWebRequest.Get(uri + "GetPoints"))
        {
            UnityWebRequest.ClearCookieCache();
            getPointsRequest.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            yield return getPointsRequest.SendWebRequest();
            if (getPointsRequest.isHttpError || getPointsRequest.isNetworkError)
            {

                print(getPointsRequest.error);
            }
            else
            {

                PlayerData.Instance.playerPoints =float.Parse(getPointsRequest.downloadHandler.text);
                OnGetPointsFinished();

            }

        }
    }
    public void SendAddPoints(int points)
    {
        StartCoroutine(AddPoints(points));
    }
    IEnumerator AddPoints(int points)
    {
        WWWForm form = new WWWForm();
        form.AddField("points", points.ToString());
        using (UnityWebRequest addPointsRequest = UnityWebRequest.Post(uri+"AddPoints",form))
        {
            UnityWebRequest.ClearCookieCache();
            addPointsRequest.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            yield return addPointsRequest.SendWebRequest();
            if (addPointsRequest.isHttpError||addPointsRequest.isNetworkError)
            {
                print(addPointsRequest.error);
            }
            else
            {
                print("Success");
                PlayerData.Instance.playerPoints += (float)points;
                OnAddPointsFinished();
            }
        }
    }
    #endregion
    #region PlayerData
    public void SendUpdateCity(string city)
    {
        StartCoroutine(UpdateCity(city));
    }
    IEnumerator UpdateCity(string city)
    {
        WWWForm form = new WWWForm();
        form.AddField("City", city);

        using (UnityWebRequest request = UnityWebRequest.Post(uri+ "UpdatePlayerLocation",form))
        {
            UnityWebRequest.ClearCookieCache();
            request.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            yield return request.SendWebRequest();
            if (request.isNetworkError||request.isHttpError)
            {
                print("update city error:" + request.error);
            }
            else
            {
                print("city update success");
                OnUpdateCityFinished();
            }
        }
    }
    public void SendBuyFromShop(string id)
    {
        StartCoroutine(BuyFromShop(id));
    }
    IEnumerator BuyFromShop(string couponId)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", couponId);
        using (UnityWebRequest request=UnityWebRequest.Post(uri+"BuyShopItem",form))
        {
            UnityWebRequest.ClearCookieCache();
            request.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            yield return request.SendWebRequest();
            if (request.isNetworkError||request.isHttpError)
            {
                print("buyError:" + request.error + "------" + request.downloadHandler.text);
                Error(request.downloadHandler.text+"-------"+request.error);
            }
            else
            {
                print(request.downloadHandler.text);
                OnBuyFromShopCompleate();
            }
        }
    }

    #endregion
    #region Coupons
    public void SendGetShopItems()
    {
        StartCoroutine(GetShopItems());
    }
    IEnumerator GetShopItems()
    {
        using (UnityWebRequest request=UnityWebRequest.Get(uri+ "GetShopItems"))
        {
            UnityWebRequest.ClearCookieCache();

            request.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            yield return request.SendWebRequest();
            if (request.isDone)
            {
                Waiting(false);
            }
            if (request.isHttpError||request.isNetworkError)
            {
                print(request.downloadHandler.text+"-----"+request.error);
            }
            else
            {
                List<ShopManager.ShopItem> items = new List<ShopManager.ShopItem>();
                var context = request.downloadHandler.text;
                JsonData itemData = JsonMapper.ToObject(context);
             
                if (itemData.IsArray)
                {
                    for (int i = 0; i < itemData.Count; i++)
                    {
                        ShopManager.ShopItem item = new ShopManager.ShopItem();
                        TextAsset binData = Resources.Load<TextAsset>("poi-icon-nightlife");
                        Texture2D texture = new Texture2D(1, 1);
                        texture.LoadImage(binData.bytes);
                        if (binData == null) continue;
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        item.companyName = (string)itemData[i]["name"];
                        item.companyLogo = sprite;
                        item.couponDescription = (string)itemData[i]["description"];
                        item.cost = (int)itemData[i]["price"];
                        item.stock = (int)itemData[i]["stock"];
                        item.couponCode = (string)itemData[i]["couponCode"]["code"];
                        item.id = (string)itemData[i]["id"] ;
                        items.Add(item);
                    }
                }
                PlayerData.Instance.shopItems = items;
                OnGetShopItemsFinished();

            }
        }
    }
    public void SendGetPlayerCoupons()
    {
        StartCoroutine(GetPlayerCoupons());
    }
    IEnumerator GetPlayerCoupons()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri + "GetShopItems?PlayerSpecific=true"))
        {
            UnityWebRequest.ClearCookieCache();
            print(request.url);
            request.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            yield return request.SendWebRequest();
            if (request.isDone)
            {
                Waiting(false);
            }
            if (request.isHttpError || request.isNetworkError)
            {
                print(request.downloadHandler.text + "-----" + request.error);
            }
            else
            {
                List<CouponsManager.Coupons> items = new List<CouponsManager.Coupons>();
                var context = request.downloadHandler.text;
                JsonData itemData = JsonMapper.ToObject(context);
                if (itemData.IsArray)
                {
                    for (int i = 0; i < itemData.Count; i++)
                    {
                        CouponsManager.Coupons item = new CouponsManager.Coupons();
                        TextAsset binData = Resources.Load<TextAsset>("poi-icon-nightlife");
                        Texture2D texture = new Texture2D(1, 1);
                        texture.LoadImage(binData.bytes);
                        if (binData == null) continue;
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        item.companyName = (string)itemData[i]["name"];
                        item.companyLogo = sprite;
                        item.couponDescription = (string)itemData[i]["description"];
                        item.cost = (int)itemData[i]["price"];
                        item.stock = (int)itemData[i]["stock"];
                        item.id = (string)itemData[i]["id"];
                        items.Add(item);
                    }
                }
                PlayerData.Instance.couponItems = items;
                OnGetPlayerCouponsFinished();

            }
        }
    }
    public void SendAddPlayerCoupons(string id)
    {
        StartCoroutine(AddPlayerCoupon(id));
    }
    IEnumerator AddPlayerCoupon(string couponId)
    {
        WWWForm form = new WWWForm();
        form.AddField("CouponId", couponId);
        using (UnityWebRequest request = UnityWebRequest.Post(uri + "AddPlayerCoupon", form))
        {
            UnityWebRequest.ClearCookieCache();
            request.SetRequestHeader("Cookie", PlayerPrefs.GetString("Cookie"));
            yield return request.SendWebRequest();

            if (request.isNetworkError||request.isHttpError)
            {
                print("addplayercoupon Error:" + request.error + "----------" + request.downloadHandler.text);
                Error(request.downloadHandler.text);
            }
            else
            {
                print("addplayercoupon Success: " + request.downloadHandler.text);
                OnAddPlayerCouponFinished();
            }
        }
    }
    #endregion
}
