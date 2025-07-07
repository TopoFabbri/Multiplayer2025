using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;
using Multiplayer.NetworkFactory;

namespace Multiplayer.Network.Messages
{
    public struct SpawnRequest
    {
        public List<SpawnableObjectData> spawnableObjects;
        
        public SpawnRequest(List<SpawnableObjectData> spawnableObjects)
        {
            this.spawnableObjects = spawnableObjects;
        }
    }

    public class NetSpawnable : Message<SpawnRequest>
    {
        private static int _ids;
        
        public NetSpawnable(SpawnRequest data) : base(data)
        {
            metadata.Type = MessageType.SpawnRequest;
            metadata.Flags = Flags.Important | Flags.Checksum | Flags.Sortable;
            metadata.MsgId = _ids++;
        }

        public NetSpawnable(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data.spawnableObjects.Count));
            
            foreach (SpawnableObjectData spawnableObj in data.spawnableObjects)
                outData.AddRange(spawnableObj.Serialized);

            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }

        protected override SpawnRequest Deserialize(byte[] message)
        {
            SpawnRequest outData = new() ;

            int startIndex = MessageMetadata.Size;
            
            int count = BitConverter.ToInt32(message, startIndex);
            startIndex += sizeof(int);
            
            outData.spawnableObjects = new List<SpawnableObjectData>();
            
            for (int i = 0; i < count; i++)
                outData.spawnableObjects.Add(SpawnableObjectData.Deserialize(message, ref startIndex));
            
            return outData;
        }
    }
}