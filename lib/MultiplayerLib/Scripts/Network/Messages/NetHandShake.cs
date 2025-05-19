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
        public readonly Dictionary<int, Color> clients;
        public readonly uint randomSeed;
        public readonly bool fromServer;
        public readonly int level;

        public HandShake(uint randomSeed, Dictionary<int, Color> clients, bool fromServer, int level)
        {
            this.fromServer = fromServer;
            this.randomSeed = randomSeed;
            this.clients = clients;
            this.level = level;
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
            
            Dictionary<int, Color> clients = new();

            for (int i = counter; i < message.Length; i += 4 + Color.Size)
            {
                byte[] curInt = { message[i], message[i + 1], message[i + 2], message[i + 3] };
                Color color = Color.Deserialize(message, i + 4);

                clients.Add(BitConverter.ToInt32(curInt, 0), color);
            }

            return new HandShake(randomSeed, clients, fromServer, level);
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();
            outData.AddRange(metadata.Serialize());

            outData.AddRange(BitConverter.GetBytes(data.fromServer));
            outData.AddRange(BitConverter.GetBytes(data.randomSeed));
            outData.AddRange(BitConverter.GetBytes(data.level));

            foreach (KeyValuePair<int, Color> client in data.clients)
            {
                outData.AddRange(BitConverter.GetBytes(client.Key));
                outData.AddRange(client.Value.Serialized);
            }

            return outData.ToArray();
        }
    }
}