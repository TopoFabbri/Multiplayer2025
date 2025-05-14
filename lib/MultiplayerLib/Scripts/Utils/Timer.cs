using System;

namespace Utils
{
    public static class Timer
    {
        public static void Start()
        {
            StartTime = DateTime.Now;
        }

        public static float Time => (float)(DateTime.UtcNow - StartTime).TotalSeconds;
        public static DateTime DateTime => DateTime.UtcNow;
        public static DateTime StartTime { get; private set; }
    }
}