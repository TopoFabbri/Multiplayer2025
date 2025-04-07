using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class NetVector3 : Message<Vector3>
    {
        private static ulong _lastMsgID;

        public NetVector3(Vector3 data) : base(data)
        {
        }
        
        public NetVector3(byte[] data) : base(data){}

        protected override Vector3 Deserialize(byte[] message)
        {
            Vector3 outData;

            outData.x = BitConverter.ToSingle(message, 12);
            outData.y = BitConverter.ToSingle(message, 16);
            outData.z = BitConverter.ToSingle(message, 20);

            return outData;
        }

        public override MessageType GetMessageType()
        {
            return MessageType.Position;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(BitConverter.GetBytes(_lastMsgID++));
            outData.AddRange(BitConverter.GetBytes(data.x));
            outData.AddRange(BitConverter.GetBytes(data.y));
            outData.AddRange(BitConverter.GetBytes(data.z));

            return outData.ToArray();
        }
    }
}