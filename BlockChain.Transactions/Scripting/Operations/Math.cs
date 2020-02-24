using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;

namespace Operations
{
    /* To-do:
     *  all methods except abs and disabled codes need at least a stack with 2 items: each 2 bytes (short). 
     *  stack: { [2], [2], ... } 
     */
    internal static class Math
    {
        [OpCode(OPCODE = OPCODE.ABS)]
        public static void ABS(ref ExecutionStack stack)
            => stack.Push(System.Math.Abs(stack.PopShort()));

        [OpCode(OPCODE = OPCODE.MAX, minLengthStack = 2)]
        public static void MAX(ref ExecutionStack stack)
            => stack.Push(System.Math.Max(stack.PopShort(), stack.PopShort()));

        [OpCode(OPCODE = OPCODE.MIN, minLengthStack = 2)]
        public static void MIN(ref ExecutionStack stack)
            => stack.Push(System.Math.Min(stack.PopShort(), stack.PopShort()));

        [OpCode(OPCODE = OPCODE.MULTIPLY)]
        [OpCode(OPCODE = OPCODE.DIVIDE)]
        [OpCode(OPCODE = OPCODE.MODULO)]
        public static EXECUTION_RESULT? DISABLED(ref ExecutionStack stack)
            => EXECUTION_RESULT.DISABLED_CODE;

        [OpCode(OPCODE = OPCODE.ADD, minLengthStack = 2)]
        public static void ADD(ref ExecutionStack stack)
            => stack.Push(stack.PopShort() + stack.PopShort());

        [OpCode(OPCODE = OPCODE.SUBSTRACT, minLengthStack = 2)]
        public static void SUBSTRACT(ref ExecutionStack stack)
            => stack.Push(stack.PopShort() - stack.PopShort());

        [OpCode(OPCODE = OPCODE.LESSTHAN, minLengthStack = 2)]
        public static void LESSTHAN(ref ExecutionStack stack)
            => stack.Push(stack.PopShort() < stack.PopShort());

        [OpCode(OPCODE = OPCODE.LESSOREQUAL, minLengthStack = 2)]
        public static void LESSOREQUAL(ref ExecutionStack stack)
            => stack.Push(stack.PopShort() <= stack.PopShort());

        [OpCode(OPCODE = OPCODE.GREATERTHAN, minLengthStack = 2)]
        public static void GREATERTHAN(ref ExecutionStack stack)
            => stack.Push(stack.PopShort() > stack.PopShort());

        [OpCode(OPCODE = OPCODE.GREATEROREQUAL, minLengthStack = 2)]
        public static void GREATEROREQUAL(ref ExecutionStack stack)
            => stack.Push(stack.PopShort() >= stack.PopShort());

        [OpCode(OPCODE = OPCODE.EQ_NUM, minLengthStack = 2)]
        public static void EQ_NUM(ref ExecutionStack stack)
            => stack.Push(stack.PopShort() == stack.PopShort());

        [OpCode(OPCODE = OPCODE.EQ_VERIFY_NUM, minLengthStack = 2)]
        public static EXECUTION_RESULT? EQ_VERIFY_NUM(ref ExecutionStack stack)
        {
            stack.Push(stack.PopShort() == stack.PopShort());
            return Verify.VERIFY(ref stack);
        }
    }
}