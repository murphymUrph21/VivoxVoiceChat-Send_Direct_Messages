using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivoxUnity;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] VivoxManager credentials;

    #region UI Variables
    // Updated UI variables not shown In my first video
    // The rest of the code is fairly the same
    [SerializeField] Text txt_UserName;
    [SerializeField] Text txt_ChannelName;
    [SerializeField] Text txt_Message_Prefab;
    [SerializeField] TMP_InputField tmp_Input_Username;
    [SerializeField] TMP_InputField tmp_Input_ChannelName;
    [SerializeField] TMP_InputField tmp_Input_SendMessages;
    [SerializeField] Image container;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
        Instantiate(txt_Message_Prefab, container.transform);
    }

    public void Btn_Join_Channel()
    {
        credentials.JoinChannel(tmp_Input_ChannelName.text, true, true, true, ChannelType.NonPositional);
    }

    public void Leave_Channel(IChannelSession channelToDiconnect, string channelName)
    {
        channelToDiconnect.Disconnect();
        credentials.vivox.loginSession.DeleteChannelSession(new ChannelId(credentials.vivox.issuer, channelName, credentials.vivox.domain));
    }

    public void Btn_Leave_Channel_Clicked()
    {
        Leave_Channel(credentials.vivox.channelSession, tmp_Input_ChannelName.text);
    }

    public void LoginUser()
    {
        credentials.Login(tmp_Input_Username.text);
    }

    public void Logout()
    {
        credentials.vivox.loginSession.Logout();
        credentials.Bind_Login_Callback_Listeners(false, credentials.vivox.loginSession);
    }
}
