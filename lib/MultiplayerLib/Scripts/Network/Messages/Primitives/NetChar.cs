using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetChar : NetPrimitive<char>
    {
        public NetChar(char data, Flags flags, List<int> path) : base(data, flags, path)
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
            outData.AddRange(SerializedPath());
            outData.AddRange(BitConverter.GetBytes(data.data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override Primitive<char> Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            Primitive<char> outData = new()
            {
                path = DeserializePath(message, ref counter),
                data = (char)message[counter]
            };

            return outData;
        }
    }
}