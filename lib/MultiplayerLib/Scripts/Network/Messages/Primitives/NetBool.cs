using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages.Primitives
{
    public class NetBool : Message<bool>
    {
        public NetBool(bool data, Flags flags) : base(data, flags)
        {
            metadata.Type = MessageType.Bool;
        }

        public NetBool(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.Add(Convert.ToByte(data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override bool Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            return Convert.ToBoolean(message[counter]);
        }
    }
}