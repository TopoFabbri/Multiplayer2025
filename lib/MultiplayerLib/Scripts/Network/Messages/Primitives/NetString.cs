using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetString : Message<string>
    {
        public NetString(string data, Flags flags) : base(data, flags)
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
            outData.AddRange(BitConverter.GetBytes(data.Length * 2));
            outData.AddRange(System.Text.Encoding.Unicode.GetBytes(data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override string Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            int dataSize = BitConverter.ToInt32(message, counter);
            counter += sizeof(int);
            
            return System.Text.Encoding.Unicode.GetString(message, counter, dataSize);
        }
    }
}