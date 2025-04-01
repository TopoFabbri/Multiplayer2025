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
            int port = System.Convert.ToInt32(portInputField.text);
            NetworkManager.Instance.StartServer(port);
            SwitchToChatScreen();
        }
    }
}