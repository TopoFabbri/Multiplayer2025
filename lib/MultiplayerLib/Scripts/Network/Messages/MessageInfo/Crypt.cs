using System;
using System.Collections.Generic;

namespace Multiplayer.Network.Messages.MessageInfo
{
    public static class Crypt
    {
        public const int OperationsCount = 4;

        public static readonly List<BitOperations.Operations> Operations = new();
        
        public static byte[] Encrypt(byte[] data)
        {
            if (data.Length <= 1)
                return data;
        
            byte[] result = new byte[data.Length];
            Array.Copy(data, result, data.Length);
        
            for (int i = 1; i < data.Length; i++)
            {
                uint operated = data[i];
                foreach (BitOperations.Operations operation in Operations)
                {
                    operated = BitOperations.Operate(operated, operation, data[i]);
                }
                result[i] = (byte)operated;
            }
            
            return result;
        }

        public static byte[] Decrypt(byte[] data)
        {
            if (data.Length <= 1)
                return data;
            
            byte[] result = new byte[data.Length];
            Array.Copy(data, result, data.Length);
            
            for (int i = 1; i < data.Length; i++)
            {
                uint operated = data[i];
                for (int j = Operations.Count - 1; j >= 0; j--)
                {
                    operated = BitOperations.Operate(operated, Operations[j], data[i]);
                }
                result[i] = (byte)operated;
            }
            
            return result;
        }

        public static bool IsCrypted(byte[] data)
        {
            return BitConverter.ToBoolean(data, 0);
        }
        
        public static void GenerateOperations(uint seed)
        {
            Random random = new((int)seed);
            
            for (int i = 0; i < OperationsCount; i++)
            {
                Operations.Add((BitOperations.Operations)random.Next(0, (int)BitOperations.Operations.Count));
            }
        }
    }
}