using System.Net;

namespace Multiplayer.Network.Messages.MessageInfo
{
    public class PendingMessage
    {
        public readonly float timeStamp;
        public readonly byte[] message;
        public readonly IPEndPoint ip;
        
        public PendingMessage(float timeStamp, byte[] message, IPEndPoint ip)
        {
            this.timeStamp = timeStamp;
            this.message = message;
            this.ip = ip;
        }
    }
}