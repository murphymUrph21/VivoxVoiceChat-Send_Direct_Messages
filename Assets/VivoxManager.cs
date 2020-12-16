using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using System.ComponentModel;
using System;
using UnityEngine.UI;
using TMPro;

public class VivoxManager : MonoBehaviour
{

    public Base_Credentials vivox = new Base_Credentials();
    public LobbyUI lobbyUI;





    private void Awake()
    {
        vivox.client = new Client();
        vivox.client.Uninitialize();
        vivox.client.Initialize();
        DontDestroyOnLoad(this);
    }

    private void OnApplicationQuit()
    {
        vivox.client.Uninitialize();
    }

    void Start()
    {


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

    public void Bind_Channel_Callback_Listeners(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.PropertyChanged += On_Channel_Status_Changed;
        }
        else
        {
            channelSesh.PropertyChanged -= On_Channel_Status_Changed;
        }
    }

    public void Bind_User_Callbacks(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.Participants.AfterKeyAdded += On_Participant_Added;
            channelSesh.Participants.BeforeKeyRemoved+= On_Participant_Removed;
            channelSesh.Participants.AfterValueUpdated += On_Participant_Updated;
        }
        {
            channelSesh.Participants.AfterKeyAdded -= On_Participant_Added;
            channelSesh.Participants.BeforeKeyRemoved -= On_Participant_Removed;
            channelSesh.Participants.AfterValueUpdated -= On_Participant_Updated;
        }
    }




    #region Login Methods


    public void Login(string userName)
    {
        AccountId accountId = new AccountId(vivox.issuer, userName, vivox.domain);
        vivox.loginSession = vivox.client.GetLoginSession(accountId);

        Bind_Login_Callback_Listeners(true, vivox.loginSession);
        vivox.loginSession.BeginLogin(vivox.server, vivox.loginSession.GetLoginToken(vivox.tokenKey, vivox.timeSpan), ar =>
        {
            try
            {
                vivox.loginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                Bind_Login_Callback_Listeners(false, vivox.loginSession);
                Debug.Log(e.Message);
            }
            // run more code here 
        });
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
                Debug.Log($"Logged In {vivox.loginSession.LoginSessionId.Name}");
                break;
        }
    }



    #endregion


    #region Join Channel Methods

    public void JoinChannel(string channelName, bool IsAudio, bool IsText, bool switchTransmission,ChannelType channelType)
    {
        
        ChannelId channelId = new ChannelId(vivox.issuer, channelName, vivox.domain, channelType);
        vivox.channelSession = vivox.loginSession.GetChannelSession(channelId);
        Bind_Channel_Callback_Listeners(true, vivox.channelSession);

        if (IsAudio)
        {
            vivox.channelSession.PropertyChanged += On_Audio_State_Changed;
        }
        if (IsText)
        {
            vivox.channelSession.PropertyChanged += On_Text_State_Changed;
        }


        vivox.channelSession.BeginConnect(IsAudio, IsText, switchTransmission, vivox.channelSession.GetConnectToken(vivox.tokenKey, vivox.timeSpan), ar => 
        {
            try
            {
                vivox.channelSession.EndConnect(ar);
            }
            catch(Exception e)
            {
                Bind_Channel_Callback_Listeners(false, vivox.channelSession);
                if (IsAudio)
                {
                    vivox.channelSession.PropertyChanged -= On_Audio_State_Changed;
                }
                if (IsText)
                {
                    vivox.channelSession.PropertyChanged -= On_Text_State_Changed;
                }
                Debug.Log(e.Message);
            }
        });
    }




    public void On_Channel_Status_Changed(object sender, PropertyChangedEventArgs channelArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch (source.ChannelState)
        {
            case ConnectionState.Connecting:
                Debug.Log("Channel Connecting");
                break;    
            case ConnectionState.Connected:
                Debug.Log($"{source.Channel.Name} Connected");
                break;       
            case ConnectionState.Disconnecting:
                Debug.Log($"{source.Channel.Name} disconnecting");
                break;    
            case ConnectionState.Disconnected:
                Debug.Log($"{source.Channel.Name} disconnected");
                break;
        }
    }

    public void On_Audio_State_Changed(object sender, PropertyChangedEventArgs audioArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch (source.AudioState)
        {
            case ConnectionState.Connecting:
                Debug.Log($"Audio Channel Connecting");
                break;          
            case ConnectionState.Connected:
                Debug.Log($"Audio Channel Connected");
                break;          
            case ConnectionState.Disconnecting:
                Debug.Log($"Audio Channel Disconnecting");
                break;         
            case ConnectionState.Disconnected:
                Debug.Log($"Audio Channel Disconnected");
                break;
        }
    }  
    
    public void On_Text_State_Changed(object sender, PropertyChangedEventArgs audioArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch(source.TextState)
        {
            case ConnectionState.Connecting:
                Debug.Log($"Text Channel Connecting");
                break;
            case ConnectionState.Connected:
                Debug.Log($"Text Channel Connected");
                break;
            case ConnectionState.Disconnecting:
                Debug.Log($"Text Channel Disconnecting");
                break;
            case ConnectionState.Disconnected:
                Debug.Log($"Text Channel Disconnected");
                break;
        }

    }

    #endregion


    #region User Callbacks


    public void On_Participant_Added(object sender, KeyEventArg<string> participantArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = source[participantArgs.Key];
        Debug.Log($"{user.Account.Name} has joined the channel");
    }   

    public void On_Participant_Removed(object sender, KeyEventArg<string> participantArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = source[participantArgs.Key];
        Debug.Log($"{user.Account.Name} has left the channel");
        
    }  

    public void On_Participant_Updated(object sender, ValueEventArg<string, IParticipant> participantArgs)
    {
        var source = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = source[participantArgs.Key];
    }



    #endregion



}
