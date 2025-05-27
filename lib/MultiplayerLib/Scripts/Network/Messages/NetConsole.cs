using System;
using System.Collections.Generic;
using System.Text;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public class NetConsole : Message<string>
    {
        private static int _messageIds;
        
        public NetConsole(string data) : base(data)
        {
            Metadata.Crypted = true;
            Metadata.Type = MessageType.Console;
            Metadata.Flags = Flags.Checksum | Flags.Important | Flags.Sortable;
            Metadata.MsgId = _messageIds++;
        }

        public NetConsole(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(metadata.Serialize());
            outData.AddRange(BitConverter.GetBytes(data.Length));
            outData.AddRange(Encoding.UTF8.GetBytes(data));
            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }

        protected override string Deserialize(byte[] message)
        {
            int dataSize = BitConverter.ToInt32(message, MessageMetadata.Size);

            return Encoding.UTF8.GetString(message, MessageMetadata.Size, dataSize);
        }
    }
}