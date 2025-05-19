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

        public static uint Operate(uint operated, Operations operation, byte data)
        {
            if (!_initiated)
                Initiate();

            return Operators[operation].Invoke(operated, data);
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
        
        private static uint LeftShift(uint operated, byte operand)
        {
            return operated << operand;
        }

        private static uint RightShift(uint operated, byte operand)
        {
            return operated >> operand;
        }

        private static uint Or(uint operated, byte operand)
        {
            return operated | operand;
        }

        private static uint And(uint operated, byte operand)
        {
            return operated & operand;
        }

        private static uint Xor(uint operated, byte operand)
        {
            return operated ^ operand;
        }
    }
}