using System;
using System.Collections.Generic;

namespace Multiplayer.Network.Messages.MessageInfo
{
    [Flags]
    public enum Flags
    {
        None = 0,
        Sortable = 1,
        Important = 2,
        Critical = 4,
        Checksum = 8,
    }

    public class MessageMetadata
    {
        public bool Crypted { get; set; }
        public MessageType Type { get; set; }
        public Flags Flags { get; set; }
        public int MsgId { get; set; }
        public int SenderId { get; set; }

        public byte[] Serialize()
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes(Crypted));
            outData.AddRange(BitConverter.GetBytes((int)Type));
            outData.AddRange(BitConverter.GetBytes((int)Flags));
            outData.AddRange(BitConverter.GetBytes(MsgId));
            outData.AddRange(BitConverter.GetBytes(SenderId));

            return outData.ToArray();
        }

        public static MessageMetadata Deserialize(byte[] message)
        {
            MessageMetadata outData = new();
            int counter = 0;

            outData.Crypted = BitConverter.ToBoolean(message, counter);
            counter += sizeof(bool);

            outData.Type = (MessageType)BitConverter.ToInt32(message, counter);

            counter += sizeof(MessageType);
            outData.Flags = (Flags)BitConverter.ToInt32(message, counter);

            counter += sizeof(Flags);
            outData.MsgId = BitConverter.ToInt32(message, counter);

            counter += sizeof(int);
            outData.SenderId = BitConverter.ToInt32(message, counter);

            return outData;
        }

        public static int Size
        {
            get
            {
                int size = sizeof(bool);
                size += sizeof(MessageType);
                size += sizeof(Flags);
                size += sizeof(int);
                size += sizeof(int);

                return size;
            }
        }

        public bool IsPrimitive =>
            Type is MessageType.Bool or MessageType.Byte or MessageType.Char or MessageType.Double or MessageType.Float or MessageType.Int or MessageType.Long or MessageType.Short
                or MessageType.String or MessageType.UInt or MessageType.ULong or MessageType.UShort;
    }
}