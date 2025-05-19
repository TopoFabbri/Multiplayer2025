using System.Collections.Generic;
using System.Linq;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Network.Objects;
using Multiplayer.NetworkFactory;
using Multiplayer.Utils;

namespace Multiplayer.Network
{
    public class ServerNetManager : NetworkManager
    {
        private readonly Dictionary<int, Client> clients = new();
        private readonly Dictionary<IPEndPoint, int> ipToId = new();

        private readonly List<IPEndPoint> disconnectedClients = new();
        private readonly Dictionary<int, Color> colorsByClientId = new();

        private readonly ObjectManager objectManager = new();

        public override void Init(int port, IPAddress ip = null)
        {
            MessageHandler.TryAddHandler(MessageType.HandShake, HandleHandshake);
            MessageHandler.TryAddHandler(MessageType.Console, HandleConsole);
            MessageHandler.TryAddHandler(MessageType.Position, HandlePosition);
            MessageHandler.TryAddHandler(MessageType.Rotation, HandleRotation);
            MessageHandler.TryAddHandler(MessageType.Crouch, HandleCrouch);
            MessageHandler.TryAddHandler(MessageType.Jump, HandleJump);
            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);
            MessageHandler.TryAddHandler(MessageType.Hit, HandleHit);
            MessageHandler.TryAddHandler(MessageType.Despawn, HandleDespawn);

            Port = port;
            connection = new UdpConnection(port, this);

            Id = 0;

            base.Init(port, ip);
            onConnectionEstablished?.Invoke();

            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.Ping, OnAcknowledgePingHandler);
            MessageHandler.TryAddOnAcknowledgeHandler(MessageType.HandShake, OnAcknowledgeHandshakeHandler);

            Log.Write("Server running at port " + port);
            Log.NewLine(2);

            CheckSum.RandomSeed = (uint)Timer.Time;
            CheckSum.CreateOperationsArrays(CheckSum.RandomSeed);
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

        private void AddClient(IPEndPoint ip, Color color)
        {
            if (ipToId.ContainsKey(ip)) return;

            int clientId = 1;

            while (clients.ContainsKey(clientId))
                clientId++;

            Log.Write("Adding client: " + clientId);
            Log.NewLine();

            ipToId[ip] = clientId;

            clients.Add(clientId, new Client(ip, clientId, Timer.Time));
            colorsByClientId.Add(clientId, color);

            HandShake hs = new(CheckSum.RandomSeed, colorsByClientId, true);
            SendData(new NetHandShake(hs, true).Serialize());
        }

        private void RemoveClient(IPEndPoint ip)
        {
            if (!ipToId.TryGetValue(ip, out int id)) return;

            Log.Write("Removing client: " + id);
            Log.NewLine();

            ipToId.Remove(ip);
            clients.Remove(id);
            
            HandShake hs = new(CheckSum.RandomSeed, colorsByClientId, true);
        }

        private void HandleHandshake(byte[] data, IPEndPoint ip)
        {
            HandShake hs = new NetHandShake(data).Deserialized();
            
            AddClient(ip, hs.clients.Last().Value);
        }

        private void HandleConsole(byte[] data, IPEndPoint ip)
        {
            SendData(data);
        }

        private void HandlePosition(byte[] data, IPEndPoint ip)
        {
            Position pos = new NetPosition(data).Deserialized();
            
            objectManager.MoveObjectTo(pos.objId, pos.position.x, pos.position.y, pos.position.z);
            
            SendData(new NetPosition(pos).Serialize());
        }

        private void HandleRotation(byte[] data, IPEndPoint ip)
        {
            Rotation rot = new NetRotation(data).Deserialized();
            
            objectManager.RotateObjectTo(rot.objId, rot.rotation);
            
            SendData(new NetRotation(rot).Serialize());
        }
        
        private void HandleCrouch(byte[] data, IPEndPoint ip)
        {
            SendData(data);
        }
        
        private void HandleJump(byte[] data, IPEndPoint ip)
        {
            SendData(data);
        }
        
        private void HandleSpawnRequest(byte[] data, IPEndPoint ip)
        {
            SpawnRequest message = new NetSpawnable(data).Deserialized();
            
            foreach (SpawnableObjectData spawnableObj in message.spawnableObjects)
            {
                spawnableObj.Id = objectManager.FreeId;
                objectManager.SpawnObject(spawnableObj);
            }

            SendData(new NetSpawnable(new SpawnRequest(objectManager.SpawnablesData)).Serialize());
        }

        private void HandleDisconnect(byte[] data, IPEndPoint ip)
        {
            RemoveClient(ip);

            SendData(data);
        }

        private void HandleHit(byte[] data, IPEndPoint ip)
        {
            Hit hit = new NetHit(data).Deserialized();
            
            NetDespawn despawn = new(hit.bulletObjId);
            
            SendData(despawn.Serialize());
            SendData(data);
        }

        private void HandleDespawn(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = MessageMetadata.Deserialize(data);
            int destroyedId = new NetDespawn(data).Deserialized();
            
            Log.Write("Despawned object: " + destroyedId + " from client " + metadata.SenderId + "'s request");
            Log.NewLine();
            
            if (objectManager.GetSpawnableData(destroyedId).OwnerId != metadata.SenderId)
                return;
            
            objectManager.DestroyObject(destroyedId);
            
            SendData(new NetDespawn(destroyedId).Serialize());
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
            MessageHandler.TryRemoveHandler(MessageType.Rotation, HandleRotation);
            MessageHandler.TryRemoveHandler(MessageType.Crouch, HandleCrouch);
            MessageHandler.TryRemoveHandler(MessageType.Jump, HandleJump);
            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryRemoveHandler(MessageType.Disconnect, HandleDisconnect);
            MessageHandler.TryRemoveHandler(MessageType.Hit, HandleHit);
            MessageHandler.TryRemoveHandler(MessageType.Despawn, HandleDespawn);
        }
    }
}