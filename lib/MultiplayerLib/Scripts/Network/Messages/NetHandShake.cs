using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public class Color
    {
        public readonly float r;
        public readonly float g;
        public readonly float b;
        public readonly float a;

        public static int Size => sizeof(float) * 4;

        public byte[] Serialized
        {
            get
            {
                List<byte> outData = new();

                outData.AddRange(BitConverter.GetBytes(r));
                outData.AddRange(BitConverter.GetBytes(g));
                outData.AddRange(BitConverter.GetBytes(b));
                outData.AddRange(BitConverter.GetBytes(a));

                return outData.ToArray();
            }
        }

        public static Color Deserialize(byte[] data, int startIndex)
        {
            return new Color(BitConverter.ToSingle(data, startIndex), BitConverter.ToSingle(data, startIndex + sizeof(float)),
                BitConverter.ToSingle(data, startIndex + sizeof(float) * 2), BitConverter.ToSingle(data, startIndex + sizeof(float) * 3));
        }

        public Color()
        {
            r = 0;
            g = 0;
            b = 0;
            a = 0;
        }

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public struct HandShake
    {
        public readonly Dictionary<int, Color> clientColorsById;
        public readonly Dictionary<int, string> clientNames;
        public readonly uint randomSeed;
        public readonly bool fromServer;
        public readonly int level;
        public readonly string name;

        public HandShake(uint randomSeed, Dictionary<int, Color> clientColorsById, Dictionary<int, string> clientNames, bool fromServer, int level, string name)
        {
            this.fromServer = fromServer;
            this.randomSeed = randomSeed;
            this.clientColorsById = clientColorsById;
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

            int clientCount = BitConverter.ToInt32(message, counter);
            counter += sizeof(int);

            Dictionary<int, Color> clients = new();

            for (int i = 0; i < clientCount; i++)
            {
                int clientId = BitConverter.ToInt32(message, counter);
                counter += sizeof(int);

                Color color = Color.Deserialize(message, counter);
                counter += Color.Size;

                clients.Add(clientId, color);
            }

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

            return new HandShake(randomSeed, clients, names, fromServer, level, name);
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
            outData.AddRange(BitConverter.GetBytes(data.clientColorsById.Count));

            foreach (KeyValuePair<int, Color> client in data.clientColorsById)
            {
                outData.AddRange(BitConverter.GetBytes(client.Key));
                outData.AddRange(client.Value.Serialized);
            }
            
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