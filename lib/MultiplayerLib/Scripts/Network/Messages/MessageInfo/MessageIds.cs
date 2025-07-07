using System.Collections.Generic;

namespace Multiplayer.Network.Messages.MessageInfo
{
    public class MessageIds
    {
        private readonly List<int> messageIds = new();

        public bool TryAddId(int id)
        {
            if (messageIds.Contains(id))
                return false;
            
            messageIds.Add(id);

            return true;
        }
        
        public bool ContainsId(int id)
        {
            return messageIds.Contains(id);
        }
    }
}