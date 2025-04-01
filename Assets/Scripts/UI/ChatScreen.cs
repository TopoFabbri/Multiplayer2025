﻿using System.Net;
using UnityEngine.UI;

namespace UI
{
    public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
    {
        public Text messages;
        public InputField inputMessage;

        protected override void Initialize()
        {
            if (inputMessage)
                inputMessage.onEndEdit.AddListener(OnEndEdit);

            gameObject.SetActive(false);

            NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;
        }

        private void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
        {
            if (NetworkManager.Instance.IsServer)
            {
                NetworkManager.Instance.Broadcast(data);
            }

            messages.text += System.Text.ASCIIEncoding.UTF8.GetString(data) + System.Environment.NewLine;
        }

        private void OnEndEdit(string str)
        {
            if (inputMessage.text == "") return;
            
            if (NetworkManager.Instance.IsServer)
            {
                NetworkManager.Instance.Broadcast(System.Text.ASCIIEncoding.UTF8.GetBytes(inputMessage.text));
                messages.text += inputMessage.text + System.Environment.NewLine;
            }
            else
            {
                NetworkManager.Instance.SendToServer(System.Text.ASCIIEncoding.UTF8.GetBytes(inputMessage.text));
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";

        }

    }
}
