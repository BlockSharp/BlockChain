namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    /* To-do:
     *  all methods except abs and disabled codes need at least a stack with 2 items: each 2 bytes (short). 
     *  stack: { [2], [2], ... } 
     */
    internal static class Math
    {
        [OpCode(Opcode = Opcode.ABS)]
        public static void Abs(ref ExecutionStack stack)
            => stack.Push(System.Math.Abs(stack.PopShort()));

        [OpCode(Opcode = Opcode.MAX, MinLengthStack = 2)]
        public static void Max(ref ExecutionStack stack)
            => stack.Push(System.Math.Max(stack.PopShort(), stack.PopShort()));

        [OpCode(Opcode = Opcode.MIN, MinLengthStack = 2)]
        public static void Min(ref ExecutionStack stack)
            => stack.Push(System.Math.Min(stack.PopShort(), stack.PopShort()));

        [OpCode(Opcode = Opcode.MULTIPLY)]
        [OpCode(Opcode = Opcode.DIVIDE)]
        [OpCode(Opcode = Opcode.MODULO)]
        [OpCode(Opcode = Opcode.NEGATE)]
        public static ExecutionResult? Disabled(ref ExecutionStack stack)
            => ExecutionResult.DISABLED_CODE;

        [OpCode(Opcode = Opcode.ADD, MinLengthStack = 2)]
        public static void Add(ref ExecutionStack stack)
            => stack.Push((short)(stack.PopShort() + stack.PopShort()));

        [OpCode(Opcode = Opcode.SUBTRACT, MinLengthStack = 2)]
        public static void Subtract(ref ExecutionStack stack)
            => stack.Push((short)(stack.PopShort() - stack.PopShort()));

        [OpCode(Opcode = Opcode.LESS_THAN, MinLengthStack = 2)]
        public static void LessThan(ref ExecutionStack stack)
            // ReSharper disable once EqualExpressionComparison
            => stack.Push(stack.PopShort() < stack.PopShort());

        [OpCode(Opcode = Opcode.LESS_OR_EQ, MinLengthStack = 2)]
        public static void LessOrEqual(ref ExecutionStack stack)
            // ReSharper disable once EqualExpressionComparison
            => stack.Push(stack.PopShort() <= stack.PopShort());

        [OpCode(Opcode = Opcode.GREATER_THAN, MinLengthStack = 2)]
        public static void GreaterThan(ref ExecutionStack stack)
            // ReSharper disable once EqualExpressionComparison
            => stack.Push(stack.PopShort() > stack.PopShort());

        [OpCode(Opcode = Opcode.GREATER_OR_EQ, MinLengthStack = 2)]
        public static void GreaterOrEqual(ref ExecutionStack stack)
            // ReSharper disable once EqualExpressionComparison
            => stack.Push(stack.PopShort() >= stack.PopShort());

        [OpCode(Opcode = Opcode.EQ_NUM, MinLengthStack = 2)]
        public static void EQ_NUM(ref ExecutionStack stack)
            // ReSharper disable once EqualExpressionComparison
            => stack.Push(stack.PopShort() == stack.PopShort());

        [OpCode(Opcode = Opcode.EQ_VERIFY_NUM, MinLengthStack = 2)]
        public static ExecutionResult? EQ_VERIFY_NUM(ref ExecutionStack stack)
        {
            EQ_NUM(ref stack);
            return Verification.Verify(ref stack);
        }
    }
}