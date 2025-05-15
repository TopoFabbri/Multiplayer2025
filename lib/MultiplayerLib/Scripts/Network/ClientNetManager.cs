using System.Collections.Generic;
using System.Linq;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Utils;

namespace Multiplayer.Network
{
    public class ClientNetManager : NetworkManager
    {
        public IPAddress IPAddress { get; private set; }
        
        private readonly List<int> clientIds = new();
        private int playerId;

        private float LastPingTime { get; set; }

        public override void Init(int port, IPAddress ip = null)
        {
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Ping, HandlePing);
            
            Port = port;
            IPAddress = ip;

            connection = new UdpConnection(ip, port, this);
            
            SendToServer(new NetHandShake(new List<int>()).Serialize());

            base.Init(port, ip);
        }

        private void SendToServer(byte[] data)
        {
            connection.Send(data);
        }

        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            clientIds.AddRange(new NetHandShake(data).Deserialized());
            
            if (ID == 0)
                ID = clientIds.Last();
        }

        private void HandlePing(byte[] data, IPEndPoint ip)
        {
            Ping = Timer.Time - LastPingTime;
            
            LastPingTime = Timer.Time;
        }

        public override void SendData(byte[] data)
        {
            SendToServer(data);
        }

        public override void SendTo(byte[] data, IPEndPoint ip = null)
        {
            base.SendTo(data, ip);
            connection.Send(data);
        }

        protected override void OnDestroy()
        {
            SendToServer(new NetDisconnect(playerId).Serialize());
            
            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryRemoveHandler(MessageType.Ping, HandlePing);
        }
    }
}