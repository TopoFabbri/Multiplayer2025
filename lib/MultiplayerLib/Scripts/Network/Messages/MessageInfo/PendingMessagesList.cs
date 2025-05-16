using System.Collections.Generic;
using System.Net;

namespace Multiplayer.Network.Messages.MessageInfo
{
    public class PendingMessagesList
    {
        private readonly Dictionary<int, PendingMessage> pendingMessagesById = new();

        public void Add(float timeStamp, byte[] message, IPEndPoint endpoint)
        {
            MessageMetadata metadata = MessageMetadata.Deserialize(message);
            
            if (!pendingMessagesById.ContainsKey(metadata.MsgId))
                pendingMessagesById.Add(metadata.MsgId, new PendingMessage(timeStamp, message, endpoint));
        }

        public List<PendingMessage> CheckMessages(float time, float timeout)
        {
            List<PendingMessage> messagesToResend = new();
            
            foreach (KeyValuePair<int, PendingMessage> pendingMessage in pendingMessagesById)
            {
                if (!(time - pendingMessage.Value.timeStamp > timeout)) continue;
                
                messagesToResend.Add(pendingMessage.Value);
            }
            
            foreach (PendingMessage pendingMessage in messagesToResend)
            {
                pendingMessagesById.Remove(MessageMetadata.Deserialize(pendingMessage.message).MsgId);
            }
            
            return messagesToResend;
        }

        public void RemoveMessage(Acknowledge acknowledge)
        {
            pendingMessagesById.Remove(acknowledge.mesId);
        }
    }
}