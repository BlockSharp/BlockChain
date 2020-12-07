namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    internal static class Logic
    {
        [OpCode(Opcode = Opcode.EQUAL)]
        public static ExecutionResult? Equals(ref ExecutionStack stack)
        {
            if (stack.Count < 2) return ExecutionResult.INVALID_STACK;
            stack.Push(System.Linq.Enumerable.SequenceEqual(stack.Pop(), stack.Pop()));
            return null;
        }
            
        [OpCode(Opcode = Opcode.EQ_VERIFY)]
        public static ExecutionResult? EqualVerify(ref ExecutionStack stack)
        {
            if (Equals(ref stack) != null) stack.Push(false);
            return Verification.Verify(ref stack);
        }

        [OpCode(Opcode = Opcode.TRUE)]
        public static void True(ref ExecutionStack stack)
            => stack.Push(true);

        [OpCode(Opcode = Opcode.FALSE)]
        public static void False(ref ExecutionStack stack)
            => stack.Push(false);

        [OpCode(Opcode = Opcode.OP_AND, MinLengthStack = 2)]
        public static void And(ref ExecutionStack stack)
            => stack.Push(stack.PopBool() && stack.PopBool());

        [OpCode(Opcode = Opcode.OP_OR, MinLengthStack = 2)]
        public static void Or(ref ExecutionStack stack)
            => stack.Push(stack.PopBool() || stack.PopBool());

        [OpCode(Opcode = Opcode.OP_NOT, MinLengthStack = 1)]
        public static void Not(ref ExecutionStack stack)
            => stack.Push((stack.PeekByte() <= 1) && !stack.PopBool());
    }
}