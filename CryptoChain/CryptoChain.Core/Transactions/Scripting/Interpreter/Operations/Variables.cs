using System.Linq;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    internal static class Variables
    {
        [OpCode(Opcode = Opcode.SET_VAR, MinLengthStack = 1, MinLengthScript = 1)]
        public static void SetVariable(ref ExecutionStack stack)
        {
            byte slot = stack.CurrentScript.NextByte();
            stack.Variables[slot] = stack.Pop();
        }
        
        [OpCode(Opcode = Opcode.GET_VAR, MinLengthScript = 1)]
        public static void GetVariable(ref ExecutionStack stack)
        {
            byte slot = stack.CurrentScript.NextByte();
            if(stack.Variables.TryGetValue(slot, out byte[]? data))
                stack.Push(data);
        }
        
        [OpCode(Opcode = Opcode.CONCAT, MinLengthStack = 2)]
        public static void Concat(ref ExecutionStack stack)
        {
            byte[] combined = stack.Pop().Concat(stack.Pop()).ToArray();
            stack.Push(combined);
        }
    }
}