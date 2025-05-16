using System;
using System.Collections.Generic;

namespace Multiplayer.Network.Messages.MessageInfo
{
    public static class BitOperations
    {
        public enum Operations
        {
            LeftShift,
            RightShift,
            Or,
            And,
            Xor,
            Count
        }

        private static bool _initiated;

        private static readonly Dictionary<Operations, Func<uint, byte, uint>> Operators = new();

        public static uint Operate(uint checksum, Operations operation, byte data)
        {
            if (!_initiated)
                Initiate();

            return Operators[operation].Invoke(checksum, data);
        }

        private static void Initiate()
        {
            Operators.Add(Operations.LeftShift, LeftShift);
            Operators.Add(Operations.RightShift, RightShift);
            Operators.Add(Operations.Or, Or);
            Operators.Add(Operations.And, And);
            Operators.Add(Operations.Xor, Xor);
            
            _initiated = true;
        }
        
        private static uint LeftShift(uint checksum, byte operand)
        {
            return checksum << operand;
        }

        private static uint RightShift(uint checksum, byte operand)
        {
            return checksum >> operand;
        }

        private static uint Or(uint checksum, byte operand)
        {
            return checksum | operand;
        }

        private static uint And(uint checksum, byte operand)
        {
            return checksum & operand;
        }

        private static uint Xor(uint checksum, byte operand)
        {
            return checksum ^ operand;
        }
    }
}