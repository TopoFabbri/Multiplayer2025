using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public static class PrimitiveSerializer
    {
        private static readonly Dictionary<Type, Func<object, Flags, List<int>, ISerializable>> ConstructorsByType =
            new()
            {
                { typeof(bool), (data, flags, path) => new NetBool((bool)data, flags, path) },
                { typeof(byte), (data, flags, path) => new NetByte((byte)data, flags, path) },
                { typeof(char), (data, flags, path) => new NetChar((char)data, flags, path) },
                { typeof(double), (data, flags, path) => new NetDouble((double)data, flags, path) },
                { typeof(float), (data, flags, path) => new NetFloat((float)data, flags, path) },
                { typeof(int), (data, flags, path) => new NetInt((int)data, flags, path) },
                { typeof(long), (data, flags, path) => new NetLong((long)data, flags, path) },
                { typeof(short), (data, flags, path) => new NetShort((short)data, flags, path) },
                { typeof(string), (data, flags, path) => new NetString((string)data, flags, path) },
                { typeof(uint), (data, flags, path) => new NetUInt((uint)data, flags, path) },
                { typeof(ulong), (data, flags, path) => new NetULong((ulong)data, flags, path) },
                { typeof(ushort), (data, flags, path) => new NetUShort((ushort)data, flags, path) }
            };

        private static readonly Dictionary<MessageType, Func<byte[], PrimitiveNetData>> DeserializationHandlers =
            new()
            {
                { MessageType.Bool, data => new NetBool(data).Deserialized() },
                { MessageType.Byte, data => new NetByte(data).Deserialized() },
                { MessageType.Char, data => new NetChar(data).Deserialized() },
                { MessageType.Double, data => new NetDouble(data).Deserialized() },
                { MessageType.Float, data => new NetFloat(data).Deserialized() },
                { MessageType.Int, data => new NetInt(data).Deserialized() },
                { MessageType.Long, data => new NetLong(data).Deserialized() },
                { MessageType.Short, data => new NetShort(data).Deserialized() },
                { MessageType.String, data => new NetString(data).Deserialized() },
                { MessageType.UInt, data => new NetUInt(data).Deserialized() },
                { MessageType.ULong, data => new NetULong(data).Deserialized() },
                { MessageType.UShort, data => new NetUShort(data).Deserialized() }
            };

        public static byte[] Serialize(object data, Flags flags, List<int> path)
        {
            Type type = data.GetType();
            
            if (!ConstructorsByType.TryGetValue(type, out Func<object, Flags, List<int>, ISerializable> constructor))
                throw new NotSupportedException($"Type {type} is not supported for serialization.");
            
            ISerializable serializerInstance = constructor(data, flags, path);
            return serializerInstance.Serialize();
        }
        
        public static PrimitiveNetData Deserialize(MessageType mesType, byte[] data)
        {
            if (!DeserializationHandlers.TryGetValue(mesType, out Func<byte[], PrimitiveNetData> handler))
                throw new NotSupportedException($"Message type {mesType} is not supported for deserialization.");
            
            return handler(data);
        }
    }
}