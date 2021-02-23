using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Algorithms;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    //Special operations on the stack
    internal static class Other
    {
        [OpCode(Opcode = Opcode.DUP, MinLengthStack = 1)]
        public static ExecutionResult? Duplicate(ref ExecutionStack stack)
        {
            if (!stack.Any()) return ExecutionResult.INVALID_STACK;
            var item = stack.Pop();
            stack.Push(item, item);
            return null;
        }

        [OpCode(Opcode = Opcode.DUP2, MinLengthStack = 1)]
        public static ExecutionResult? DblDuplicate(ref ExecutionStack stack)
        {
            Duplicate(ref stack);
            return Duplicate(ref stack);
        }

        [OpCode(Opcode = Opcode.RETURN)]
        public static ExecutionResult? Return(ref ExecutionStack stack)
        {
            stack.Push(stack.CurrentScript.Serialize());
            return ExecutionResult.EXECUTION_STOPPED;
        }

        [OpCode(Opcode = Opcode.EVAL_SCRIPT, MinLengthStack = 1)]
        public static void Evaluate(ref ExecutionStack stack)
        {
            var s = new Script(stack.Pop());
            stack.CurrentScript.Add(s);
        }

        [OpCode(Opcode = Opcode.PUSH_TIMESTAMP)]
        public static void PushUnixTimestamp(ref ExecutionStack stack)
        {
            var unixTimestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            stack.Push(unixTimestamp);
        }

        [OpCode(Opcode = Opcode.ALG_RSA)]
        public static void SetAlgorithmRSA(ref ExecutionStack stack)
            => stack.Push((byte)Algorithm.RSA);
        
        [OpCode(Opcode = Opcode.ALG_ECDSA)]
        public static void SetAlgorithmECDSA(ref ExecutionStack stack)
            => stack.Push((byte)Algorithm.ECDSA);
    }
}