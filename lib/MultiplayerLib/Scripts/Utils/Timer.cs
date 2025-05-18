using System;

namespace Multiplayer.Utils
{
    public static class Timer
    {
        public static void Start()
        {
            StartTime = DateTime.Now;
        }

        public static float Time
        {
            get
            {
                if (StartTime != default) return (float)(DateTime.UtcNow - StartTime).TotalSeconds;
                
                Console.WriteLine("Timer not started!");
                return -1f;
            }
        }
        
        public static DateTime DateTime => DateTime.UtcNow;
        public static DateTime DateTimeNow => DateTime.Now;
        public static DateTime StartTime { get; private set; }
    }
}