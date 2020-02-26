using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using BlockChain.Transactions.Scripting.Scripts;
using System.Linq;

namespace Operations
{
    //Special operations on the stack
    internal static class Operations
    {
        [OpCode(OPCODE = OPCODE.DUP, minLengthStack = 1)]
        public static EXECUTION_RESULT? DUP(ref ExecutionStack stack)
        {
            if (!stack.Any()) return EXECUTION_RESULT.INVALID_STACK;
            var item = stack.Pop();
            stack.Push(item, item);
            return null;
        }

        [OpCode(OPCODE = OPCODE.DUP2, minLengthStack = 1)]
        public static EXECUTION_RESULT? DUP2(ref ExecutionStack stack)
        {
            DUP(ref stack);
            return DUP(ref stack);
        }

        [OpCode(OPCODE = OPCODE.EVAL_SCRIPT, minLengthStack = 2)]
        public static void EVAL_SCRIPT(ref ExecutionStack stack)
        {
            Script lockScript = new Script(stack.Pop());
            Script unlockScript = new Script(stack.Pop());

            unlockScript.InsertScript(lockScript);

            stack.Script.AddRange(unlockScript.Reverse().ToArray());
        }
    }
}
