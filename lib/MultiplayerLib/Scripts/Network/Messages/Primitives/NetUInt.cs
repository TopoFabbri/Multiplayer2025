using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetUInt : NetPrimitive<uint>
    {
        public NetUInt(uint data, Flags flags, List<int> path) : base(data, flags, path)
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
            outData.AddRange(SerializedPath());
            outData.AddRange(BitConverter.GetBytes(data.data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override Primitive<uint> Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            Primitive<uint> outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = BitConverter.ToUInt32(message, counter)
            };

            return outData;
        }
    }
}