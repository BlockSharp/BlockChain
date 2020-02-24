using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;

namespace Operations
{
    internal class Math
    {
        [OpCode(OPCODE = OPCODE.ABS)]
        public static void ABS(ref ExecutionStack stack) => stack.Push(System.Math.Abs(stack.PopShort()));

        [OpCode(OPCODE = OPCODE.MAX)]
        public static void MAX(ref ExecutionStack stack) =>
            stack.Push(System.Math.Max(stack.PopShort(), stack.PopShort()));

        [OpCode(OPCODE = OPCODE.MIN)]
        public static void MIN(ref ExecutionStack stack) =>
            stack.Push(System.Math.Min(stack.PopShort(), stack.PopShort()));
    }
}