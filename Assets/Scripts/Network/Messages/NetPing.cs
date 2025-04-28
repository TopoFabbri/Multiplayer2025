using System;
using System.Collections.Generic;

namespace Network.Messages
{
    public class NetPing : Message<float>
    {
        public NetPing(float data) : base(data)
        {
            metadata.Type = MessageType.Ping;
        }

        public NetPing(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data));

            return outData.ToArray();
        }

        protected override float Deserialize(byte[] message)
        {
            return BitConverter.ToSingle(message, MessageMetadata.Size);
        }
    }
}