using Multiplayer.Reflection;

namespace Objects
{
    public struct TestColor
    {
        [Sync] public float R;
        [Sync] public float G;
        [Sync] public float B;
        [Sync] public float A;

        public TestColor(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}