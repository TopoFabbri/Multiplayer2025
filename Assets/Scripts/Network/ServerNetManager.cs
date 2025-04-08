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

        private readonly List<Spawnable> spawnedObjects = new();

        public override void Init(int port, IPAddress ip = null)
        {
            Port = port;
            connection = new UdpConnection(port, this);
            
            base.Init(port, ip);
        }

        private void Broadcast(byte[] data)
        {
            foreach (KeyValuePair<int, Client> keyValuePair in clients)
                connection.Send(data, keyValuePair.Value.ipEndPoint);
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
            List<Spawnable> message = new NetSpawnable(data).Deserialized();

            int newId = 0;

            while (spawnedObjects.Any(spawnable => spawnable.id == newId))
                newId++;

            Spawnable last = message.Last();
            last.id = newId;
            
            spawnedObjects.Add(last);
            
            SendData(new NetSpawnable(spawnedObjects).Serialize());
        }

        public override void SendData(byte[] data)
        {
            Broadcast(data);
        }
    }
}