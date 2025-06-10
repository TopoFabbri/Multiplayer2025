using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetLong : NetPrimitive<long>
    {
        public NetLong(long data, Flags flags, List<int> path) : base(data, flags, path)
        {
            metadata.Type = MessageType.Long;
        }

        public NetLong(byte[] data) : base(data)
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

        protected override Primitive<long> Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            Primitive<long> outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = BitConverter.ToInt64(message, counter)
            };

            return outData;
        }
    }
}