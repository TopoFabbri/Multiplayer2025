using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct SpawnRequest
    {
        public int spawnableNumber;
        public int id;
    }

    public class NetSpawnable : Message<List<SpawnRequest>>
    {
        public NetSpawnable(List<SpawnRequest> data) : base(data)
        {
            metadata.Type = MessageType.SpawnRequest;
            metadata.Flags = Flags.Important;
        }

        public NetSpawnable(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(metadata.Serialize());

            foreach (SpawnRequest spawnable in data)
            {
                outData.AddRange(BitConverter.GetBytes(spawnable.spawnableNumber));
                outData.AddRange(BitConverter.GetBytes(spawnable.id));
            }
            
            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }

        protected override List<SpawnRequest> Deserialize(byte[] message)
        {
            List<SpawnRequest> outData = new();

            for (int i = MessageMetadata.Size; i < message.Length - 2 * sizeof(uint); i += sizeof(int) * 2)
            {
                SpawnRequest spawnRequest = new()
                {
                    spawnableNumber = BitConverter.ToInt32(message, i),
                    id = BitConverter.ToInt32(message, i + sizeof(int))
                };
                
                outData.Add(spawnRequest);
            }
            
            return outData;
        }
    }
}