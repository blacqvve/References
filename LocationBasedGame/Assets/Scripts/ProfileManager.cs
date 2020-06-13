using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject modalWindow, mainMenu;
    bool isSend = false;
    DatabaseManager databaseManager;


    private void Awake()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
    }
    public void Logout()
    {
        modalWindow.SetActive(true);
        var modalText=modalWindow.GetComponentInChildren<Text>();
        modalText.text = "Kullanıcı hesabınızdan çıkış yapmak istiyor musunuz?";
    }
    public void ConfirmLogout(string choise)
    {
        if (choise=="Yes")
        {
            databaseManager.SendLogout();
            databaseManager.OnRegisterFinished += LogoutFinished;
        }
        else if(choise=="No")
        {
            modalWindow.SetActive(false);
        }
    }

    private void LogoutFinished()
    {
        if (String.IsNullOrEmpty(PlayerPrefs.GetString("Cookie")))
        {
            PlayerData.Instance.playerCookie = PlayerPrefs.GetString("Cookie");
            StartCoroutine(LoadScene());
        }
    }
    IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("MainMenu");
        while (!op.isDone)
        {
            print(op.progress);
            yield return null;
        }
    }

    private void OnEnable()
    {
        databaseManager.SendGetPoints();
        databaseManager.OnGetPointsFinished += PrintPoints;
        //StartCoroutine(Start());
    }

    private void PrintPoints()
    {
        print(PlayerData.Instance.playerPoints);
        print(PlayerData.Instance.playerCity);
        isSend = true;
    }

    private void Instance_OnUpdateCityFinished()
    {
        print("city updated");
    }

    //IEnumerator Start()
    //{
       
    //    while (!Input.location.isEnabledByUser)
    //    {
    //        print("gps not active");
    //        yield return new WaitForSeconds(1f);
    //    }
    //    Input.location.Start(1, 0.1f);
    //    int maxWait = 20;
    //    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        maxWait--;
    //    }

    //    if (Input.location.status != LocationServiceStatus.Running && maxWait > 0)
    //    {
    //        yield return new WaitForSeconds(1f);
    //        maxWait--;
    //    }

    //    if (maxWait < 1)
    //    {
    //        print("gps hatası");
    //        yield break;
    //    }
    //    if (Input.location.status == LocationServiceStatus.Failed)
    //    {
    //        print("konum bulma başarısız oldu");
    //        yield break;
    //    }
    //    else
    //    {
    //        print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);


    //        if (Input.location.isEnabledByUser&&isSend==false)
    //        {
    //            databaseManager.GetCity(Input.location.lastData.latitude.ToString().Replace(",", "."), Input.location.lastData.longitude.ToString().Replace(",", "."), "tr");
    //            databaseManager.OnGetCityFinished += PrintPoints;
    //            yield break;
    //        }

    //        yield return StartCoroutine(Start());
    //        yield break;
    //    }
    //}
    public void AddPoints()
    {
        databaseManager.SendAddPoints(100);
        databaseManager.OnAddPointsFinished += Instance_OnAddPointsFinished;
    }

    private void Instance_OnAddPointsFinished()
    {
        print(PlayerData.Instance.playerPoints);
    }
}
