using System;
using System.Collections.Generic;
using System.Net;

namespace Multiplayer.Network.Messages
{
    public class ImportantMessageHandler
    {
        private readonly Dictionary<int, MessageIds> acknowledgedMessageIdsBySender = new();
        private readonly Dictionary<int, PendingMessagesList> pendingMessagesBySender = new();
        
        public static event Action<List<PendingMessage>> OnShouldResendMessages; 
        
        public bool ShouldAcknowledge(MessageMetadata metadata)
        {
            if (!metadata.Important)
                return false;
            
            if (!acknowledgedMessageIdsBySender.ContainsKey(metadata.SenderId))
                acknowledgedMessageIdsBySender.Add(metadata.SenderId, new MessageIds());

            acknowledgedMessageIdsBySender[metadata.SenderId].TryAddId(metadata.Id);
            
            return true;
        }
        
        public void UpdatePendingMessages(float time, float timeout)
        {
            foreach (KeyValuePair<int,PendingMessagesList> pendingMessagesOfSender in pendingMessagesBySender)
            {
                List<PendingMessage> messagesToResend = pendingMessagesOfSender.Value.CheckMessages(time, timeout);
                
                if (messagesToResend.Count > 0)
                    OnShouldResendMessages?.Invoke(messagesToResend);
            }
        }

        public void AddPendingMessage(float timeStamp, byte[] message, IPEndPoint endpoint)
        {
            MessageMetadata metadata = MessageMetadata.Deserialize(message);
            
            if (!pendingMessagesBySender.ContainsKey(metadata.SenderId))
                pendingMessagesBySender.Add(metadata.SenderId, new PendingMessagesList());
            
            pendingMessagesBySender[metadata.SenderId].Add(timeStamp, message, endpoint);
        }
        
        public void RemoveSender(int id)
        {
            acknowledgedMessageIdsBySender.Remove(id);
            pendingMessagesBySender.Remove(id);
        }

        public void RemoveMessage(MessageMetadata metadata, Acknowledge acknowledge)
        {
            pendingMessagesBySender[metadata.SenderId].RemoveMessage(acknowledge);
        }
    }
}