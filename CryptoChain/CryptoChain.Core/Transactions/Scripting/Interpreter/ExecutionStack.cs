using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter
{
    /// <summary>
    /// Class that will be passed to every operator method
    /// Holds all values that are used during runtime of script
    /// </summary>
    public class ExecutionStack : Stack<byte[]>
    {
        //Script and transaction to provide information for specific OPCodes
        public IScript CurrentScript { get; private set; }
        public Transaction Transaction { get; set; }

        public void SetScript(ref Script script)
            => CurrentScript = script;

        //base keyword is important! Else it is recursive
        public void Push(params byte[][] bytes) => bytes.ToList().ForEach(f => base.Push(f));
        public void Push(byte b) => Push(new[] {b});
        public void Push(short n) => Push(BitConverter.GetBytes(n));
        public void Push(uint n) => Push(BitConverter.GetBytes(n));
        public void Push(int n) => Push(BitConverter.GetBytes(n));
        public void Push(bool b) => Push(Convert.ToByte(b));
        public byte PopByte() => Pop()[0];
        public byte PeekByte() => Peek()[0];
        public bool PopBool()
        {
            byte d = PopByte();
            if (d == 0 || d == 1) return d == 1;
            return Convert.ToBoolean(d);
        }
        public bool PeekBool()
        {
            byte d = PeekByte();
            if (d == 0 || d == 1) return d == 1;
            return Convert.ToBoolean(d);
        }
        public short PopShort() => Peek().Length != 2 ? (short) 0 : BitConverter.ToInt16(Pop());
        public short PeekShort() => Peek().Length != 2 ? (short) 0 : BitConverter.ToInt16(Peek());
        public uint PopUInt() => Peek().Length != 4 ? 0 : BitConverter.ToUInt32(Pop());
        public uint PeekUInt() => Peek().Length != 4 ? 0 : BitConverter.ToUInt32(Peek());
        public int PopInt() => Peek().Length != 4 ? 0 : BitConverter.ToInt32(Pop());
        public int PeekInt() => Peek().Length != 4 ? 0 : BitConverter.ToInt32(Peek());

        /// <summary>
        /// Helps with debugging. Prints current stack to console in human-readable format
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Stack is now: [ ");
            for (int i = 0; i < Count; i++)
            {
                byte[] s = this.Skip(i).Take(1).First();
                if (s.Length == 1)
                {
                    byte b = s[0];
                    if (b == 0 || b == 1)
                        sb.Append(Convert.ToBoolean(b) + " ");
                    else
                        sb.Append(b + " ");
                }
                else if (s.Length == 2)
                    sb.Append(BitConverter.ToInt16(s, 0) + " ");
                else if (s.Length == 4) //commonly uint
                    sb.Append(BitConverter.ToUInt32(s, 0) + " ");
                else
                    sb.Append($"ARR({s.Length}) ");
            }

            sb.Append("]\n\n");
            return sb.ToString();
        }
    }
}