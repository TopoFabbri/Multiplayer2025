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
        private readonly List<int> redirectedClients = new();
        private readonly List<int> readyClients = new();
        private readonly List<int> openServers = new();

        private string serverPath;
        private readonly List<IPEndPoint> disconnectedClients = new();
        private readonly Dictionary<int, Color> colorsByClientId = new();

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

            CheckSum.RandomSeed = (uint)Timer.Time;
            CheckSum.CreateOperationsArrays(CheckSum.RandomSeed);
        }

        public override void Update()
        {
            base.Update();

            if (readyClients.Count < 2) return;

            List<Client> clientsToConnect = new();

            foreach (int clientId in readyClients)
            {
                if (redirectedClients.Contains(clientId)) continue;
                
                redirectedClients.Add(clientId);
                clientsToConnect.Add(clients[clientId]);
            }
            
            if (clientsToConnect.Count < 2) return;

            OpenServer(clientsToConnect);

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
            AddClient(ip);
        }

        private void AddClient(IPEndPoint ip)
        {
            int clientId = 1;

            while (clients.ContainsKey(clientId))
                clientId++;

            clients.Add(clientId, new Client(ip, clientId, Timer.Time));
            ipToId.Add(ip, clientId);
            colorsByClientId.Add(clientId, new Color());

            HandShake hs = new(CheckSum.RandomSeed, colorsByClientId, false);
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
            readyClients.Add(ipToId[ip]);
        }

        private void RemoveClient(IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int clientId)) return;

            readyClients.Remove(clientId);
            redirectedClients.Remove(clientId);
            clients.Remove(clientId);
            ipToId.Remove(ip);
            Log.Write("Client " + clientId + " disconnected!");
            Log.NewLine();
            LogConnectedClients();
        }

        private void HandleAcknowledgedHs(byte[] data, IPEndPoint ip)
        {
            SendTo(new NetPing(0f).Serialize(), ip);
        }

        private void HandleAcknowledgedPing(byte[] data, IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;
            float ping = Timer.Time - clients[id].lastPingTime;

            Client client = clients[id];

            client.lastPingTime = Timer.Time;

            clients[id] = client;

            SendTo(new NetPing(ping).Serialize(), ip);
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

            if (ip == null)
                connection.Send(data);
            else
                connection.Send(data, ip);
        }

        public override void SendData(byte[] data)
        {
            connection?.Send(data);
        }
    }
}