using System.Net;
using Network;
using Network.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ClientNetworkScreen : NetworkScreen
    {
        [SerializeField] private Button connectBtn;
        [SerializeField] private InputField addressInputField;
        
        private void Awake()
        {
            if (connectBtn)
                connectBtn.onClick.AddListener(OnConnectBtnClick);
        }
        
        private void OnConnectBtnClick()
        {
            IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
            int port = System.Convert.ToInt32(portInputField.text);

            NetworkManager.Instance.Init(port, ipAddress);
        
            SwitchToChatScreen();
        }
    }
}