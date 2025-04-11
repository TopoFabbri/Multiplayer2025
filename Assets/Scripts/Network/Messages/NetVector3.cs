using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Messages
{
    public struct Position
    {
        public Vector3 position;
        public int objId;
        public int originId;

        public Position(float x, float y, float z, int objId, int originId)
        {
            position = new Vector3(x, y, z);
            this.objId = objId;
            this.originId = originId;
        }

        public Position(Vector3 position, int objId, int originId)
        {
            this.position = position;
            this.objId = objId;
            this.originId = originId;
        }
    }

    public class NetVector3 : Message<Position>
    {
        public NetVector3(Position position) : base(position)
        {
        }

        public NetVector3(byte[] data) : base(data)
        {
        }

        protected override Position Deserialize(byte[] message)
        {
            Position outPosition;

            const int messageTypeSize = sizeof(MessageType);

            outPosition.position.x = BitConverter.ToSingle(message, messageTypeSize);
            outPosition.position.y = BitConverter.ToSingle(message, messageTypeSize + sizeof(float));
            outPosition.position.z = BitConverter.ToSingle(message, messageTypeSize + sizeof(float) * 2);
            outPosition.objId = BitConverter.ToInt32(message, messageTypeSize + sizeof(float) * 3);
            outPosition.originId = BitConverter.ToInt32(message, messageTypeSize + sizeof(float) * 3 + sizeof(int));

            return outPosition;
        }

        public override MessageType GetMessageType()
        {
            return MessageType.Position;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(BitConverter.GetBytes(data.position.x));
            outData.AddRange(BitConverter.GetBytes(data.position.y));
            outData.AddRange(BitConverter.GetBytes(data.position.z));
            outData.AddRange(BitConverter.GetBytes(data.objId));
            outData.AddRange(BitConverter.GetBytes(data.originId));

            return outData.ToArray();
        }
    }
}