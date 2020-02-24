

using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;

namespace Operations
{
    internal static class Logic
    {
        [OpCode(OPCODE = OPCODE.EQUALS)]
        public static EXECUTION_RESULT? EQUALS(ref ExecutionStack stack)
        {
            if (stack.Count < 2) return EXECUTION_RESULT.INVALID_STACK;
            stack.Push(System.Linq.Enumerable.SequenceEqual(stack.Pop(), stack.Pop()));
            return null;
        }
            
        [OpCode(OPCODE = OPCODE.EQ_VERIFY)]
        public static EXECUTION_RESULT? EQ_VERIFY(ref ExecutionStack stack)
        {
            if (EQUALS(ref stack) != null) stack.Push(false);
            return Verify.VERIFY(ref stack);
        }
    }
}
