using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetUShort : NetBasePrimitive
    {
        public NetUShort(ushort data, Flags flags, List<int> path) : base(data, flags, path)
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
            outData.AddRange(SerializedPath());
            outData.AddRange(BitConverter.GetBytes((ushort)data.data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override PrimitiveNetData Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            PrimitiveNetData outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = BitConverter.ToUInt16(message, counter)
            };

            return outData;
        }
    }
}