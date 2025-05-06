using System.Collections.Generic;
using System.Net;

namespace Network.Messages
{
    public class PendingMessagesList
    {
        private readonly List<PendingMessage> pendingMessages = new();
        
        public void Add(float timeStamp, byte[] message, IPEndPoint endpoint) => pendingMessages.Add(new PendingMessage(timeStamp, message, endpoint));

        public List<PendingMessage> CheckMessages(float time, float timeout)
        {
            List<PendingMessage> messagesToResend = new();
            
            foreach (PendingMessage pendingMessage in pendingMessages)
            {
                if (!(time - pendingMessage.timeStamp > timeout)) continue;
                
                messagesToResend.Add(pendingMessage);
            }
            
            foreach (PendingMessage pendingMessage in messagesToResend)
            {
                if (pendingMessages.Contains(pendingMessage))
                    pendingMessages.Remove(pendingMessage);
            }
            
            return messagesToResend;
        }
    }
}