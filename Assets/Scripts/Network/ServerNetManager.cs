using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network.Messages;
using UnityEngine;
using UnityEngine.Serialization;

namespace Network
{
    public class ServerNetManager : NetworkManager
    {
        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();

        private int clientId = 1;

        private readonly List<SpawnRequest> spawnedObjects = new();

        public Dictionary<string, int> log = new();

        public override void Init(int port, IPAddress ip = null)
        {
            Port = port;
            connection = new UdpConnection(port, this);

            ID = 0;

            base.Init(port, ip);
        }

        protected override void Update()
        {
            base.Update();

            List<IPEndPoint> disconnected = new();

            foreach (KeyValuePair<int, Client> client in clients)
            {
                float timeSinceLastPing = Time.time - client.Value.lastPingTime;

                if (timeSinceLastPing > TimeOut)
                    disconnected.Add(client.Value.ipEndPoint);
            }

            foreach (IPEndPoint ipEndPoint in disconnected)
                RemoveClient(ipEndPoint);
        }

        public override void OnReceiveData(byte[] data, IPEndPoint ip)
        {
            base.OnReceiveData(data, ip);
            
            string logMessage = "Received: " + MessageHandler.GetMessageType(data);

            if (log.ContainsKey(logMessage))
            {
                if (log[logMessage] < 99)
                    log[logMessage]++;
            }
            else
            {
                log[logMessage] = 1;
            }

        }

        private void Broadcast(byte[] data)
        {
            foreach (KeyValuePair<int, Client> keyValuePair in clients)
                SendToClient(data, keyValuePair.Key);
        }

        private void SendToClient(byte[] data, int id)
        {
            connection.Send(data, clients[id].ipEndPoint);

            string logMessage = "Sent: " + MessageHandler.GetMessageType(data) + " to: " + id;

            if (log.ContainsKey(logMessage))
            {
                if (log[logMessage] < 99)
                    log[logMessage]++;
            }
            else
            {
                log[logMessage] = 1;
            }
        }

        private void AddClient(IPEndPoint ip)
        {
            if (ipToId.ContainsKey(ip)) return;

            Debug.Log("Adding client: " + ip.Address);

            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, clientId, Time.realtimeSinceStartup));

            clientId++;

            SendData(new NetHandShake(clients.Select(keyValuePair => keyValuePair.Key).ToList()).Serialize());
        }

        private void RemoveClient(IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;

            Debug.Log("Removing client: " + ip.Address);

            ipToId.Remove(ip);
            clients.Remove(id);

            SendData(new NetHandShake(clients.Select(keyValuePair => keyValuePair.Key).ToList()).Serialize());
        }

        protected override void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            AddClient(ip);
            SendToClient(new NetPing(0f).Serialize(), ipToId[ip]);
        }

        protected override void HandleConsole(byte[] data, IPEndPoint ip)
        {
            base.HandleConsole(data, ip);

            SendData(data);
        }

        protected override void HandlePosition(byte[] data, IPEndPoint ip)
        {
            SendData(data);
        }

        protected override void HandleSpawnable(byte[] data, IPEndPoint ip)
        {
            List<SpawnRequest> message = new NetSpawnable(data).Deserialized();

            int newId = 0;

            while (spawnedObjects.Any(spawnable => spawnable.id == newId))
                newId++;

            SpawnRequest last = message.Last();
            last.id = newId;

            spawnedObjects.Add(last);

            SendData(new NetSpawnable(spawnedObjects).Serialize());
        }

        protected override void HandlePing(byte[] data, IPEndPoint ip)
        {
            float ping = Time.time - clients[ipToId[ip]].lastPingTime;

            Client client = clients[ipToId[ip]];

            client.lastPingTime = Time.time;

            clients[ipToId[ip]] = client;

            SendToClient(new NetPing(ping).Serialize(), ipToId[ip]);
        }
        
        protected override void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
            RemoveClient(ip);
            
            SendData(data);
        }

        public override void SendData(byte[] data)
        {
            Broadcast(data);
        }

        private void OnDestroy()
        {
            connection.Close();
        }
    }
}