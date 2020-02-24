using System.Linq;
using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;

namespace Operations
{
    internal static class Verify
    {
        [OpCode(OPCODE = OPCODE.VERIFY)]
        public static EXECUTION_RESULT? VERIFY(ref ExecutionStack stack)
        {
            if (!stack.Any()) return EXECUTION_RESULT.INVALID_STACK;
            else if (!stack.PopBool()) return EXECUTION_RESULT.FAILURE;
            else return null;
        }
    }
}