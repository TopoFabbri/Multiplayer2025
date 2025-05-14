using System;
using System.Collections.Generic;

namespace Network.Messages
{
    public class NetHandShake : Message<List<int>>
    {
        public NetHandShake(List<int> data) : base(data)
        {
            metadata.Type = MessageType.HandShake;
        }

        public NetHandShake(byte[] data) : base(data)
        {
        }

        protected override List<int> Deserialize(byte[] message)
        {
            List<int> outData = new();

            for (int i = MessageMetadata.Size; i < message.Length; i += 4)
            {
                byte[] curInt = { message[i], message[i + 1], message[i + 2], message[i + 3] };
                outData.Add(BitConverter.ToInt32(curInt, 0));
            }

            return outData;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            outData.AddRange(metadata.Serialize());

            foreach (int i in data)
                outData.AddRange(BitConverter.GetBytes(i));

            return outData.ToArray();
        }
    }
}