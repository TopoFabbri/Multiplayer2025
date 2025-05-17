using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Timer = Multiplayer.Utils.Timer;

namespace Multiplayer.Network
{
    public class MatchMakerNetManager : NetworkManager
    {
        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();
        private readonly List<int> redirectedClients = new();

        private readonly List<int> openServers = new();
        
        private string serverPath;

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
            
            Console.WriteLine("Started MatchMaker at port: " + port);

            CheckSum.RandomSeed = (uint)Timer.Time;
            CheckSum.CreateOperationsArrays(CheckSum.RandomSeed);
        }

        public override void Update()
        {
            base.Update();

            if (clients.Count < 2) return;

            int clientsWaiting = 0;
            
            foreach (KeyValuePair<int, Client> client in clients)
            {
                if (!redirectedClients.Contains(client.Key))
                    clientsWaiting++;
            }
            
            if (clientsWaiting < 2) return;
            
            List<Client> clientsToConnect = new();

            foreach (KeyValuePair<int, Client> client in clients)
            {
                clientsToConnect.Add(client.Value);
                redirectedClients.Add(client.Key);
            }
                
            OpenServer(clientsToConnect);
        }

        private void HandleHandShake(byte[] data, IPEndPoint ip)
        {
            int clientId = 1;
            
            while (clients.ContainsKey(clientId))
                clientId++;
            
            clients.Add(clientId, new Client(ip, clientId, Timer.Time));
            ipToId.Add(ip, clientId);
            
            HandShake hs = new(CheckSum.RandomSeed, clients.Keys.ToList(), false);
            SendTo(new NetHandShake(hs, false).Serialize(), ip);
        }

        private void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
            if (ipToId.ContainsKey(ip))
            {
                int clientId = ipToId[ip];
                
                clients.Remove(clientId);
                redirectedClients.Remove(ipToId[ip]);
                ipToId.Remove(ip);
            }
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
                Console.WriteLine("Server not found!");

            ProcessStartInfo processStartInfo = new()
            {
                FileName = serverPath,
                Arguments = $"{port}",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };
            
            Process process = Process.Start(processStartInfo);
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