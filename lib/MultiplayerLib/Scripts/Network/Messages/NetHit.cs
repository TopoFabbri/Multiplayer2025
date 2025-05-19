using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct Hit
    {
        public readonly int hitObjId;
        public readonly int bulletObjId;
        public readonly int damage;

        public Hit(int hitObjId, int bulletObjId, int damage)
        {
            this.hitObjId = hitObjId;
            this.bulletObjId = bulletObjId;
            this.damage = damage;
        }
    }

    public class NetHit : Message<Hit>
    {
        public NetHit(Hit data) : base(data)
        {
            metadata.Type = MessageType.Hit;
            metadata.Flags = Flags.Important | Flags.Checksum;
        }

        public NetHit(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data.hitObjId));
            outData.AddRange(BitConverter.GetBytes(data.bulletObjId));
            outData.AddRange(BitConverter.GetBytes(data.damage));
            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }

        protected override Hit Deserialize(byte[] message)
        {
            return new Hit(BitConverter.ToInt32(message, MessageMetadata.Size), BitConverter.ToInt32(message, MessageMetadata.Size + sizeof(int)),
                BitConverter.ToInt32(message, MessageMetadata.Size + sizeof(int) * 2));
        }
    }
}