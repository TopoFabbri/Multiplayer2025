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

        public override void Init(int port, IPAddress ip = null)
        {
            Port = port;
            IpAddress = ip;

            connection = new UdpConnection(ip, port, this);

            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Ping, HandlePing);

            SendTo(new NetHandShake(new HandShake(0, new List<int>()), false).Serialize());

            base.Init(port, ip);
        }

        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            HandShake hs = new NetHandShake(data).Deserialized();

            clientIds.AddRange(hs.clients);
            
            CheckSum.RandomSeed = hs.randomSeed;
            CheckSum.CreateOperationsArrays((int)hs.randomSeed);

            if (Id == 0)
            {
                Id = clientIds.Last();
                onConnectionEstablished?.Invoke();
            }
        }

        private void HandlePing(byte[] data, IPEndPoint ip)
        {
            Ping = Timer.Time - LastPingTime;

            LastPingTime = Timer.Time;
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

        protected override void OnDestroy()
        {
            SendTo(new NetDisconnect(PlayerId).Serialize());

            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryRemoveHandler(MessageType.Ping, HandlePing);
        }
    }
}