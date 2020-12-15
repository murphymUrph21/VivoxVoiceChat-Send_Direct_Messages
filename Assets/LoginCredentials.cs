using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.ComponentModel;
using System;
using UnityEngine.UI;
using TMPro;

public class LoginCredentials : MonoBehaviour
{
    VivoxUnity.Client client;
    private Uri server = new Uri("https://mt1s.www.vivox.com/api2");
    private string issuer = "johnmu0739-vi31-dev";
    private string  domain = "mt1s.vivox.com";
    private string tokenKey = "luck408";
    private TimeSpan timeSpan = new TimeSpan(90);
    private ILoginSession loginSession;


    #region UI Variables

    [SerializeField] Text txt_UserName;
    [SerializeField] Text txt_ChannelName;
    [SerializeField] Text txt_Message_Prefab;
    [SerializeField] TMP_InputField tmp_Input_Username;
    [SerializeField] TMP_InputField tmp_Input_ChannelName;
    [SerializeField] TMP_InputField tmp_Input_SendMessages;
    [SerializeField] Image container;

    #endregion



    private void Awake()
    {
        client = new Client();
        client.Uninitialize();
        client.Initialize();
        DontDestroyOnLoad(this);
    }

    private void OnApplicationQuit()
    {
        client.Uninitialize();
    }

    void Start()
    {
      //  Instantiate(txtMessage, container.transform);

    }

    public void Bind_Login_Callback_Listeners(bool bind, ILoginSession loginSesh)
    {
        if (bind)
        {
             loginSesh.PropertyChanged += Login_Status;
        }
        else
        {
            loginSesh.PropertyChanged -= Login_Status;
        }

    }

    public void LoginUser()
    {
        Login("Test_Name");
    }

    public void Login(string userName)
    {
        AccountId accountId = new AccountId(issuer, userName, domain);
        loginSession = client.GetLoginSession(accountId);

        Bind_Login_Callback_Listeners(true, loginSession);
        loginSession.BeginLogin(server, loginSession.GetLoginToken(tokenKey, timeSpan), ar =>
        {
            try
            {
                loginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                Bind_Login_Callback_Listeners(false, loginSession);
                Debug.Log(e.Message);
            }
            // run more code here 
        });
    }

    public void Logout()
    {
        loginSession.Logout();
        Bind_Login_Callback_Listeners(false, loginSession);
    }

    public void Login_Status(object sender, PropertyChangedEventArgs loginArgs)
    {
        var source = (ILoginSession)sender;
        
        switch (source.State)
        {
            case LoginState.LoggingIn:
                Debug.Log("Logging In");
                break;
            
            case LoginState.LoggedIn:
                Debug.Log($"Logged In {loginSession.LoginSessionId.Name}");
                break;
        }
    }

}
