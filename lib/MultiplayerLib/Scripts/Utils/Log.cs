using System;
using System.Drawing;
using System.Globalization;

namespace Multiplayer.Utils
{
    public static class Log
    {
        private static bool newLine = true;
        private static ConsoleColor dateColor = ConsoleColor.Cyan;

        public static ConsoleColor Color
        {
            get => Console.ForegroundColor;
            set => Console.ForegroundColor = value;
        }

        public static ConsoleColor DateColor => dateColor;
        
        public static void Write(string message)
        {
            if (newLine)
                WriteDate();
            
            Console.Write(message);
            
            newLine = false;
        }
        
        public static void NewLine(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
                Console.WriteLine();
            
            newLine = true;
        }

        private static void WriteDate()
        {
            ConsoleColor color = Color;
            Color = dateColor;
            
            Console.Write("[");
            
            Console.Write(Timer.DateTime.ToString("HH:mm:ss") + "] ");
            
            Color = color;
        }
    }
}