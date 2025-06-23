using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetShort : NetBasePrimitive
    {
        public NetShort(short data, Flags flags, List<int> path) : base(data, flags, path)
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
            outData.AddRange(BitConverter.GetBytes((short)data.data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override PrimitiveNetData Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            PrimitiveNetData outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = BitConverter.ToInt16(message, counter)
            };

            return outData;
        }
    }
}