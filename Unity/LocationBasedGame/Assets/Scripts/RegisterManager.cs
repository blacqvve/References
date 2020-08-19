using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RegisterManager : MonoBehaviour
{

    [SerializeField]
    private Toggle termsToggle;
    [SerializeField]
    private GameObject termsWindow,profilePanel,subLoading;
    [SerializeField]
    private GameObject inputEmail, inputPassword, inputGender;
    [SerializeField]
    private Text errorText;
    string email, password, gender;
    DatabaseManager databaseManager;

    private void Awake()
    {
        databaseManager = FindObjectOfType<DatabaseManager>();
    }
    private void OnEnable()
    {
        termsWindow.SetActive(false);
        termsToggle.isOn = false;
    }

    private void LateUpdate()
    {
        email = inputEmail.GetComponent<InputField>().text;
        password = inputPassword.GetComponent<InputField>().text;
        var genderOptions = inputGender.GetComponent<Dropdown>();
        gender = genderOptions.value.ToString() ;

    }
    public void ValideInputs()
    {
        //if (!String.IsNullOrEmpty(email)&&!String.IsNullOrEmpty(password)&&!String.IsNullOrEmpty(gender))
        //{
        Waiting();
        databaseManager.SendRegister(email, password, gender);
        databaseManager.Error += Instance_Error;
        databaseManager.OnRegisterFinished += Instance_OnRegisterFinished;
        //}
        //else
        //{
        //    //TODO:Validation error
        //    print("inputlar boş");
        //}
    }

    private void Instance_Error(string obj)
    {
        if (!System.String.IsNullOrEmpty(obj))
        {
            subLoading.SetActive(false);
            errorText.text = obj;
        }
    }

    private void Waiting()
    {
        subLoading.SetActive(true);
    }
    private void Instance_OnRegisterFinished()
    {
        subLoading.SetActive(false);
        this.gameObject.SetActive(false);
        profilePanel.SetActive(true);
    }

    public void AcceptTerms(string choise)
    {
        switch (choise)
        {
            case "Accepted":
                termsWindow.SetActive(false);
                termsToggle.isOn = true;
                break;
            case "Rejected":
                termsWindow.SetActive(false);
                termsToggle.isOn = false;
                break;
            default:
                termsWindow.SetActive(true);
                break;
        }
    }
}
