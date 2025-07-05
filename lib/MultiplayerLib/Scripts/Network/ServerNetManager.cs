using System.Collections.Generic;
using System.Linq;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Utils;

namespace Multiplayer.Network
{
    public class ServerNetManager : NetworkManager
    {
        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();

        private readonly List<IPEndPoint> disconnectedClients = new();
        private readonly Dictionary<int, Color> colorsByClientId = new();

        public bool Active { get; private set; } = true;

        public override void Init(int port, IPAddress ip = null, string name = "Player")
        {
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);

            Port = port;
            connection = new UdpConnection(port, this);

            Id = 0;

            base.Init(port, ip, name);
            onConnectionEstablished?.Invoke();

            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.Ping, OnAcknowledgePingHandler);
            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.HandShake, OnAcknowledgeHandshakeHandler);

            Log.Write("Server running at port " + port);
            Log.NewLine(2);

            CheckSum.RandomSeed = (uint)Timer.DateTime.Millisecond;
            CheckSum.CreateOperationsArrays(CheckSum.RandomSeed);
            Crypt.GenerateOperations(CheckSum.RandomSeed);

            Active = true;
        }

        public override void Update()
        {
            base.Update();

            disconnectedClients.Clear();

            foreach (KeyValuePair<int, Client> client in clients)
            {
                float timeSinceLastPing = Timer.Time - client.Value.lastPingTime;

                if (timeSinceLastPing > TimeOut)
                    disconnectedClients.Add(client.Value.ipEndPoint);
            }

            foreach (IPEndPoint ipEndPoint in disconnectedClients)
                RemoveClient(ipEndPoint);

            disconnectedClients.Clear();
        }

        private void SendToClient(byte[] data, int id)
        {
            SendTo(data, clients[id].ipEndPoint);
        }

        private void AddClient(IPEndPoint ip, Color color, string name)
        {
            if (ipToId.ContainsKey(ip)) return;

            int clientId = 1;

            while (clients.ContainsKey(clientId))
                clientId++;

            Log.Write("Adding client: " + name);
            Log.NewLine();

            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, clientId, Timer.Time, 0, name));
            colorsByClientId.Add(clientId, color);


            Dictionary<int, string> names = new();

            foreach (KeyValuePair<int, Client> tmpClient in clients)
                names.Add(tmpClient.Key, tmpClient.Value.name);

            HandShake hs = new(CheckSum.RandomSeed, colorsByClientId, names, true, 0, name);
            SendData(new NetHandShake(hs, true).Serialize());
        }

        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            HandShake hs = new NetHandShake(data).Deserialized();

            AddClient(ip, hs.clientColorsById.Last().Value, hs.name);
        }

        protected void RemoveClient(IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;

            Log.Write("Removing client: " + clients[id].name);
            Log.NewLine();

            ipToId.Remove(ip);
            clients.Remove(id);
            colorsByClientId.Remove(id);

            if (clients.Count <= 0)
                Active = false;

            SendData(new NetDisconnect(0).Serialize());
        }

        public override void SendData(byte[] data)
        {
            foreach (KeyValuePair<int, Client> keyValuePair in clients)
                SendTo(data, clients[keyValuePair.Key].ipEndPoint);
        }

        protected void SendWithException(byte[] data, List<IPEndPoint> exceptions)
        {
            List<int> exceptionIds = new();
            
            foreach (IPEndPoint ip in exceptions)
            {
                if (ipToId.TryGetValue(ip, out int id))
                    exceptionIds.Add(id);
            }
            
            foreach (KeyValuePair<int, Client> keyValuePair in clients)
            {
                if (!exceptionIds.Contains(keyValuePair.Key))
                    SendTo(data, clients[keyValuePair.Key].ipEndPoint);
            }
        }

        public override void SendTo(byte[] data, IPEndPoint ip = null)
        {
            base.SendTo(data, ip);

            if (Crypt.IsCrypted(data))
                data = Crypt.Encrypt(data);

            connection?.Send(data, ip);
        }

        private void OnAcknowledgePingHandler(byte[] data, IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;

            Client client = clients[id];

            client.lastPingTime = Timer.Time;

            clients[id] = client;

            Dictionary<int, float> pingsByClientId = new();

            foreach (KeyValuePair<int, Client> tmpClient in clients)
                pingsByClientId.Add(tmpClient.Key, Timer.Time - tmpClient.Value.lastPingTime);

            SendToClient(new NetPing(new PingWrapper(pingsByClientId)).Serialize(), id);
        }

        private void OnAcknowledgeHandshakeHandler(byte[] data, IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;

            SendToClient(new NetPing(new PingWrapper()).Serialize(), id);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);

            connection.Close();
        }
    }
}