using System.Linq;
using BlockChain.Core.Cryptography.RSA;
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

        /*
         * This method needs at least a stack with 2 items. The first one (pubkey) 84 bytes the second (signature) 64.
         * stack: { [84], [64], ... }
         */
        [OpCode(OPCODE = OPCODE.CHECKSIG)]
        public static EXECUTION_RESULT? CHECKSIG(ref ExecutionStack stack)
        {
            if (stack.Transaction == null) return EXECUTION_RESULT.NO_TRANSACTION_GIVEN;
            if (stack.Count < 2) return EXECUTION_RESULT.INVALID_STACK;

            //Verify if signature is valid
            stack.Push(new CryptoRSA(stack.Pop()).Verify(stack.Transaction.Hash(), stack.Pop()));
            return null;
        }

        [OpCode(OPCODE = OPCODE.CHECKSIG_VERIFY)]
        public static EXECUTION_RESULT? CHECKSIG_VERIFY(ref ExecutionStack stack)
        {
            if (CHECKSIG(ref stack) != null) stack.Push(false);
            return VERIFY(ref stack);
        }

        [OpCode(OPCODE = OPCODE.CHECKLOCKTIME)]
        public static EXECUTION_RESULT? CHECKLOCKTIME(ref ExecutionStack stack)
        {
            if (stack.Transaction == null) return EXECUTION_RESULT.NO_TRANSACTION_GIVEN;
            if (stack.Count() < 1) return EXECUTION_RESULT.INVALID_STACK;
            if (stack.Peek().Length != 4) return EXECUTION_RESULT.INVALID_BYTE_SIZE;

            uint size = stack.PopUInt();
            stack.Push(stack.Transaction.lockTime >= size);
            return null;
        }

        [OpCode(OPCODE = OPCODE.CHECKLOCKTIME_VERIFY)]
        public static EXECUTION_RESULT? CHECKLOCKTIME_VERIFY(ref ExecutionStack stack)
        {
            if (CHECKLOCKTIME(ref stack) != null) stack.Push(false);
            return VERIFY(ref stack);
        }
    }
}