using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct SpawnRequest
    {
        public int requesterId;
        private Dictionary<int, int> spawnablesById;

        public Dictionary<int, int> SpawnablesById
        {
            get { return spawnablesById ??= new Dictionary<int, int>(); }
            private set => spawnablesById = value;
        }

        public SpawnRequest(int requesterId, Dictionary<int, int> spawnablesById)
        {
            this.requesterId = requesterId;
            this.spawnablesById = spawnablesById;
        }
    }

    public class NetSpawnable : Message<SpawnRequest>
    {
        public NetSpawnable(SpawnRequest data) : base(data)
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

            outData.AddRange(BitConverter.GetBytes(data.requesterId));

            foreach (KeyValuePair<int, int> spawnableById in data.SpawnablesById)
            {
                outData.AddRange(BitConverter.GetBytes(spawnableById.Key));
                outData.AddRange(BitConverter.GetBytes(spawnableById.Value));
            }

            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }

        protected override SpawnRequest Deserialize(byte[] message)
        {
            SpawnRequest outData = new() { requesterId = BitConverter.ToInt32(message, MessageMetadata.Size) };

            for (int i = MessageMetadata.Size + sizeof(int); i < message.Length - 2 * sizeof(uint); i += sizeof(int) * 2)
            {
                outData.SpawnablesById.Add(BitConverter.ToInt32(message, i), BitConverter.ToInt32(message, i + sizeof(int)));
            }

            return outData;
        }
    }
}