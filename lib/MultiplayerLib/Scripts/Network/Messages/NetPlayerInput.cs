using System.Collections.Generic;
using Multiplayer.AuthoritativeServer;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public class NetPlayerInput : Message<PlayerInput>
    {
        private static int _messageIds;
        
        public NetPlayerInput(PlayerInput data) : base(data)
        {
            Metadata.Type = MessageType.PlayerInput;
            Metadata.Flags = Flags.Checksum | Flags.Sortable;
            Metadata.MsgId = _messageIds++;
        }

        public NetPlayerInput(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            
            outData.AddRange(metadata.Serialize());
            outData.AddRange(Data.Serialize());
            outData.AddRange(GetCheckSum(outData));
            
            return outData.ToArray();
        }

        protected override PlayerInput Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;
            
            PlayerInput input = PlayerInput.Deserialize(message, ref counter);
            
            return input;
        }
    }
}