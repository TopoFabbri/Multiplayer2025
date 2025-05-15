using System.Collections.Generic;

namespace Multiplayer.Network.Messages
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
    }
}