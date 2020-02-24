using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using System.Security.Cryptography;
using BlockChain.Core.Cryptography;

namespace Operations
{
    /* To-do:
     * All these functions need at least one item in the stack
     */

    internal static class Hashing
    {
        [OpCode(OPCODE = OPCODE.SHA256)]
        public static EXECUTION_RESULT? SHA_256(ref ExecutionStack stack)
        {
            if (stack.Count < 1) return EXECUTION_RESULT.INVALID_STACK;
            using (var sha = SHA256.Create())
                stack.Push(sha.ComputeHash(stack.Pop()));
            return null;
        }

        [OpCode(OPCODE = OPCODE.RIPEMD160)]
        public static EXECUTION_RESULT? RIPEMD_160(ref ExecutionStack stack)
        {
            if (stack.Count < 1) return EXECUTION_RESULT.INVALID_STACK;
            using (var ripe = RIPEMD160.Create())
                stack.Push(ripe.ComputeHash(stack.Pop()));
            return null;
        }

        [OpCode(OPCODE = OPCODE.HASH256)]
        public static EXECUTION_RESULT? HASH256(ref ExecutionStack stack)
        {
            SHA_256(ref stack);
            return SHA_256(ref stack);
        }

        [OpCode(OPCODE = OPCODE.HASH160)]
        public static EXECUTION_RESULT? HASH_160(ref ExecutionStack stack)
        {
            SHA_256(ref stack);
            return RIPEMD_160(ref stack);
        }

    }
}
