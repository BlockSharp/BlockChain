using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    internal static class Hashing
    {
        [OpCode(Opcode = Opcode.SHA256, MinLengthStack = 1)]
        public static void SHA_256(ref ExecutionStack stack)
            => stack.Push(Hash.SHA_256(stack.Pop()));

        [OpCode(Opcode = Opcode.RIPEMD160, MinLengthStack = 1)]
        public static void RIPEMD_160(ref ExecutionStack stack)
            => stack.Push(Hash.RIPEMD_160(stack.Pop()));

        [OpCode(Opcode = Opcode.HASH256, MinLengthStack = 1)]
        public static void HASH_256(ref ExecutionStack stack)
            => stack.Push(Hash.HASH_256(stack.Pop()));

        [OpCode(Opcode = Opcode.HASH160, MinLengthStack = 1)]
        public static void HASH_160(ref ExecutionStack stack)
            => stack.Push(Hash.HASH_160(stack.Pop()));
    }
}