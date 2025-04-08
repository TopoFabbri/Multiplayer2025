using System;
using Network;
using Network.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChatScreen : MonoBehaviour
    {
        [SerializeField] private Text messages;
        [SerializeField] private InputField inputMessage;

        private void Start()
        {
            if (inputMessage)
                inputMessage.onEndEdit.AddListener(OnEndEdit);

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            NetworkManager.Instance.OnReceiveDataAction += OnReceiveConsoleHandler;
        }

        private void OnDisable()
        {
            if (NetworkManager.Instance)
                NetworkManager.Instance.OnReceiveDataAction -= OnReceiveConsoleHandler;
        }

        private void OnReceiveConsoleHandler(byte[] data)
        {
            if (MessageHandler.GetMessageType(data) != MessageType.Console)
                return;
            
            string message = new NetConsole(data).Deserialized();
            messages.text += message + Environment.NewLine;
        }

        private void OnEndEdit(string str)
        {
            if (inputMessage.text == "") return;

            NetworkManager.Instance.SendData(new NetConsole(inputMessage.text).Serialize());

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}