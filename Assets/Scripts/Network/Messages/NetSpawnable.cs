using System;
using System.Collections.Generic;

namespace Network.Messages
{
    public struct Spawnable
    {
        public int spawnableNumber;
        public int id;
    }

    public class NetSpawnable : Message<List<Spawnable>>
    {
        public NetSpawnable(List<Spawnable> data) : base(data)
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

            foreach (Spawnable spawnable in data)
            {
                outData.AddRange(BitConverter.GetBytes(spawnable.spawnableNumber));
                outData.AddRange(BitConverter.GetBytes(spawnable.id));
            }


            return outData.ToArray();
        }

        protected override List<Spawnable> Deserialize(byte[] message)
        {
            const int messageTypeSize = sizeof(MessageType);

            List<Spawnable> outData = new();

            for (int i = messageTypeSize; i < message.Length; i += sizeof(int) * 2)
            {
                Spawnable spawnable = new()
                {
                    spawnableNumber = BitConverter.ToInt32(message, i),
                    id = BitConverter.ToInt32(message, i + sizeof(int))
                };
                
                outData.Add(spawnable);
            }
            
            return outData;
        }
    }
}