using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using System.Security.Cryptography;
using BlockChain.Core.Cryptography;

namespace Operations
{
    internal static class Hashing
    {
        [OpCode(OPCODE = OPCODE.SHA256, minLengthStack = 1)]
        public static EXECUTION_RESULT? SHA_256(ref ExecutionStack stack)
        {
            stack.Push(Hash.SHA256(stack.Pop()));
            return null;
        }

        [OpCode(OPCODE = OPCODE.RIPEMD160, minLengthStack = 1)]
        public static EXECUTION_RESULT? RIPEMD_160(ref ExecutionStack stack)
        {
            stack.Push(Hash.RIPEMD160(stack.Pop()));
            return null;
        }

        [OpCode(OPCODE = OPCODE.HASH256, minLengthStack = 1)]
        public static EXECUTION_RESULT? HASH256(ref ExecutionStack stack)
        {
            stack.Push(Hash.HASH256(stack.Pop()));
            return null;
        }

        [OpCode(OPCODE = OPCODE.HASH160, minLengthStack = 1)]
        public static EXECUTION_RESULT? HASH_160(ref ExecutionStack stack)
        {
            stack.Push(Hash.HASH160(stack.Pop()));
            return null;
        }
    }
}
