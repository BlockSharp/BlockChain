using System.Linq;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    //Special operations on the stack
    internal static class Other
    {
        [OpCode(Opcode = Opcode.DUP, MinLengthStack = 1)]
        public static ExecutionResult? Duplicate(ref ExecutionStack stack)
        {
            if (!stack.Any()) return ExecutionResult.INVALID_STACK;
            var item = stack.Pop();
            stack.Push(item, item);
            return null;
        }

        [OpCode(Opcode = Opcode.DUP2, MinLengthStack = 1)]
        public static ExecutionResult? DblDuplicate(ref ExecutionStack stack)
        {
            Duplicate(ref stack);
            return Duplicate(ref stack);
        }

        [OpCode(Opcode = Opcode.RETURN)]
        public static ExecutionResult? Return(ref ExecutionStack stack)
        {
            stack.Push(stack.CurrentScript.Serialize());
            return ExecutionResult.EXECUTION_STOPPPED;
        }

        [OpCode(Opcode = Opcode.EVAL_SCRIPT, MinLengthStack = 1)]
        public static void Evaluate(ref ExecutionStack stack)
        {
            IScript s = new Script(stack.Pop());
            stack.CurrentScript.AddRange(s.Serialize());
        }
    }
}