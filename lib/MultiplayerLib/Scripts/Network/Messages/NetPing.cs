using System;
using System.Collections.Generic;
using Multiplayer.Network.Messages.MessageInfo;

namespace Multiplayer.Network.Messages
{
    public class PingWrapper
    {
        public Dictionary<int, float> PingsByClientId { get; } = new();

        public byte[] Serialized
        {
            get
            {
                List<byte> outData = new();

                outData.AddRange(BitConverter.GetBytes(PingsByClientId.Count));

                foreach (KeyValuePair<int, float> pingByClient in PingsByClientId)
                {
                    outData.AddRange(BitConverter.GetBytes(pingByClient.Key));
                    outData.AddRange(BitConverter.GetBytes(pingByClient.Value));
                }

                return outData.ToArray();
            }
        }

        public static Dictionary<int, float> Deserialize(byte[] data, int startIndex)
        {
            int count = BitConverter.ToInt32(data, startIndex);
            Dictionary<int, float> outPingsByClientId = new();

            startIndex += sizeof(int);

            for (int i = 0; i < count; i++)
            {
                int clientId = BitConverter.ToInt32(data, startIndex);
                startIndex += sizeof(int);

                float ping = BitConverter.ToSingle(data, startIndex);
                startIndex += sizeof(float);

                outPingsByClientId.Add(clientId, ping);
            }

            return outPingsByClientId;
        }

        public PingWrapper()
        {
        }

        public PingWrapper(Dictionary<int, float> pingsByClientId)
        {
            this.PingsByClientId = pingsByClientId;
        }
    }

    public class NetPing : Message<PingWrapper>
    {
        private static int _ids;

        public NetPing(PingWrapper data) : base(data)
        {
            metadata.Type = MessageType.Ping;
            metadata.Flags = Flags.Checksum | Flags.Sortable | Flags.Important;
            metadata.MsgId = _ids++;
        }

        public NetPing(byte[] data) : base(data)
        {
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(metadata.Serialize());
            outData.AddRange(data.Serialized);
            outData.AddRange(GetCheckSum(outData));

            return outData.ToArray();
        }

        protected override PingWrapper Deserialize(byte[] message)
        {
            return new PingWrapper(PingWrapper.Deserialize(message, MessageMetadata.Size));
        }
    }
}