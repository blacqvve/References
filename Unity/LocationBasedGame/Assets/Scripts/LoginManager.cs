using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private GameObject profilePanel, subLoading;
    [SerializeField]
    private GameObject inputEmail, inputPassword;
    [SerializeField]
    private Text errorText;
    string email, password;
    DatabaseManager databaseManager;

    private void Awake()
    {
        databaseManager =FindObjectOfType<DatabaseManager>();
    }
    void Start()
    {
        if (!System.String.IsNullOrEmpty(PlayerPrefs.GetString("Cookie")))
        {
            print("Cookie bulundu:" + PlayerPrefs.GetString("Cookie"));
            LoginFinished();
        }
    }
    private void LateUpdate()
    {
        email = inputEmail.GetComponent<InputField>().text;
        password = inputPassword.GetComponent<InputField>().text;
    }
    public void ValideInputs()
    {
        if (!System.String.IsNullOrEmpty(email) && !System.String.IsNullOrEmpty(password))
        {
            Instance_Waiting();
            databaseManager.SendLogin(email, password);
            databaseManager.Error += Instance_Error;
            databaseManager.OnLoginFinished += LoginFinished;


        }
        else
        {
            //TODO:Validation Error
            print("inputlar boş");
        }
    }

    private void Instance_Error(string obj)
    {
        if (!System.String.IsNullOrEmpty(obj))
        {
            subLoading.SetActive(false);
            errorText.text = obj;
        }
    }

    private void Instance_Waiting()
    {
        subLoading.SetActive(true);
    }

    private void LoginFinished()
    {
        subLoading.SetActive(false);
        this.gameObject.SetActive(false);
        profilePanel.SetActive(true);
    }

    private void Perms()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

        }
#endif
    }
}
