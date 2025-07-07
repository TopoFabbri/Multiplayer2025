using System;
using System.Collections.Generic;
using System.Net;
using Multiplayer.Network.Messages;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.Utils;

namespace Multiplayer.Network
{
    public static class MessageHandler
    {
        private static readonly Dictionary<MessageType, Action<byte[], IPEndPoint>> Handlers = new();
        private static readonly Dictionary<MessageType, ImportantMessageHandler> ImportantMessageHandlersByMessageType = new();
        private static readonly Dictionary<MessageType, Action<byte[], IPEndPoint>> OnAcknowledgedByMessageType = new();

        private static CriticMessagesHandler _criticMessagesHandler = new();

        public static Action<MessageMetadata, IPEndPoint> onShouldAcknowledge;

        private const float Timeout = 1f;

        public static void TryAddHandler(MessageType type, Action<byte[], IPEndPoint> handler)
        {
            if (!Handlers.ContainsKey(type))
                Handlers.TryAdd(type, handler);
            else
                Handlers[type] += handler;
        }

        public static void TryRemoveHandler(MessageType type, Action<byte[], IPEndPoint> handler)
        {
            if (Handlers.ContainsKey(type))
                Handlers[type] -= handler;
        }

        public static void Receive(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = GetMetadata(data);

            if (metadata.Flags.HasFlag(Flags.Checksum))
            {
                if (!CheckSum.IsValid(data))
                    return;
            }

            if (Handlers.TryGetValue(metadata.Type, out Action<byte[], IPEndPoint> handler))
                handler?.Invoke(data, ip);

            CheckReceivedImportantMessage(metadata, ip);
            
            foreach (KeyValuePair<MessageType, ImportantMessageHandler> importantMessageHandler in ImportantMessageHandlersByMessageType)
                importantMessageHandler.Value.UpdatePendingMessages(Timer.Time, Timeout);
        }

        public static void OnSendData(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = GetMetadata(data);

            if (!metadata.Flags.HasFlag(Flags.Important)) return;

            if (!ImportantMessageHandlersByMessageType.ContainsKey(metadata.Type))
                ImportantMessageHandlersByMessageType.Add(metadata.Type, new ImportantMessageHandler());

            ImportantMessageHandlersByMessageType[metadata.Type].AddPendingMessage(Timer.Time, data, ip);

            _criticMessagesHandler.HandleCriticMessages(data);
        }

        public static MessageMetadata GetMetadata(byte[] data)
        {
            return MessageMetadata.Deserialize(data);
        }

        public static void HandleAcknowledge(byte[] data, IPEndPoint ip)
        {
            MessageMetadata metadata = GetMetadata(data);
            Acknowledge acknowledge = new NetAcknowledge(data).Deserialized();

            ImportantMessageHandlersByMessageType[acknowledge.mesType].RemoveMessage(metadata, acknowledge);

            if (OnAcknowledgedByMessageType.TryGetValue(acknowledge.mesType, out Action<byte[], IPEndPoint> onAcknowledge))
                onAcknowledge?.Invoke(data, ip);
        }

        public static void TryAddOnAcknowledgeHandler(MessageType type, Action<byte[], IPEndPoint> handler)
        {
            if (!OnAcknowledgedByMessageType.ContainsKey(type))
                OnAcknowledgedByMessageType.TryAdd(type, handler);
            else
                OnAcknowledgedByMessageType[type] += handler;
        }

        public static void TryRemoveOnAcknowledgeHandler(MessageType type, Action<byte[], IPEndPoint> handler)
        {
            if (OnAcknowledgedByMessageType.ContainsKey(type))
                OnAcknowledgedByMessageType[type] -= handler;
        }

        private static void CheckReceivedImportantMessage(MessageMetadata metadata, IPEndPoint ip)
        {
            ImportantMessageHandlersByMessageType.TryAdd(metadata.Type, new ImportantMessageHandler());

            if (!ImportantMessageHandlersByMessageType[metadata.Type].ShouldAcknowledge(metadata)) return;

            onShouldAcknowledge?.Invoke(metadata, ip);
        }
    }
}