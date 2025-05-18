using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public class NetCrouch : Message<int>
    {
        public NetCrouch(int objId) : base(objId)
        {
            metadata.Type = MessageType.Crouch;
            metadata.Flags = Flags.Checksum | Flags.Important;
        }

        public NetCrouch(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data));
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override int Deserialize(byte[] message)
        {
            return BitConverter.ToInt32(message, MessageMetadata.Size);
        }
    }
}