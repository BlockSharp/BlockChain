

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

        [OpCode(OPCODE = OPCODE.TRUE)]
        public static void TRUE(ref ExecutionStack stack)
            => stack.Push(true);

        [OpCode(OPCODE = OPCODE.FALSE)]
        public static void FALSE(ref ExecutionStack stack)
            => stack.Push(false);

        [OpCode(OPCODE = OPCODE.OP_AND, minLengthStack = 2)]
        public static void AND(ref ExecutionStack stack)
            => stack.Push(stack.PopBool() && stack.PopBool());

        [OpCode(OPCODE = OPCODE.OP_OR, minLengthStack = 2)]
        public static void OR(ref ExecutionStack stack)
            => stack.Push(stack.PopBool() || stack.PopBool());

        [OpCode(OPCODE = OPCODE.OP_NOT, minLengthStack = 1)]
        public static void NOT(ref ExecutionStack stack)
            => stack.Push((stack.PeekByte() <= 1) ? !stack.PopBool() : false);
    }
}
