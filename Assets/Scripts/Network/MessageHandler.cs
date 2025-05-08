using System;
using System.Collections.Generic;
using System.Net;
using Network.Messages;
using UnityEngine;

namespace Network
{
    public static class MessageHandler
    {
        private static readonly Dictionary<MessageType, Action<byte[], IPEndPoint>> Handlers = new();
        private static readonly Dictionary<MessageType, ImportantMessageHandler> ImportantMessageHandlersByMessageType = new();
        private static readonly Dictionary<MessageType, Action<byte[], IPEndPoint>> OnAcknowledgedByMessageType = new();

        private const float Timeout = .5f;

        public static void TryAddHandler(MessageType type, Action<byte[], IPEndPoint> handler)
        {
            if (!Handlers.ContainsKey(type))
                Handlers.TryAdd(type, handler);
            else
                Handlers[type] += handler;
        }
        
        public static void Receive(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = GetMetadata(data);

            CheckReceivedImportantMessage(metadata, ip);

            if (Handlers.TryGetValue(metadata.Type, out Action<byte[], IPEndPoint> handler))
                handler?.Invoke(data, ip);
            
            foreach (KeyValuePair<MessageType, ImportantMessageHandler> importantMessageHandler in ImportantMessageHandlersByMessageType)
                importantMessageHandler.Value.UpdatePendingMessages(Time.time, Timeout);
        }

        public static void OnSendData(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = GetMetadata(data);
            
            if (!metadata.Important) return;
            
            if (!ImportantMessageHandlersByMessageType.ContainsKey(metadata.Type))
                ImportantMessageHandlersByMessageType.Add(metadata.Type, new ImportantMessageHandler());
            
            
        }
        
        public static MessageMetadata GetMetadata(byte[] data)
        {
            return MessageMetadata.Deserialize(data);
        }

        public static void HandleAcknowledge(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = GetMetadata(data);
            
            if (OnAcknowledgedByMessageType.TryGetValue(metadata.Type, out Action<byte[], IPEndPoint> onAcknowledge))
                onAcknowledge?.Invoke(data, ip);
        }
        
        public static void TryAddOnAcknowledgeHandler(MessageType type, Action<byte[], IPEndPoint> handler)
        {
            if (!OnAcknowledgedByMessageType.ContainsKey(type))
                OnAcknowledgedByMessageType.TryAdd(type, handler);
            else
                OnAcknowledgedByMessageType[type] += handler;
        }
        
        private static void CheckReceivedImportantMessage(MessageMetadata metadata, IPEndPoint ip)
        {
            ImportantMessageHandlersByMessageType.TryAdd(metadata.Type, new ImportantMessageHandler());

            if (!ImportantMessageHandlersByMessageType[metadata.Type].ShouldAcknowledge(metadata)) return;

            Acknowledge acknowledge = new() { mesId = metadata.Id, senderId = metadata.SenderId, mesType = metadata.Type };

            NetworkManager.Instance.SendTo(new NetAcknowledge(acknowledge).Serialize());
        }
    }
}