using System;
using System.Linq;
using System.Collections.Generic;
using BlockChain.Transactions.Scripting.Scripts;

namespace BlockChain.Transactions.Scripting
{
    /// <summary>
    /// Class that will be passed to every operator method
    /// Holds all values that are used during runtime of script
    /// </summary>
    public class ExecutionStack : Stack<byte[]>
    {
        public readonly Script Script;
        public readonly Transaction Transaction;

        public ExecutionStack(ref Script script) => Script = script;
        public ExecutionStack(ref Script script, ref Transaction transaction) : this(ref script) => Transaction = transaction;
        
        //base keyword is important! Else it is recursive
        public void Push(params byte[][] bytes) => bytes.ToList().ForEach(f => base.Push(f));
        public void Push(byte b) => Push(new [] { b });
        public void Push(short n) => Push(BitConverter.GetBytes(n));
        public void Push(uint n) => Push(BitConverter.GetBytes(n));
        public void Push(int n) => Push(BitConverter.GetBytes(n));
        public void Push(bool b) => Push(Convert.ToByte(b));

        public byte PopByte() => Pop()[0];
        public byte PeekByte() => Peek()[0];
        public bool PopBool() => Convert.ToBoolean(Pop()[0]);
        public bool PeekBool() => Convert.ToBoolean(Peek()[0]);
        public short PopShort() => Peek().Length != 2 ? (short)0 : BitConverter.ToInt16(Pop());
        public short PeekShort() => Peek().Length != 2 ? (short)0 : BitConverter.ToInt16(Peek());
        public uint PopUInt() => Peek().Length != 4 ? 0 : BitConverter.ToUInt32(Pop());
        public uint PeekUInt() => Peek().Length != 4 ? 0 : BitConverter.ToUInt32(Peek());
        public int PopInt() => Peek().Length != 4 ? 0 : BitConverter.ToInt32(Pop());
        public int PeekInt() => Peek().Length != 4 ? 0 : BitConverter.ToInt32(Peek());

#if DEBUG
        /// <summary>
        /// Helps with debugging. Prints current stack to console in human-readable format
        /// </summary>
        public void PrintStack()
        {
            Console.Write("Stack is now: [ ");
            for (int i = 0; i < this.Count(); i++)
            {
                byte[] s = this.Skip(i).Take(1).First();
                if (s.Length == 1)
                {
                    byte b = s[0];
                    if (b == 0 || b == 1)
                        Console.Write(Convert.ToBoolean(b) + " ");
                    else
                        Console.Write(b + " ");
                }
                else if (s.Length == 2)
                    Console.Write(BitConverter.ToInt16(s, 0) + " ");
                else if (s.Length == 4) //commonly uint
                    Console.Write(BitConverter.ToUInt32(s, 0) + " ");
                else
                    Console.Write($"ARR({s.Length}) ");
            }
            Console.Write("]\n\n");
        }
#endif
    }
}