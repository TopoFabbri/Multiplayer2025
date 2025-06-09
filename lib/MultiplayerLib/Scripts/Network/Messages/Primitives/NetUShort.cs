using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetUShort : Message<ushort>
    {
        public NetUShort(ushort data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.UShort;
        }

        public NetUShort(byte[] data) : base(data)
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

        protected override ushort Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            return BitConverter.ToUInt16(message, counter);
        }
    }
}