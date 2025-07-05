using System.Collections.Generic;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.NetworkFactory;

namespace Multiplayer.Network
{
    public class NonAuthoritativeServer : ServerNetManager
    {
        public override void Init(int port, IPAddress ip = null, string name = "Player")
        {
            base.Init(port, ip, name);

            MessageHandler.TryAddHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryAddHandler(MessageType.Disconnect, HandleDisconnect);
            MessageHandler.TryAddHandler(MessageType.Despawn, HandleDespawn);
            MessageHandler.TryAddHandler(MessageType.Action, HandleRpc);

            MessageHandler.TryAddHandler(MessageType.Bool, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.Byte, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.Char, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.Double, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.Float, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.Int, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.Long, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.Short, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.String, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.UInt, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.ULong, HandlePrimitive);
            MessageHandler.TryAddHandler(MessageType.UShort, HandlePrimitive);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            MessageHandler.TryRemoveHandler(MessageType.SpawnRequest, HandleSpawnRequest);
            MessageHandler.TryRemoveHandler(MessageType.Disconnect, HandleDisconnect);
            MessageHandler.TryRemoveHandler(MessageType.Despawn, HandleDespawn);
            MessageHandler.TryRemoveHandler(MessageType.Action, HandleRpc);

            MessageHandler.TryRemoveHandler(MessageType.Bool, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.Byte, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.Char, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.Double, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.Float, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.Int, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.Long, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.Short, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.String, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.UInt, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.ULong, HandlePrimitive);
            MessageHandler.TryRemoveHandler(MessageType.UShort, HandlePrimitive);
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
        }

        private void HandleDespawn(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = MessageMetadata.Deserialize(data);
            int destroyedId = new NetDespawn(data).Deserialized();

            if (objectManager.GetSpawnableData(destroyedId).OwnerId != metadata.SenderId)
                return;

            objectManager.DestroyObject(destroyedId);

            SendData(new NetDespawn(destroyedId).Serialize());
        }

        private void HandleRpc(byte[] data, IPEndPoint ip)
        {
            SendWithException(data, new List<IPEndPoint> { ip });
        }

        private void HandlePrimitive(byte[] data, IPEndPoint ip)
        {
            SendWithException(data, new List<IPEndPoint> { ip });
        }
    }
}