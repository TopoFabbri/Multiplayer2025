using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetChar : Message<char>
    {
        public NetChar(char data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.Char;
        }

        public NetChar(byte[] data) : base(data)
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

        protected override char Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            return BitConverter.ToChar(message, counter);
        }
    }
}