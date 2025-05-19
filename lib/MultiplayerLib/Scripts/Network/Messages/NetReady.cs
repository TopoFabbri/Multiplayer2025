using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public class NetReady : Message<byte>
    {
        public NetReady(byte data) : base(data)
        {
            metadata.Type = MessageType.Ready;
            metadata.Flags = Flags.Checksum | Flags.Important;
        }

        public NetReady(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(Metadata.Serialize());
            outData.Add(Data);
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override byte Deserialize(byte[] message)
        {
            return message[0];
        }
    }
}