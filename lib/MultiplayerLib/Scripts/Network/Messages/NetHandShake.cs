using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public struct HandShake
    {
        public readonly Dictionary<int, string> clientNames;
        public readonly uint randomSeed;
        public readonly bool fromServer;
        public readonly int level;
        public readonly string name;

        public HandShake(uint randomSeed, Dictionary<int, string> clientNames, bool fromServer, int level, string name)
        {
            this.fromServer = fromServer;
            this.randomSeed = randomSeed;
            this.clientNames = clientNames;
            this.level = level;
            this.name = name;
        }
    }

    public class NetHandShake : Message<HandShake>
    {
        public NetHandShake(HandShake data, bool important) : base(data)
        {
            metadata.Type = MessageType.HandShake;

            if (important)
                metadata.Flags = Flags.Important;
        }

        public NetHandShake(byte[] data) : base(data)
        {
        }

        protected override HandShake Deserialize(byte[] message)
        {
            int counter = MessageMetadata.Size;

            bool fromServer = BitConverter.ToBoolean(message, counter);
            counter += sizeof(bool);

            uint randomSeed = BitConverter.ToUInt32(message, counter);
            counter += sizeof(uint);

            int level = BitConverter.ToInt32(message, counter);
            counter += sizeof(int);

            int nameLength = BitConverter.ToInt32(message, counter);
            counter += sizeof(int);

            string name = System.Text.Encoding.UTF8.GetString(message, counter, nameLength);
            counter += nameLength;

            int nameCount = BitConverter.ToInt32(message, counter);
            counter += sizeof(int);

            Dictionary<int, string> names = new();

            for (int i = 0; i < nameCount; i++)
            {
                int clientId = BitConverter.ToInt32(message, counter);
                counter += sizeof(int);

                int clientNameLength = BitConverter.ToInt32(message, counter);
                counter += sizeof(int);

                string clientName = System.Text.Encoding.UTF8.GetString(message, counter, clientNameLength);
                counter += clientNameLength;

                names.Add(clientId, clientName);
            }

            return new HandShake(randomSeed, names, fromServer, level, name);
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            outData.AddRange(metadata.Serialize());

            outData.AddRange(BitConverter.GetBytes(data.fromServer));
            outData.AddRange(BitConverter.GetBytes(data.randomSeed));
            outData.AddRange(BitConverter.GetBytes(data.level));
            outData.AddRange(BitConverter.GetBytes(data.name.Length));
            outData.AddRange(System.Text.Encoding.UTF8.GetBytes(data.name));
            
            outData.AddRange(BitConverter.GetBytes(data.clientNames.Count));

            foreach (KeyValuePair<int, string> client in data.clientNames)
            {
                outData.AddRange(BitConverter.GetBytes(client.Key));
                outData.AddRange(BitConverter.GetBytes(client.Value.Length));
                outData.AddRange(System.Text.Encoding.UTF8.GetBytes(client.Value));
            }

            return outData.ToArray();
        }
    }
}