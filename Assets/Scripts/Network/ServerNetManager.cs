using System.Collections.Generic;
using System.Linq;
using System.Net;
using Network.Messages;
using UnityEngine;

namespace Network
{
    public class ServerNetManager : NetworkManager
    {
        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();

        private int clientId = 1;

        private readonly List<SpawnRequest> spawnedObjects = new();

        public override void Init(int port, IPAddress ip = null)
        {
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Console, HandleConsole);
            MessageHandler.TryAddHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnable);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);
            
            Port = port;
            connection = new UdpConnection(port, this);

            ID = 0;

            base.Init(port, ip);
            
            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.Acknowledge, OnAcknowledgePingHandler);
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

        private void Broadcast(byte[] data)
        {
            foreach (KeyValuePair<int, Client> keyValuePair in clients)
                SendTo(data, clients[keyValuePair.Key].ipEndPoint);
        }

        private void SendToClient(byte[] data, int id)
        {
            SendTo(data, clients[id].ipEndPoint);
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

        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            AddClient(ip);
            SendToClient(new NetPing(0f).Serialize(), ipToId[ip]);
        }

        private void HandleConsole(byte[] data, IPEndPoint ip)
        {
            SendData(data);
        }

        private void HandlePosition(byte[] data, IPEndPoint ip)
        {
            SendData(data);
        }

        private void HandleSpawnable(byte[] data, IPEndPoint ip)
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

        private void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
            RemoveClient(ip);
            
            SendData(data);
        }

        public override void SendData(byte[] data)
        {
            Broadcast(data);
        }

        public override void SendTo(byte[] data, IPEndPoint ip = null)
        {
            base.SendTo(data, ip);
            
            connection?.Send(data, ip);
        }

        private void OnAcknowledgePingHandler(byte[] data, IPEndPoint ip)
        {
            float ping = Time.time - clients[ipToId[ip]].lastPingTime;

            Client client = clients[ipToId[ip]];

            client.lastPingTime = Time.time;

            clients[ipToId[ip]] = client;

            SendToClient(new NetPing(ping).Serialize(), ipToId[ip]);
        }

        private void OnDestroy()
        {
            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryRemoveHandler(MessageType.Console, HandleConsole);
            MessageHandler.TryRemoveHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, HandleSpawnable);
            MessageHandler.TryRemoveHandler(MessageType.Disconnect, HandleDisconnect);
        }
    }
}