using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetBool : NetPrimitive<bool>
    {
        public NetBool(bool data, Flags flags, List<int> path) : base(data, flags, path)
        {
            metadata.Type = MessageType.Bool;
        }

        public NetBool(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(SerializedPath());
            outData.Add(Convert.ToByte(data.data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override Primitive<bool> Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            Primitive<bool> outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = Convert.ToBoolean(message[counter])
            };

            return outData;
        }
    }
}