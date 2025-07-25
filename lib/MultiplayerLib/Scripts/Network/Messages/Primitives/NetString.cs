using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetString : NetBasePrimitive
    {
        public NetString(string data, Flags flags, List<int> path) : base(data, flags, path)
        {
            metadata.Type = MessageType.String;
        }

        public NetString(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(SerializedPath());
            outData.AddRange(BitConverter.GetBytes(((string)data.data).Length * 2));
            outData.AddRange(System.Text.Encoding.Unicode.GetBytes((string)data.data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override PrimitiveNetData Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            PrimitiveNetData outData = new()
            {
                path = DeserializePath(message, ref counter)
            };
            
            int dataSize = BitConverter.ToInt32(message, counter);
            counter += sizeof(int);
            
            outData.data = System.Text.Encoding.Unicode.GetString(message, counter, dataSize);

            return outData;
        }
    }
}