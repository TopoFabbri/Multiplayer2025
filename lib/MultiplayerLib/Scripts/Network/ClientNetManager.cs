using System.Collections.Generic;
using System.Linq;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Utils;

namespace Multiplayer.Network
{
    public class ClientNetManager : NetworkManager
    {
        public IPAddress IpAddress { get; private set; }

        private readonly List<int> clientIds = new();
        public int PlayerId { private get; set; }

        private float LastPingTime { get; set; }
        private int MmPort { get; set; }
        private bool ConnectToServer { get; set; }
        private IPEndPoint MmIp { get; set; }

        protected override void Start()
        {
            base.Start();
            
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Ping, HandlePing);
            MessageHandler.TryAddHandler(MessageType.ServerInfo, HandleServerInfo);

            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.Disconnect, HandleAcknowledgedDisconnect);
        }

        public override void Init(int port, IPAddress ip = null)
        {
            Port = port;
            IpAddress = ip;

            Id = -1;
            
            connection = new UdpConnection(ip, port, this);

            SendTo(new NetHandShake(new HandShake(0, new List<int>(), false), false).Serialize());

            base.Init(port, ip);
        }

        public override void Update()
        {
            base.Update();

            // if (Timer.Time - LastPingTime > TimeOut)
            //     Disconnect();
        }

        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            HandShake hs = new NetHandShake(data).Deserialized();

            CheckSum.RandomSeed = hs.randomSeed;
            CheckSum.CreateOperationsArrays(hs.randomSeed);

            clientIds.AddRange(hs.clients);

            if (Id > 0) return;

            Id = clientIds.Last();

            if (hs.fromServer)
            {
                onConnectionEstablished?.Invoke();
            }
            else
            {
                MmPort = Port;
                MmIp = ip;
            }
        }

        private void HandlePing(byte[] data, IPEndPoint ip)
        {
            Ping = Timer.Time - LastPingTime;

            LastPingTime = Timer.Time;
        }

        private void HandleServerInfo(byte[] data, IPEndPoint ip)
        {
            Port = new NetServerInfo(data).Deserialized().port;
            ConnectToServer = true;

            RequestDisconnect();
        }

        private void HandleAcknowledgedDisconnect(byte[] data, IPEndPoint ip)
        {
            Disconnect();

            if (!ConnectToServer) return;

            Init(Port, IpAddress);
            ConnectToServer = false;
        }

        public override void SendData(byte[] data)
        {
            SendTo(data);
        }

        public override void SendTo(byte[] data, IPEndPoint ip = null)
        {
            base.SendTo(data, ip);
            connection.Send(data);
        }

        public void RequestDisconnect()
        {
            SendTo(new NetDisconnect(PlayerId).Serialize());
        }

        private void Disconnect()
        {
            clientIds.Clear();

            connection?.Close();
        }

        protected override void OnDestroy()
        {
            Disconnect();

            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryRemoveHandler(MessageType.Ping, HandlePing);
        }
    }
}