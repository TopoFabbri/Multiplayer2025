using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public static class PrimitiveSerializer
    {
        // The mapping of System.Type to a factory delegate for IPrimitiveSerializer.
        private static readonly Dictionary<Type, Func<object, Flags, List<int>, ISerializable>> ConstructorsByType =
            new()
            {
                { typeof(int), (data, flags, path) => new NetInt((int)data, flags, path) },
                { typeof(char), (data, flags, path) => new NetChar((char)data, flags, path) },
                { typeof(float), (data, flags, path) => new NetFloat((float)data, flags, path) },
                { typeof(double), (data, flags, path) => new NetDouble((double)data, flags, path) },
                { typeof(bool), (data, flags, path) => new NetBool((bool)data, flags, path) },
                { typeof(string), (data, flags, path) => new NetString((string)data, flags, path) },
            };

        public static byte[] Serialize<T>(T data, Flags flags, List<int> path)
        {
            Type type = typeof(T);
            
            if (!ConstructorsByType.TryGetValue(type, out Func<object, Flags, List<int>, ISerializable> constructor))
                throw new NotSupportedException($"Type {type} is not supported for serialization.");
            
            ISerializable serializerInstance = constructor(data, flags, path);
            return serializerInstance.Serialize();
        }
    }
}