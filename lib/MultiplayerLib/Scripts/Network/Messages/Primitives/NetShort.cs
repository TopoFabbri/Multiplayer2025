using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetShort : Message<short>
    {
        public NetShort(short data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.Short;
        }

        public NetShort(byte[] data) : base(data)
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

        protected override short Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            return BitConverter.ToInt16(message, counter);
        }
    }
}