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
        public static void Duplicate(ref ExecutionStack stack)
        {
            var item = stack.Pop();
            stack.Push(item, item);
        }

        [OpCode(Opcode = Opcode.DUP2, MinLengthStack = 1)]
        public static void DblDuplicate(ref ExecutionStack stack)
        {
            Duplicate(ref stack);
            Duplicate(ref stack);
        }

        [OpCode(Opcode = Opcode.RETURN)]
        public static ExecutionResult? Return(ref ExecutionStack stack)
        {
            stack.Push(stack.CurrentScript.Serialize());
            return ExecutionResult.EXECUTION_STOPPED;
        }
        
        [OpCode(Opcode = Opcode.EXIT)]
        public static ExecutionResult? Exit(ref ExecutionStack stack)
        {
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

        [OpCode(Opcode = Opcode.PUSH_BLOCK_HEIGHT)]
        public static void PushBlockHeight(ref ExecutionStack stack)
            => stack.Push(stack.BlockHeight);

        [OpCode(Opcode = Opcode.ALG_RSA)]
        public static void SetAlgorithmRsa(ref ExecutionStack stack)
            => stack.CurrentAlgorithm = Algorithm.RSA;

        [OpCode(Opcode = Opcode.ALG_ECDSA)]
        public static void SetAlgorithmEcdsa(ref ExecutionStack stack)
            => stack.CurrentAlgorithm = Algorithm.ECDSA;
    }
}