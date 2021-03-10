using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

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

        public static void Log(BigInteger nr) => Log("0x" + nr.ToString("x2"));
        public static void Log(params object[] message) => WriteLine(string.Join(' ', message).TrimEnd(), MessageState.LOG);
        public static void Info(params object[] message) => WriteLine(string.Join(' ', message).TrimEnd(), MessageState.INFO);
        public static void ImHere([CallerMemberName] string callingMethod = "",[CallerLineNumber]int line = 0) 
            => WriteLine($"I am here: {callingMethod}() ln: {line}", MessageState.INFO);

        public static void WriteLine(object message, ConsoleColor color = ConsoleColor.Gray)
            => Write(message + "\n", color);

        public static void WriteLine(object message, MessageState state)
            => Write(message + "\n", state);

        public static void Write(object message, ConsoleColor color = ConsoleColor.Gray)
        {
            if(!IsDebug)
                return;
            
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        public static void Write(object message, MessageState state)
            => Write($"[{state.ToString()}] {message}", state switch
            {
                MessageState.INFO => ConsoleColor.Blue,
                MessageState.LOG => ConsoleColor.White,
                MessageState.WARNING => ConsoleColor.Yellow,
                MessageState.ERROR => ConsoleColor.Red,
                MessageState.SUCCESS => ConsoleColor.Green,
                _ => ConsoleColor.Gray
            });

        public static TimeSpan TimeAverage(Action function, int iterations = 100)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                function();
            }
            return sw.Elapsed / iterations;
        }
    }
}