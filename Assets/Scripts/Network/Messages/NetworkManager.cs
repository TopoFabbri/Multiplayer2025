using System;
using System.Net;
using Network.interfaces;
using Utils;

namespace Network
{
    public struct Client
    {
        public float timeStamp;
        public int id;
        public readonly IPEndPoint ipEndPoint;

        public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
        {
            this.timeStamp = timeStamp;
            this.id = id;
            this.ipEndPoint = ipEndPoint;
        }
    }

    public abstract class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
    {
        protected int Port { get; set; }
        
        public Action<byte[]> OnReceiveDataAction;
        
        public int timeOut = 30;

        protected UdpConnection connection;
        
        protected override void Initialize()
        {
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Console, HandleConsole);
            MessageHandler.TryAddHandler(MessageType.Position, HandlePosition);
        }

        protected abstract void HandleHandshake(byte[] data, IPEndPoint ip);

        protected virtual void HandleConsole(byte[] data, IPEndPoint ip)
        {
            OnReceiveDataAction?.Invoke(data);
        }

        protected abstract void HandlePosition(byte[] data, IPEndPoint ip);

        public void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            MessageHandler.Receive(data, ip);
        }

        private void Update()
        {
            // Flush the data in main thread
            connection?.FlushReceiveData();
        }

        public abstract void SendData(byte[] data);
        
        public abstract void Init(int port, IPAddress ip = null);

        private void OnDestroy()
        {
            connection?.Close();
        }
    }
}