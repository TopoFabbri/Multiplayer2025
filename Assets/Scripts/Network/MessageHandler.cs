using System;
using System.Collections.Generic;
using System.Net;
using Network.Messages;

namespace Network
{
    public static class MessageHandler
    {
        private static readonly Dictionary<MessageType, Action<byte[], IPEndPoint>> Handlers = new();

        public static bool TryAddHandler(MessageType type, Action<byte[], IPEndPoint> handler)
        {
            return Handlers.TryAdd(type, handler);
        }
        
        public static void Receive(byte[] data, IPEndPoint ip)
        {
            if (Handlers.ContainsKey(GetMetadata(data).Type))
                Handlers[GetMetadata(data).Type]?.Invoke(data, ip);
        }

        public static MessageMetadata GetMetadata(byte[] data)
        {
            return MessageMetadata.Deserialize(data);
        }
    }
}