using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetByte : Message<byte>
    {
        public NetByte(byte data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.Byte;
        }

        public NetByte(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.Add(data);
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override byte Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            return message[counter];
        }
    }
}