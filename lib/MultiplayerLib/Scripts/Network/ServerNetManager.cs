using System;
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

        private int clientId = 1;

        private readonly Dictionary<int, SpawnRequest> spawnedObjectsById = new();

        private readonly List<IPEndPoint> disconnectedClients = new();

        public override void Init(int port, IPAddress ip = null)
        {
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Console, HandleConsole);
            MessageHandler.TryAddHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnable);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);
            
            Port = port;
            connection = new UdpConnection(port, this);

            Id = 0;

            base.Init(port, ip);
            onConnectionEstablished?.Invoke();
            
            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.Ping, OnAcknowledgePingHandler);
            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.HandShake, OnAcknowledgeHandshakeHandler);
            
            Console.WriteLine("Server running at port " + port);
            
            CheckSum.RandomSeed = (uint)Timer.Time;
            CheckSum.CreateOperationsArrays((int)CheckSum.RandomSeed);
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

            Console.WriteLine("Adding client: " + clientId);

            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, clientId, Timer.Time));

            clientId++;

            HandShake hs = new(CheckSum.RandomSeed, clients.Select(keyValuePair => keyValuePair.Key).ToList());
            SendData(new NetHandShake(hs, true).Serialize());
        }

        private void RemoveClient(IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;

            Console.WriteLine("Removing client: " + id);

            ipToId.Remove(ip);
            clients.Remove(id);

            HandShake hs = new(CheckSum.RandomSeed, clients.Select(keyValuePair => keyValuePair.Key).ToList());
            SendData(new NetHandShake(hs, true).Serialize());
        }

        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            AddClient(ip);
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

            while (spawnedObjectsById.ContainsKey(newId))
                newId++;

            SpawnRequest last = message.Last();
            last.id = newId;

            spawnedObjectsById.Add(last.id, last);

            SendData(new NetSpawnable(spawnedObjectsById.Values.ToList()).Serialize());
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
            float ping = Timer.Time - clients[ipToId[ip]].lastPingTime;

            Client client = clients[ipToId[ip]];

            client.lastPingTime = Timer.Time;

            clients[ipToId[ip]] = client;

            SendToClient(new NetPing(ping).Serialize(), ipToId[ip]);
        }

        private void OnAcknowledgeHandshakeHandler(byte[] data, IPEndPoint ip)
        {
            SendToClient(new NetPing(0f).Serialize(), ipToId[ip]);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            MessageHandler.TryRemoveHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryRemoveHandler(MessageType.Console, HandleConsole);
            MessageHandler.TryRemoveHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, HandleSpawnable);
            MessageHandler.TryRemoveHandler(MessageType.Disconnect, HandleDisconnect);
        }
    }
}