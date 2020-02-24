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
        public Script Script;
        public Transaction Transaction;

        public ExecutionStack(ref Script script) => Script = script;
        public ExecutionStack(ref Script script, ref Transaction transcation) : this(ref script) => this.Transaction = transcation;
        
        //Helper methods
        public void Push(byte b) => Push(new [] { b });
        public void Push(short n) => Push(BitConverter.GetBytes(n));
        public void Push(uint n) => Push(BitConverter.GetBytes(n));
        public void Push(int n) => Push(BitConverter.GetBytes(n));
        public void Push(bool b) => Push(Convert.ToByte(b));

        //base keyword is important! Else it is recursive
        public void Push(params byte[][] bytes)
            => bytes.ToList().ForEach(f => base.Push(f));

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
    }
}