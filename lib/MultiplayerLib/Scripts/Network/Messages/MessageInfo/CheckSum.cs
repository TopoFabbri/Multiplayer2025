using System;
using System.Collections.Generic;

namespace Multiplayer.Network.Messages.MessageInfo
{
    public static class CheckSum
    {
        private const int Length = 4;
        private static readonly List<BitOperations.Operations> Operations1 = new();
        private static readonly List<BitOperations.Operations> Operations2 = new();
        public static uint RandomSeed {get; set;}

        public static uint Get(byte[] data, bool first)
        {
            uint sum = RandomSeed;

            foreach (byte b in data)
            {
                List<BitOperations.Operations> ops = first ? Operations1 : Operations2;

                foreach (BitOperations.Operations operation in ops)
                {
                    sum += BitOperations.Operate(sum, operation, b);
                }
            }

            return sum;
        }

        public static void CreateOperationsArrays(int seed)
        {
            Random random = new(seed);

            Operations1.Clear();
            Operations2.Clear();
            
            for (int i = 0; i < Length; i++)
            {
                Operations1.Add((BitOperations.Operations)random.Next(0, (int)BitOperations.Operations.Count));
                Operations2.Add((BitOperations.Operations)random.Next(0, (int)BitOperations.Operations.Count));
            }
        }

        public static bool IsValid(byte[] data)
        {
            List<byte> message = new();
            
            uint checksum1 = BitConverter.ToUInt32(data, data.Length - sizeof(uint) * 2);
            uint checksum2 = BitConverter.ToUInt32(data, data.Length - sizeof(uint));

            for (int i = 0; i < data.Length - sizeof(uint) * 2; i++)
            {
                byte b = data[i];
                message.Add(b);
            }
            
            if (Get(message.ToArray(), true) != checksum1)
                return false;
            
            message.AddRange(BitConverter.GetBytes(checksum1));
            
            return Get(message.ToArray(), false) == checksum2;
        }
    }
}