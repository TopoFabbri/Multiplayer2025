using System;
using System.Collections.Generic;

namespace Network
{
    public class NetHandShake : Message<List<int>>
    {
        public NetHandShake(List<int> data) : base(data)
        {
        }

        public NetHandShake(byte[] data) : base(data)
        {
        }

        protected override List<int> Deserialize(byte[] message)
        {
            List<int> outData = new();

            for (int i = sizeof(MessageType); i < message.Length; i += 4)
            {
                byte[] curInt = { message[i], message[i + 1], message[i + 2], message[i + 3] };
                outData.Add(BitConverter.ToInt32(curInt, 0));
            }

            return outData;
        }

        public override MessageType GetMessageType()
        {
            return MessageType.HandShake;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

            foreach (int i in data)
                outData.AddRange(BitConverter.GetBytes(i));

            return outData.ToArray();
        }
    }
}