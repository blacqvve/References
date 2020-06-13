using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUI : MonoBehaviour
{
    [SerializeField]
    private Text clock, points;
    [SerializeField]
    private Image batteryProgress;
    [SerializeField]
    private Material morningMaterial, nightMaterial;
    [SerializeField]
    private GameObject profilePanel, shopPanel, couponsPanel;
    [SerializeField]
    private GameObject[] backButtons;
    private GameObject activePanel;
    ILocationProvider _locationProvider;
    DatabaseManager databaseManager;
    ILocationProvider LocationProvider
    {
        get
        {
            if (_locationProvider == null)
            {
                _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            }

            return _locationProvider;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
        SetSkybox();
    }
    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            foreach (var item in backButtons)
            {
                item.SetActive(false);
            }
        }
        batteryProgress.fillAmount = 0;
        SetPoints();
        InvokeRepeating("SetPoints", 1, 15f);
    }
    private void CheckLocation()
    {
        var map = LocationProviderFactory.Instance.mapManager;
        print(_locationProvider.CurrentLocation.LatitudeLongitude);
        string lat = Input.location.lastData.latitude.ToString();
        string lon = Input.location.lastData.longitude.ToString();
       databaseManager.GetCity(lat, lon, "tr");
    }
    // Update is called once per frame
    void LateUpdate()
    {
        SetClock();
        SetBattery(SystemInfo.batteryLevel);
        BackButtonControl();
        //InvokeRepeating("CheckLocation", 2f, 15f);
    }

    private void SetPoints()
    {
       databaseManager.SendGetPoints();
       databaseManager.OnGetPointsFinished += Instance_OnGetPointsFinished;

    }

    private void Instance_OnGetPointsFinished()
    {
        points.text = PlayerData.Instance.playerPoints.ToString();
        print(points.text);
    }

    private void SetBattery(float batteryLevel)
    {
        float batLevel = batteryLevel * 100f;
        batteryProgress.fillAmount = batteryLevel;

        //batteryProgress.fillAmount = SystemInfo.batteryLevel ;

    }
    private void SetSkybox()
    {
        if (System.DateTime.Now.Hour >= 19)
        {
            RenderSettings.skybox = nightMaterial;
        }
        else if (System.DateTime.Now.Hour <= 19)
        {
            RenderSettings.skybox = morningMaterial;
        }
    }

    private void SetClock()
    {
        string time = DateTime.Now.ToShortTimeString();
        clock.text = time;
    }
    public void IosBackButtonControl()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i).gameObject;
            if (child.activeSelf == true)
            {
                activePanel = child;
            }
        }
        switch (activePanel.name)
        {
            case "Shop":
                shopPanel.SetActive(false);
                profilePanel.SetActive(true);
                break;
            case "Profile":
                //TODO: Çýkýþ Yapmak istiyormusunuz prompt buraya      
                profilePanel.SetActive(false);
                break;
            case "Coupons":
                activePanel.SetActive(false);
                profilePanel.SetActive(true);
                break;
            default:

                break;
        }
    }
    public void ChangePanel(GameObject panel)
    {
        switch (panel.name)
        {

            case "Shop":
                profilePanel.SetActive(false);
                panel.SetActive(true);
                break;
            case "Coupons":
                profilePanel.SetActive(false);
                panel.SetActive(true);
                break;
            default:
                break;
        }
    }
    void BackButtonControl()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {


                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    var child = this.transform.GetChild(i).gameObject;
                    if (child.activeSelf == true)
                    {
                        activePanel = child;
                    }
                }
                switch (activePanel.name)
                {
                    case "Shop":
                        shopPanel.SetActive(false);
                        profilePanel.SetActive(true);
                        break;
                    case "Profile":
                        //TODO: Çýkýþ Yapmak istiyormusunuz prompt buraya      
                        profilePanel.SetActive(false);
                        break;
                    case "Coupons":
                        activePanel.SetActive(false);
                        profilePanel.SetActive(true);
                        break;
                    default:

                        break;
                }
            }
        }
    }
}
