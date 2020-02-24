using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using System.Linq;

namespace Operations
{
    //Special operations on the stack
    internal static class Operations
    {
        [OpCode(OPCODE = OPCODE.DUP)]
        public static EXECUTION_RESULT? DUP(ref ExecutionStack stack)
        {
            if (!stack.Any()) return EXECUTION_RESULT.INVALID_STACK;
            var item = stack.Pop();
            stack.Push(item, item);
            return null;
        }

        [OpCode(OPCODE = OPCODE.DUP2)]
        public static EXECUTION_RESULT? DUP2(ref ExecutionStack stack)
        {
            DUP(ref stack);
            return DUP(ref stack);
        }
    }
}
