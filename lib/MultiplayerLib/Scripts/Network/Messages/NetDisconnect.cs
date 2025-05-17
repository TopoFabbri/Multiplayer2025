using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public class NetDisconnect : Message<int>
    {
        public NetDisconnect(int data) : base(data)
        {
            metadata.Type = MessageType.Disconnect;
            metadata.Flags = Flags.Important;
        }
        
        public NetDisconnect(byte[] data) : base(data)
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