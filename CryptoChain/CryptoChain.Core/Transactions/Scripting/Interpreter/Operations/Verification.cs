using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.RSA;

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
            if (stack.Count < 2) return ExecutionResult.INVALID_STACK;

            var pubKey = stack.Pop();
            var signature = stack.Pop();
            var rsa = new CryptoRsa(pubKey);
            stack.Push(rsa.Verify(stack.Transaction.TxId, signature));
            return null;
        }

        [OpCode(Opcode = Opcode.CHECKSIG_VERIFY)]
        public static ExecutionResult? CheckSignature_Verify(ref ExecutionStack stack)
        {
            if (CheckSignature(ref stack) != null) 
                stack.Push(false);
            return Verify(ref stack);
        }

        [OpCode(Opcode = Opcode.CHECKMULTISIG, MinLengthStack = 3)]
        public static ExecutionResult? CheckMultiSig(ref ExecutionStack stack)
        {
            int amount = stack.PopShort();
            if (stack.Count < amount) return ExecutionResult.INVALID_STACK;
            var pubKeys = stack.PopRange(amount);
            int minValidAmount = stack.PopShort();
            int signatureCount = stack.PopShort();
            if (stack.Count < signatureCount) return ExecutionResult.INVALID_STACK;
            var signatures = stack.PopRange(signatureCount);

            if (signatureCount < minValidAmount)
            {
                stack.Push(false);
                return null;
            }
            
            var transactionHash = stack.Transaction.TxId;

            var results = pubKeys.ToDictionary(x => x, x => false);
            foreach (var x in results.Keys.ToList())
            {
                var rsa = new CryptoRsa(new RsaKey(x));
                foreach (var sig in signatures)
                    if (rsa.Verify(transactionHash, sig))
                        results[x] = true;
            }
            
            
            stack.Push(results.Count(x => x.Value) >= minValidAmount);
            return null;
        }

        [OpCode(Opcode = Opcode.CHECKLOCKTIME, MinLengthStack = 1)]
        public static ExecutionResult? CheckLockTime(ref ExecutionStack stack)
        {
            //unlocked at unix time stamp
            if (stack.Transaction.LockTime >= 100000000)
            {
                uint epochNow = (uint)(DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
                stack.Push(epochNow > stack.Transaction.LockTime);
            }
            else //unlocked at blockheight  //TODO: get blockheight
                stack.Push(false);
            
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