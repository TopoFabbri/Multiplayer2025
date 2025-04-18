﻿using System.Net;
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

        [SerializeField] private string defaultAddress = "127.0.0.1";
        
        private void Awake()
        {
            if (connectBtn)
                connectBtn.onClick.AddListener(OnConnectBtnClick);
        }
        
        private void OnConnectBtnClick()
        {
            if (addressInputField.text == "")
                addressInputField.text = defaultAddress;
            
            if (portInputField.text == "")
                portInputField.text = defaultPort;
            
            IPAddress ipAddress = IPAddress.Parse(addressInputField.text);
            int port = System.Convert.ToInt32(portInputField.text);

            NetworkManager.Instance.Init(port, ipAddress);
        
            SwitchToChatScreen();
        }
    }
}