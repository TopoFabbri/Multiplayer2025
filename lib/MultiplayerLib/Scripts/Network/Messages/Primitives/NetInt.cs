using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetInt : Message<int>
    {
        public NetInt(int data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.Int;
        }

        public NetInt(byte[] data) : base(data)
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

        protected override int Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            int value = BitConverter.ToInt32(message, counter);

            return value;
        }
    }
}