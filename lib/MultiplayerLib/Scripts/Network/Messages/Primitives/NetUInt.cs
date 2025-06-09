using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetUInt : Message<uint>
    {
        public NetUInt(uint data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.UInt;
        }

        public NetUInt(byte[] data) : base(data)
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

        protected override uint Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            return BitConverter.ToUInt32(message, counter);
        }
    }
}