using System.Collections.Generic;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.Primitives;
using Multiplayer.Reflection;

namespace Multiplayer.Network.Objects
{
    public class Model
    {
        private readonly List<int> nodesPath = new();
        
        public Model()
        {
            MessageHandler.TryAddHandler(MessageType.Bool, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.Byte, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.Char, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.Double, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.Float, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.Int, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.Long, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.Short, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.String, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.UInt, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.ULong, AddIncomingData);
            MessageHandler.TryAddHandler(MessageType.UShort, AddIncomingData);
        }

        ~Model()
        {
            MessageHandler.TryRemoveHandler(MessageType.Bool, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.Byte, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.Char, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.Double, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.Float, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.Int, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.Long, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.Short, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.String, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.UInt, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.ULong, AddIncomingData);
            MessageHandler.TryRemoveHandler(MessageType.UShort, AddIncomingData);
        }
        
        public void Update()
        {
            nodesPath.Clear();
            Synchronizer.Synchronize(this, nodesPath);

            while (Synchronizer.HasDirty())
                NetworkManager.Instance.SendData(Synchronizer.DequeueDirty());
        }

        private static void AddIncomingData(byte[] data, IPEndPoint ip)
        {
            MessageType mesType = MessageHandler.GetMetadata(data).Type;
            PrimitiveNetData primitiveData = PrimitiveSerializer.Deserialize(mesType, data);
            Synchronizer.AddIncomingData(primitiveData.path, primitiveData.data);
        }
    }
}