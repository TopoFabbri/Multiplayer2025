using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Multiplayer.Network.interfaces;
using Multiplayer.Network.Messages;
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

    public abstract class NetworkManager : Singleton<NetworkManager>, IReceiveData, IDisposable
    {
        protected int ID { get; set; }

        protected int Port { get; set; }
        public float Ping { get; protected set; }

        public Action onConnectionEstablished;
        public Action<byte[], IPEndPoint> OnReceiveDataAction;
        protected const float TimeOut = 5f;

        protected UdpConnection connection;
        protected bool _disposed;
        protected override void Init()
        {
            ImportantMessageHandler.OnShouldResendMessages += ResendMessages;
        }

        private void ResendMessages(List<PendingMessage> messagesToResend)
        {
            foreach (PendingMessage pendingMessage in messagesToResend)
            {
                SendTo(pendingMessage.message, pendingMessage.ip);
            }
        }
        
        public virtual void Dispose()
        {
            if (_disposed) return;

            try
            {
                connection.FlushReceiveData();
                Thread.Sleep(100);

                connection.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[NetworkManager] Disposal error: {e.Message}");
            }

            _disposed = true;
        }
        
        public virtual void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            OnReceiveDataAction?.Invoke(data, ip);

            MessageHandler.Receive(data, ip);
        }

        protected virtual void Update()
        {
            // Flush the data in main thread
            connection?.FlushReceiveData();
        }

        public abstract void SendData(byte[] data);

        public virtual void SendTo(byte[] data, IPEndPoint ip = null)
        {
            MessageHandler.OnSendData(data, ip);
        }

        public virtual void Init(int port, IPAddress ip = null)
        {
            onConnectionEstablished?.Invoke();
            
            MessageHandler.TryAddHandler(MessageType.Acknowledge, MessageHandler.HandleAcknowledge);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            Dispose();
        }
    }
}