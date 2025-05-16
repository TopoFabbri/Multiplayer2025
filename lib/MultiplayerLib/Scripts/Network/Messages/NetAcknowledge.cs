using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct Acknowledge
    {
        internal int mesId;
        internal int senderId;
        internal MessageType mesType;
    }
    
    internal class NetAcknowledge : Message<Acknowledge>
    {
        public NetAcknowledge(Acknowledge data) : base(data)
        {
            Metadata.Type = MessageType.Acknowledge;
            Metadata.Flags = Flags.Checksum;
        }

        public NetAcknowledge(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data.mesId));
            outData.AddRange(BitConverter.GetBytes(data.senderId));
            outData.AddRange(BitConverter.GetBytes((int)data.mesType));
            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }

        protected override Acknowledge Deserialize(byte[] message)
        {
            Acknowledge outAcknowledge;
            
            outAcknowledge.mesId = BitConverter.ToInt32(message, MessageMetadata.Size);
            outAcknowledge.senderId = BitConverter.ToInt32(message, MessageMetadata.Size + sizeof(int));
            outAcknowledge.mesType = (MessageType)BitConverter.ToInt32(message, MessageMetadata.Size + sizeof(int) * 2);

            return outAcknowledge;
        }
    }
}