using System;
using System.Net;
using Game;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChatScreen : MonoBehaviour
    {
        [SerializeField] private Text messages;
        [SerializeField] private InputField inputMessage;

        private void Awake()
        {
            if (inputMessage)
                inputMessage.onEndEdit.AddListener(OnEndEdit);

            MessageHandler.TryAddHandler(MessageType.Console, OnReceiveConsoleHandler);

            GameStateController.StateChanged += OnStateChanged;

            InputListener.Chat += OnChatHandler;
        }

        private void OnStateChanged(GameState newState)
        {
            if (newState == GameState.InGame)
            {
                gameObject.SetActive(true);
                return;
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            MessageHandler.TryRemoveHandler(MessageType.Console, OnReceiveConsoleHandler);

            GameStateController.StateChanged -= OnStateChanged;

            InputListener.Chat -= OnChatHandler;
        }

        private void OnChatHandler()
        {
            if (GameStateController.State != GameState.InGame)
                return;

            gameObject.SetActive(!gameObject.activeSelf);
        }

        private void OnReceiveConsoleHandler(byte[] data, IPEndPoint ip)
        {
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