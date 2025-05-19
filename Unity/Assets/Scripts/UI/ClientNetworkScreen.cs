using System;
using System.Net;
using Multiplayer.Network;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ClientNetworkScreen : NetworkScreen
    {
        [SerializeField] private Button connectBtn;
        [SerializeField] private InputField addressInputField;
        [SerializeField] private InputField nameIf;

        [SerializeField] private string defaultAddress = "127.0.0.1";

        public static event Action<string> Connect;
        
        protected override void Awake()
        {
            base.Awake();
            
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
            int port = Convert.ToInt32(portInputField.text);

            NetworkManager.Instance.Init(port, ipAddress, nameIf.text);
            
            Connect?.Invoke(nameIf.text);
        }
    }
}