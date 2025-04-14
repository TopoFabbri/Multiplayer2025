using System;
using System.Collections.Generic;

namespace Network.Messages
{
    public class NetDisconnect : Message<int>
    {
        public NetDisconnect(int data) : base(data)
        {
            metadata.Type = MessageType.Disconnect;
        }
        
        public NetDisconnect(byte[] data) : base(data)
        {
            metadata.Type = MessageType.Disconnect;
        }
        
        public override byte[] Serialize()
        {
            List<byte> outData = new();
    
            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data));

            return outData.ToArray();
        }

        protected override int Deserialize(byte[] message)
        {
            return BitConverter.ToInt32(message, MessageMetadata.Size);
        }
    }
}