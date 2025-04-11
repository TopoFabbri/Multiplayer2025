using System;
using System.Collections.Generic;
using System.Net;

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
            Handlers[GetMessageType(data)]?.Invoke(data, ip);
        }

        public static MessageType GetMessageType(byte[] data)
        {
            return MessageMetadata.Deserialize(data).Type;
        }
    }
}