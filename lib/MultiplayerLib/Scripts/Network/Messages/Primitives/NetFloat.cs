using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetFloat : Message<float>
    {
        public NetFloat(float data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.Float;
        }

        public NetFloat(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override float Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            return BitConverter.ToSingle(message, counter);
        }
    }
}