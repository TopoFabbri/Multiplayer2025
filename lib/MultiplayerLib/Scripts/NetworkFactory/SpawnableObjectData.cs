using System.Numerics;
using Vector3 = Multiplayer.CustomMath.Vector3;

namespace Multiplayer.NetworkFactory
{
    public class SpawnableObjectData
    {
        public int Id { get; set; }
        
        public int PrefabId { get; set; }
        
        public Vector3 Pos { get; set; }
        public Quaternion Rot { get; set; }
    }
}