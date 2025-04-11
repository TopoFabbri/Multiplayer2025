using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network.Messages;

namespace Network
{
    public class ClientNetManager : NetworkManager
    {
        public IPAddress IPAddress { get; private set; }
        
        private readonly List<int> clientIds = new();

        public override void Init(int port, IPAddress ip = null)
        {
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

        protected override void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            clientIds.AddRange(new NetHandShake(data).Deserialized());
            
            if (ID == 0)
                ID = clientIds.Last();
        }

        protected override void HandlePosition(byte[] data, IPEndPoint ip)
        {
        }

        public override void SendData(byte[] data)
        {
            SendToServer(data);
        }
    }
}