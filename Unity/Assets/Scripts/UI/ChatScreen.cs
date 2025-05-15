using System;
using System.Net;
using Game;
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

            NetworkManager.Instance.OnReceiveDataAction += OnReceiveConsoleHandler;

            InputListener.Chat += OnChatHandler;
        }

        private void OnDestroy()
        {
            NetworkManager.Instance.OnReceiveDataAction -= OnReceiveConsoleHandler;

            InputListener.Chat -= OnChatHandler;
        }

        private void OnChatHandler()
        {
            gameObject.SetActive(!gameObject.activeSelf);

            Cursor.lockState = gameObject.activeSelf ? CursorLockMode.None : CursorLockMode.Confined;
            Cursor.visible = gameObject.activeSelf;
        }

        private void OnReceiveConsoleHandler(byte[] data, IPEndPoint ip)
        {
            if (MessageHandler.GetMetadata(data).Type != MessageType.Console)
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