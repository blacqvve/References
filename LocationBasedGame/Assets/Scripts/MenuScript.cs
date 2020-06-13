using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using System;
using Mapbox.Unity;

public class MenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject loginPanel, registerPanel, profilePanel, menu, shopPanel, couponsPanel;
    [SerializeField]
    private Button loginButton, registerButton, mainButton, playButton, shopButton, couponsButton;
    GameObject dialog = null;
    Player player = new Player("player");
    private bool escapePressedOnce = false;
    [SerializeField]
    private GameObject loadingScreen;
    [SerializeField]
    private GameObject popupWindow;
    //scene load action
    readonly Action load = () => { SceneManager.LoadScene("World"); };
    [SerializeField]
    private GameObject[] backButtons;
    //Geri dönüş için aktif panel
    private GameObject activePanel;

    [SerializeField]
    private Font font;
    private void Awake()
    {

        MapboxAccess.Instance.ClearAllCacheFiles();
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i).gameObject;
            child.SetActive(false);
        }
        menu.SetActive(true);
    }
    public void ClearCookie()
    {
        PlayerPrefs.DeleteKey("Cookie");
        print("login Cookie Silindi");
    }
    void Start()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {

            foreach (var item in backButtons)
            {
                item.SetActive(false);
            }
        }
        playButton.GetComponent<Animator>().speed = 0;
        loginButton.onClick.AddListener(delegate { ChangeMenu(loginPanel); });
        registerButton.onClick.AddListener(delegate { ChangeMenu(registerPanel); });
        shopButton.onClick.AddListener(delegate { ChangeMenu(shopPanel); });
        couponsButton.onClick.AddListener(delegate { ChangeMenu(couponsPanel); });
        playButton.onClick.AddListener(delegate { StartCoroutine(LoadScene()); });
       

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) || !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            string[] perms = new string[2];
            perms[0] = Permission.ExternalStorageWrite;
            perms[1] = Permission.ExternalStorageRead;

            for (int i = 0; i < perms.Length; i++)
            {
                Permission.RequestUserPermission(perms[i]);
                print(perms[i] + Permission.HasUserAuthorizedPermission(perms[i]));
            }
        }
#endif
    }
    private void Perms()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                StartCoroutine(LoadScene());
            }
            else
            {
                dialog = new GameObject();
            }


        }
        else
        {
            StartCoroutine(LoadScene());
        }
#endif
    }
    IEnumerator LoadScene()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            print(Permission.HasUserAuthorizedPermission(Permission.FineLocation));

            //loadingScreen.SetActive(true);
            //loadingIcon.GetComponent<Animator>().Play("Loading");


            AsyncOperation async = SceneManager.LoadSceneAsync("Test");

            while (!async.isDone)
            {
                print(async.progress);
                yield return null;
            }
        }
        else
        {
            Perms();
        }
    }
//    void OnGUI()
//    {
//#if PLATFORM_ANDROID
//        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
//        {

//            dialog.AddComponent<PermissionsRationaleDialog>();
//            return;
//        }
//        else if (dialog != null)
//        {
//            Destroy(dialog);
//        }
//#endif
//    }
    public void ChangeFont()
    {
        Color color = new Color(74, 121, 187);
        Text[] yourLabels = FindObjectsOfType<Text>();
        foreach (Text item in yourLabels)
        {
            item.font = font;
            item.font.material.color = color;
        }
    }
    private void Update()
    {
        ChangeFont();
        BackButtonControll();
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
                //TODO: Çıkış Yapmak istiyormusunuz prompt buraya
                if (!System.String.IsNullOrEmpty(PlayerPrefs.GetString("Cookie")))
                {
                    popupWindow.SetActive(true);
                    popupWindow.gameObject.GetComponentInChildren<Text>().text = "Çıkış yapmak istiyor musunuz?";
                    popupWindow.GetComponent<Animator>().Play("Exit Panel In");
                }
                else
                {
                    profilePanel.SetActive(false);
                    menu.SetActive(true);
                }
                break;
            case "Login":
                loginPanel.SetActive(false);
                menu.SetActive(true);
                break;
            case "Main Menu":
                popupWindow.SetActive(true);
                popupWindow.gameObject.GetComponentInChildren<Text>().text = "Çıkış yapmak istiyor musunuz?";
                popupWindow.GetComponent<Animator>().Play("Exit Panel In");
                break;
            case "Coupons":
                activePanel.SetActive(false);
                profilePanel.SetActive(true);
                break;
            case "Register":
                activePanel.SetActive(false);
                menu.SetActive(true);
                break;
            default:

                break;
        }
    }

    public void BackButtonControll()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escapePressedOnce = true;
                print("pressed once");
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
                        //TODO: Çıkış Yapmak istiyormusunuz prompt buraya
                        if (!System.String.IsNullOrEmpty(PlayerPrefs.GetString("Cookie")))
                        {
                            popupWindow.SetActive(true);
                            popupWindow.gameObject.GetComponentInChildren<Text>().text = "Çıkış yapmak istiyor musunuz?";
                            popupWindow.GetComponent<Animator>().Play("Exit Panel In");
                        }
                        else
                        {
                            profilePanel.SetActive(false);
                            menu.SetActive(true);
                        }
                        break;
                    case "Login":
                        loginPanel.SetActive(false);
                        menu.SetActive(true);
                        break;
                    case "Main Menu":
                        popupWindow.SetActive(true);
                        popupWindow.gameObject.GetComponentInChildren<Text>().text = "Çıkış yapmak istiyor musunuz?";
                        popupWindow.GetComponent<Animator>().Play("Exit Panel In");
                        break;
                    case "Coupons":
                        activePanel.SetActive(false);
                        profilePanel.SetActive(true);
                        break;
                    case "Register":
                        activePanel.SetActive(false);
                        menu.SetActive(true);
                        break;
                    default:

                        break;
                }
            }
        }
    }

    public void ChangeMenu(GameObject panel)
    {
        switch (panel.name)
        {
            case "Login":
                menu.SetActive(false);
                panel.SetActive(true);
                break;
            case "Shop":
                profilePanel.SetActive(false);
                panel.SetActive(true);
                break;
            case "Coupons":
                profilePanel.SetActive(false);
                panel.SetActive(true);
                break;
            case "Register":
                menu.SetActive(false);
                panel.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void StartAnimation()
    {
        playButton.GetComponent<Animator>().speed = 1;
    }
    public void Popup(string choise)
    {
        if (choise == "Yes")
        {
            Application.Quit();
        }
        else if (choise == "No")
        {
            popupWindow.GetComponent<Animator>().Play("Exit Panel Out");
            popupWindow.SetActive(false);
        }
    }
}
