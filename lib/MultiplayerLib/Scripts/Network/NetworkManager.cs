using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Multiplayer.Network.interfaces;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Utils;
using Timer = Multiplayer.Utils.Timer;

namespace Multiplayer.Network
{
    public struct Client
    {
        public float timeStamp;
        public int id;
        public readonly IPEndPoint ipEndPoint;
        public float lastPingTime;

        public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
        {
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
            lastPingTime = Timer.Time;
        }
    }

    public abstract class NetworkManager : Singleton<NetworkManager>, IReceiveData
    {
        public int Id { get; protected set; }

        protected int Port { get; set; }
        public float Ping { get; protected set; }
        public bool IsInitiated { get; private set; }

        public Action onConnectionEstablished;
        public Action<byte[], IPEndPoint> OnReceiveDataAction;
        protected const float TimeOut = 5f;

        protected UdpConnection connection;
        protected override void Start()
        {
            ImportantMessageHandler.OnShouldResendMessages += ResendMessages;
            MessageHandler.TryAddHandler(MessageType.Acknowledge, MessageHandler.HandleAcknowledge);
            MessageHandler.onShouldAcknowledge += OnShouldAcknowledge;
        }

        private void ResendMessages(List<PendingMessage> messagesToResend)
        {
            foreach (PendingMessage pendingMessage in messagesToResend)
            {
                SendTo(pendingMessage.message, pendingMessage.ip);
            }
        }
        
        public virtual void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            OnReceiveDataAction?.Invoke(data, ip);

            MessageHandler.Receive(data, ip);
        }

        public virtual void Update()
        {
            connection.FlushReceiveData();
        }

        public abstract void SendData(byte[] data);
        
        public virtual void SendTo(byte[] data, IPEndPoint ip = null)
        {
            MessageHandler.OnSendData(data, ip);
        }

        public virtual void Init(int port, IPAddress ip = null)
        {
            IsInitiated = true;
        }

        private void OnShouldAcknowledge(MessageMetadata metadata, IPEndPoint ip)
        {
            Acknowledge acknowledge = new() { mesId = metadata.MsgId, senderId = metadata.SenderId, mesType = metadata.Type };

            SendTo(new NetAcknowledge(acknowledge).Serialize(), ip);
        }
    }
}