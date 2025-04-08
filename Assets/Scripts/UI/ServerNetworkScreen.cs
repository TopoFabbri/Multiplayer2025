using System;
using Network;
using Network.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ServerNetworkScreen : NetworkScreen
    {
        [SerializeField] private Button startServerBtn;
        
        private void Awake()
        {
            startServerBtn.onClick.AddListener(OnStartServerBtnClick);
        }
        
        private void OnStartServerBtnClick()
        {
            int port = Convert.ToInt32(portInputField.text);
            NetworkManager.Instance.Init(port);
            SwitchToChatScreen();
        }
    }
}