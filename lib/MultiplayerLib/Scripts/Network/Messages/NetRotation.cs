using System;
using System.Collections.Generic;
using System.Numerics;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct Rotation
    {
        public Vector2 rotation;
        public int objId;

        public Rotation(float x, float y, int objId)
        {
            rotation = new Vector2(x, y);
            this.objId = objId;
        }

        public Rotation(Vector2 rotation, int objId)
        {
            this.rotation = rotation;
            this.objId = objId;
        }
    }

    public class NetRotation : Message<Rotation>
    {
        private static int _rotIds;

        public NetRotation(Rotation data) : base(data)
        {
            metadata.Type = MessageType.Rotation;
            metadata.Flags = Flags.Sortable | Flags.Checksum;
            metadata.MsgId = _rotIds++;
        }

        public NetRotation(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(Metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(Data.rotation.X));
            outData.AddRange(BitConverter.GetBytes(Data.rotation.Y));
            outData.AddRange(BitConverter.GetBytes(Data.objId));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override Rotation Deserialize(byte[] message)
        {
            Rotation outRot;

            outRot.rotation.X = BitConverter.ToSingle(message, MessageMetadata.Size);
            outRot.rotation.Y = BitConverter.ToSingle(message, MessageMetadata.Size + sizeof(float));
            outRot.objId = BitConverter.ToInt32(message, MessageMetadata.Size + sizeof(float) * 2);

            return outRot;
        }
    }
}