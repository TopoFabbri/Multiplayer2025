using System;
using System.Collections.Generic;
using Multiplayer.Network.Objects;
using Multiplayer.Reflection;

namespace Multiplayer.AuthoritativeServer
{
    public class PlayerInput : ObjectM
    {
        [field: Sync] public int CursorX { get; set; }
        [field: Sync] public int CursorY { get; set; }
        [field: Sync] public bool Clicked { get; set; }
        
        public byte[] Serialize()
        {
            List<byte> data = new();
            
            data.AddRange(BitConverter.GetBytes(ObjectId));
            data.AddRange(BitConverter.GetBytes(Owner));
            data.AddRange(BitConverter.GetBytes(CursorX));
            data.AddRange(BitConverter.GetBytes(CursorY));
            data.AddRange(BitConverter.GetBytes(Clicked));
            
            return data.ToArray();
        }
        
        public static PlayerInput Deserialize(byte[] data, ref int counter)
        {
            PlayerInput input = new() { objectId = BitConverter.ToInt32(data, counter) };

            counter += sizeof(int);
            
            input.Owner = BitConverter.ToInt32(data, counter);
            counter += sizeof(int);
            
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