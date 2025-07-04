using System;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = Multiplayer.CustomMath.Vector3;

namespace Multiplayer.NetworkFactory
{
    public class SpawnableObjectData
    {
        public int Id { get; set; }
        
        public int OwnerId { get; set; }
        
        public int PrefabId { get; set; }

        public byte[] Serialized => Serialize(this);
        
        public static int Size => sizeof(int) * 3 + sizeof(float) * 5;

        public static byte[] Serialize(SpawnableObjectData data)
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes(data.Id));
            
            outData.AddRange(BitConverter.GetBytes(data.OwnerId));
            
            outData.AddRange(BitConverter.GetBytes(data.PrefabId));
                
            return outData.ToArray();
        }

        public static SpawnableObjectData Deserialize(byte[] data, ref int startIndex)
        {
            SpawnableObjectData objData = new();
            
            objData.Id = BitConverter.ToInt32(data, startIndex);
            startIndex += sizeof(int);
            
            objData.OwnerId = BitConverter.ToInt32(data, startIndex);
            startIndex += sizeof(int);
            
            objData.PrefabId = BitConverter.ToInt32(data, startIndex);
            startIndex += sizeof(int);
            
            return objData;
        }
    }
}