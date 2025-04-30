using System.Collections.Generic;

namespace Network.Messages
{
    public class ImportantMessageHandler
    {
        private readonly Dictionary<int, MessageIds> acknowledgedMessageIdsBySender = new();
        private readonly Dictionary<int, PendingMessages> pendingMessagesBySender = new();
        
        public bool ShouldAcknowledge(MessageMetadata metadata)
        {
            if (!metadata.Important)
                return false;
            
            if (!acknowledgedMessageIdsBySender.ContainsKey(metadata.SenderId))
                acknowledgedMessageIdsBySender.Add(metadata.SenderId, new MessageIds());

            acknowledgedMessageIdsBySender[metadata.SenderId].TryAddId(metadata.Id);
            
            return true;
        }
        
        public void RemoveSender(int id)
        {
            acknowledgedMessageIdsBySender.Remove(id);
            pendingMessagesBySender.Remove(id);
        }
    }
}