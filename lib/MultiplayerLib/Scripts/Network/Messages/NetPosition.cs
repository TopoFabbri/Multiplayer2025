using System;
using System.Collections.Generic;
using Multiplayer.CustomMath;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct Position
    {
        public Vector3 position;
        public int objId;

        public Position(float x, float y, float z, int objId)
        {
            position = new Vector3(x, y, z);
            this.objId = objId;
        }

        public Position(Vector3 position, int objId)
        {
            this.position = position;
            this.objId = objId;
        }
    }

    public class NetPosition : Message<Position>
    {
        private static int _posIds;
        
        public NetPosition(Position position) : base(position)
        {
            metadata.Type = MessageType.Position;
            metadata.Flags = Flags.Sortable | Flags.Checksum;
            metadata.MsgId = _posIds++;
        }

        public NetPosition(byte[] data) : base(data)
        {
        }

        protected override Position Deserialize(byte[] message)
        {
            Position outPosition;
            
            outPosition.position.x = BitConverter.ToSingle(message, MessageMetadata.Size);
            outPosition.position.y = BitConverter.ToSingle(message, MessageMetadata.Size + sizeof(float));
            outPosition.position.z = BitConverter.ToSingle(message, MessageMetadata.Size + sizeof(float) * 2);
            outPosition.objId = BitConverter.ToInt32(message, MessageMetadata.Size + sizeof(float) * 3);

            return outPosition;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data.position.x));
            outData.AddRange(BitConverter.GetBytes(data.position.y));
            outData.AddRange(BitConverter.GetBytes(data.position.z));
            outData.AddRange(BitConverter.GetBytes(data.objId));
            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }
    }
}