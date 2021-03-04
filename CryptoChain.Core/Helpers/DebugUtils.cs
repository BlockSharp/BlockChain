using System;

namespace CryptoChain.Core.Helpers
{
    public static class DebugUtils
    {
        public enum MessageState
        {
            SUCCESS, INFO, WARNING, ERROR, LOG
        }
        
        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public static void Log(string message) => WriteLine(message, MessageState.LOG);
        public static void Info(string message) => WriteLine(message, MessageState.INFO);


        public static void WriteLine(string message, ConsoleColor color = ConsoleColor.Gray)
            => Write(message + "\n", color);

        public static void WriteLine(string message, MessageState state)
            => Write(message + "\n", state);

        public static void Write(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            if(!IsDebug)
                return;
            
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void Write(string message, MessageState state)
            => Write($"[{state.ToString()}] {message}", state switch
            {
                MessageState.INFO => ConsoleColor.Blue,
                MessageState.LOG => ConsoleColor.White,
                MessageState.WARNING => ConsoleColor.Yellow,
                MessageState.ERROR => ConsoleColor.Red,
                MessageState.SUCCESS => ConsoleColor.Green,
                _ => ConsoleColor.Gray
            });
    }
}