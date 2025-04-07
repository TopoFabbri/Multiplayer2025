using System;
using System.Collections.Generic;

namespace Network.Messages
{
    public struct Spawnable
    {
        public int spawnableNumber;
        public int id;
    }

    public class NetSpawnable : Message<Spawnable>
    {
        public NetSpawnable(Spawnable vector) : base(vector)
        {
        }

        public NetSpawnable(byte[] data) : base(data)
        {
        }

        public override MessageType GetMessageType()
        {
            return MessageType.Spawnable;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(BitConverter.GetBytes(data.spawnableNumber));
            outData.AddRange(BitConverter.GetBytes(data.id));
            
            return outData.ToArray();
        }

        protected override Spawnable Deserialize(byte[] message)
        {
            const int messageTypeSize = sizeof(MessageType);

            return new Spawnable()
            {
                spawnableNumber = BitConverter.ToInt32(message, messageTypeSize),
                id = BitConverter.ToInt32(message, messageTypeSize + sizeof(int))
            };
        }
    }
}