using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network.Messages;
using Objects;
using UnityEngine;

namespace Network
{
    public class ClientNetManager : NetworkManager
    {
        public IPAddress IPAddress { get; private set; }
        
        private readonly List<int> clientIds = new();

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
            Ping = Time.time - LastPingTime;
            
            LastPingTime = Time.time;
        }

        public override void SendData(byte[] data)
        {
            SendToServer(data);
        }

        public override void SendTo(byte[] data, IPEndPoint ip = null)
        {
            connection?.Send(data);
        }

        private void OnDestroy()
        {
            SendToServer(new NetDisconnect(Player.PlayerID).Serialize());
            
            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryRemoveHandler(MessageType.Ping, HandlePing);
        }
    }
}