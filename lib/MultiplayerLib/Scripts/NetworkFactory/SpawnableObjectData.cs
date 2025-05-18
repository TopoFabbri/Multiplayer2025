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
        
        public Vector3 Pos { get; set; }
        public Quaternion Rot { get; set; }

        public byte[] Serialized => Serialize(this);
        
        public static int Size => sizeof(int) * 3 + sizeof(float) * 7;

        public static byte[] Serialize(SpawnableObjectData data)
        {
            List<byte> outData = new();

            outData.AddRange(BitConverter.GetBytes(data.Id));
            
            outData.AddRange(BitConverter.GetBytes(data.OwnerId));
            
            outData.AddRange(BitConverter.GetBytes(data.PrefabId));
            
            outData.AddRange(BitConverter.GetBytes(data.Pos.x));
            outData.AddRange(BitConverter.GetBytes(data.Pos.y));
            outData.AddRange(BitConverter.GetBytes(data.Pos.z));
            
            outData.AddRange(BitConverter.GetBytes(data.Rot.X));
            outData.AddRange(BitConverter.GetBytes(data.Rot.Y));
            outData.AddRange(BitConverter.GetBytes(data.Rot.Z));
            outData.AddRange(BitConverter.GetBytes(data.Rot.W));
                
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
            
            objData.Pos = new Vector3(
                BitConverter.ToSingle(data, startIndex), 
                BitConverter.ToSingle(data, startIndex + sizeof(float)), 
                BitConverter.ToSingle(data, startIndex + sizeof(float) * 2));
            startIndex += sizeof(float) * 3;
            
            objData.Rot = new Quaternion(
                BitConverter.ToSingle(data, startIndex), 
                BitConverter.ToSingle(data, startIndex + sizeof(float)), 
                BitConverter.ToSingle(data, startIndex + sizeof(float) * 2), 
                BitConverter.ToSingle(data, startIndex + sizeof(float) * 3));
            startIndex += sizeof(float) * 4;
            
            return objData;
        }
    }
}