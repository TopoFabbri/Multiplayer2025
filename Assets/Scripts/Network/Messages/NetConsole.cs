using System;
using System.Collections.Generic;

namespace Network
{
    public class NetConsole : Message<string>
    {
        public NetConsole(string data) : base(data)
        {
        }

        public NetConsole(byte[] data) : base(data)
        {
        }

        public override MessageType GetMessageType() => MessageType.Console;

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
            outData.AddRange(System.Text.Encoding.UTF8.GetBytes(data));

            return outData.ToArray();
        }

        protected override string Deserialize(byte[] message)
        {
            const int messageTypeSize = sizeof(MessageType);
            int dataSize = message.Length - messageTypeSize;

            return System.Text.Encoding.UTF8.GetString(message, messageTypeSize, dataSize);
        }
    }
}