using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetFloat : NetPrimitive<float>
    {
        public NetFloat(float data, Flags flags, List<int> path) : base(data, flags, path)
        {
            metadata.Type = MessageType.Float;
        }

        public NetFloat(byte[] data) : base(data)
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

        protected override Primitive<float> Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            Primitive<float> outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = BitConverter.ToSingle(message, counter)
            };

            return outData;
        }
    }
}