using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network
{
    public class CriticMessagesHandler
    {
        private readonly List<byte[]> messages = new();
        
        public void HandleCriticMessages(byte[] data)
        {
            if (MessageMetadata.Deserialize(data).Flags.HasFlag(Flags.Critical))
                messages.Add(data);
        }
    }
}