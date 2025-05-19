using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Utils;
using Timer = Multiplayer.Utils.Timer;

namespace Multiplayer.Network
{
    public class MatchMakerNetManager : NetworkManager
    {
        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();
        private readonly List<int> openServers = new();

        private List<int> readyClients = new();

        private string serverPath;
        private readonly List<IPEndPoint> disconnectedClients = new();
        private readonly Dictionary<int, Color> colorsByClientId = new();

        private const int PlayerQty = 2;

        protected override void Start()
        {
            base.Start();

            serverPath = Directory.GetCurrentDirectory() + "/Server.exe";
        }

        public override void Init(int port, IPAddress ip = null)
        {
            base.Init(port, ip);

            Port = port;
            connection = new UdpConnection(port, this);

            Id = 0;

            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandShake);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);
            MessageHandler.TryAddHandler(MessageType.Ready, HandleReady);

            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.HandShake, HandleAcknowledgedHs);
            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.Ping, HandleAcknowledgedPing);

            Log.Write("Started MatchMaker at port: " + port);
            Log.NewLine(2);

            CheckSum.RandomSeed = (uint)Timer.DateTime.Millisecond;
            CheckSum.CreateOperationsArrays(CheckSum.RandomSeed);
            Crypt.GenerateOperations(CheckSum.RandomSeed);
            ;
        }

        public override void Update()
        {
            base.Update();

            if (readyClients.Count < PlayerQty) return;

            readyClients = SortedClientsByLevel(readyClients);

            List<Client> clientsToConnect = new();
            
            for (int i = 0; i + 1 < readyClients.Count; i += 2)
            {
                clientsToConnect.Add(clients[readyClients[i]]);
                clientsToConnect.Add(clients[readyClients[i + 1]]);

                if (clientsToConnect.Count < 2) return;

                OpenServer(new List<Client> { clients[readyClients[i]], clients[readyClients[i + 1]]});
            }

            foreach (Client client in clientsToConnect)
                readyClients.Remove(client.id);

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

        private void HandleHandShake(byte[] data, IPEndPoint ip)
        {
            HandShake hs = new NetHandShake(data).Deserialized();
            AddClient(ip, hs.level);
        }

        private void AddClient(IPEndPoint ip, int level)
        {
            int clientId = 1;

            while (clients.ContainsKey(clientId))
                clientId++;

            clients.Add(clientId, new Client(ip, clientId, Timer.Time, level));
            ipToId.Add(ip, clientId);
            colorsByClientId.Add(clientId, new Color());

            HandShake hs = new(CheckSum.RandomSeed, colorsByClientId, false, 0);
            SendTo(new NetHandShake(hs, true).Serialize(), ip);

            Log.Write("Client " + clientId + " connected!");
            Log.NewLine();

            LogConnectedClients();
        }

        private void LogConnectedClients()
        {
            if (clients.Count == 0)
            {
                Log.Write("No clients connected!");
                Log.NewLine();
            }
            else
            {
                Log.Write("Connected clients: ");

                foreach (KeyValuePair<int, Client> client in clients)
                    Log.Write("Client " + client.Value.id + "    ");
            }

            Log.NewLine(2);
        }

        private void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
            RemoveClient(ip);
        }

        private void HandleReady(byte[] data, IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;

            readyClients.Add(id);
        }

        private void RemoveClient(IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int clientId)) return;

            readyClients.Remove(clientId);
            colorsByClientId.Remove(clientId);
            clients.Remove(clientId);
            ipToId.Remove(ip);
            Log.Write("Client " + clientId + " disconnected!");
            Log.NewLine();
            LogConnectedClients();
        }

        private void HandleAcknowledgedHs(byte[] data, IPEndPoint ip)
        {
            SendTo(new NetPing(new PingWrapper()).Serialize(), ip);
        }

        private void HandleAcknowledgedPing(byte[] data, IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;
            float ping = Timer.Time - clients[id].lastPingTime;

            Client client = clients[id];

            client.lastPingTime = Timer.Time;

            clients[id] = client;
            Dictionary<int, float> pingsByClientId = new();
            
            foreach (KeyValuePair<int, Client> tmpClient in clients)
                pingsByClientId.Add(tmpClient.Key, Timer.Time - tmpClient.Value.lastPingTime);

            SendTo(new NetPing(new PingWrapper(pingsByClientId)).Serialize(), clients[id].ipEndPoint);
        }

        private void OpenServer(List<Client> clientsToConnect)
        {
            int port = Port;

            do
            {
                port++;
            } while (openServers.Contains(port));

            openServers.Add(port);

            StartServer(port);

            Thread.Sleep(1000);

            NetServerInfo svInfo = new(new ServerInfo(port));

            foreach (Client client in clientsToConnect)
                SendTo(svInfo.Serialize(), client.ipEndPoint);
        }

        private void StartServer(int port)
        {
            if (!File.Exists(serverPath))
            {
                Log.Write("Server not found!");
                Log.NewLine(2);
            }

            ProcessStartInfo processStartInfo = new()
            {
                FileName = serverPath,
                Arguments = $"{port}",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };

            Process.Start(processStartInfo);
        }

        public override void SendTo(byte[] data, IPEndPoint ip = null)
        {
            base.SendTo(data, ip);

            if (Crypt.IsCrypted(data))
                data = Crypt.Encrypt(data);

            if (ip == null)
                connection.Send(data);
            else
                connection.Send(data, ip);
        }

        public override void SendData(byte[] data)
        {
            foreach (KeyValuePair<int, Client> client in clients)
                SendTo(data, client.Value.ipEndPoint);
        }

        private List<int> SortedClientsByLevel(List<int> clientsToSort)
        {
            List<int> sortedList = new(clientsToSort);
            sortedList.Sort((a, b) => clients[a].level.CompareTo(clients[b].level));
            return sortedList;
        }
    }
}