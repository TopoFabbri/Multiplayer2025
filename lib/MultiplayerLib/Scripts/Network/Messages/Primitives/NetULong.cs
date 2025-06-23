using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetULong : NetBasePrimitive
    {
        public NetULong(ulong data, Flags flags, List<int> path) : base(data, flags, path)
        {
            metadata.Type = MessageType.ULong;
        }

        public NetULong(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(SerializedPath());
            outData.AddRange(BitConverter.GetBytes((ulong)data.data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override PrimitiveNetData Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            PrimitiveNetData outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = BitConverter.ToUInt64(message, counter)
            };

            return outData;
        }
    }
}