using System.Collections.Generic;
using System.Text;

namespace Network.Messages
{
    public class NetConsole : Message<string>
    {
        public NetConsole(string data) : base(data)
        {
            metadata.Type = MessageType.Console;
        }

        public NetConsole(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(metadata.Serialize());
            outData.AddRange(Encoding.UTF8.GetBytes(data));

            return outData.ToArray();
        }

        protected override string Deserialize(byte[] message)
        {
            int dataSize = message.Length - MessageMetadata.Size;

            return Encoding.UTF8.GetString(message, MessageMetadata.Size, dataSize);
        }
    }
}