using System;
using System.Collections.Generic;

namespace Multiplayer.AuthoritativeServer
{
    public class PlayerInput
    {
        public int Id { get; set; }
        public int CursorX { get; set; }
        public int CursorY { get; set; }
        public bool Clicked { get; set; }
        
        public byte[] Serialize()
        {
            List<byte> data = new();
            
            data.AddRange(BitConverter.GetBytes(CursorX));
            data.AddRange(BitConverter.GetBytes(CursorY));
            data.AddRange(BitConverter.GetBytes(Clicked));
            
            return data.ToArray();
        }
        
        public static PlayerInput Deserialize(byte[] data, ref int counter)
        {
            PlayerInput input = new();
            
            input.CursorX = BitConverter.ToInt32(data, counter);
            counter += sizeof(int);
            
            input.CursorY = BitConverter.ToInt32(data, counter);
            counter += sizeof(int);
            
            input.Clicked = BitConverter.ToBoolean(data, counter);
            counter += sizeof(bool);
            
            return input;
        }
        
        public void Set(byte[] data, ref int counter)
        {
            PlayerInput playerInput = Deserialize(data, ref counter);
            
            CursorX = playerInput.CursorX;
            CursorY = playerInput.CursorY;
            Clicked = playerInput.Clicked;
        }
    }
}