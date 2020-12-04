using System.Linq;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    internal static class Verification
    {
        [OpCode(Opcode = Opcode.VERIFY)]
        public static ExecutionResult? Verify(ref ExecutionStack stack)
        {
            if (!stack.Any()) return ExecutionResult.INVALID_STACK;
            if (!stack.PopBool()) return ExecutionResult.FAILURE;
            return null;
        }

        /*
         * This method needs at least a stack with 2 items. The first one (pubkey) 84 bytes the second (signature) 64.
         * stack: { [84], [64], ... }
         */
        [OpCode(Opcode = Opcode.CHECKSIG, MinLengthStack = 2)]
        public static ExecutionResult? CheckSignature(ref ExecutionStack stack)
        {
            //if (stack.Transaction == null) return ExecutionResult.NO_TRANSACTION_GIVEN;
            if (stack.Count < 2) return ExecutionResult.INVALID_STACK;

            //Verify if signature is valid
            //first pop = public key, second is the signature from the unlock script
            
            //stack.Push(new CryptoRSA(stack.Pop()).Verify(stack.Transaction.Hash(), stack.Pop()));
            return null;
        }

        [OpCode(Opcode = Opcode.CHECKSIG_VERIFY)]
        public static ExecutionResult? CheckSignature_Verify(ref ExecutionStack stack)
        {
            if (CheckSignature(ref stack) != null) 
                stack.Push(false);
            return Verify(ref stack);
        }

        [OpCode(Opcode = Opcode.CHECKLOCKTIME)]
        public static ExecutionResult? CheckLockTime(ref ExecutionStack stack)
        {
            //if (stack.Transaction == null) return ExecutionResult.NO_TRANSACTION_GIVEN;
            if (stack.Count < 1) return ExecutionResult.INVALID_STACK;
            if (stack.Peek().Length != 4) return ExecutionResult.INVALID_BYTE_SIZE;

            uint size = stack.PopUInt();
            //stack.Push(stack.Transaction.LockTime >= size);
            return null;
        }

        [OpCode(Opcode = Opcode.CHECKLOCKTIME_VERIFY)]
        public static ExecutionResult? CheckLockTime_Verify(ref ExecutionStack stack)
        {
            if (CheckLockTime(ref stack) != null) 
                stack.Push(false);
            return Verify(ref stack);
        }
    }
}