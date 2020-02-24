using System.Linq;
using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using BlockChain.Transactions.Scripting.Scripts;

namespace Operations.Constants
{
    internal static class AddData
    {
        [OpCode(OPCODE = OPCODE.OP_1)]
        public static void OP_1(ref ExecutionStack stack) => stack.Push((short) 1);

        [OpCode(OPCODE = OPCODE.OP_2)]
        public static void OP_2(ref ExecutionStack stack) => stack.Push((short) 2);

        [OpCode(OPCODE = OPCODE.OP_3)]
        public static void OP_3(ref ExecutionStack stack) => stack.Push((short) 3);

        [OpCode(OPCODE = OPCODE.OP_4)]
        public static void OP_4(ref ExecutionStack stack) => stack.Push((short) 4);

        [OpCode(OPCODE = OPCODE.OP_5)]
        public static void OP_5(ref ExecutionStack stack) => stack.Push((short) 5);

        [OpCode(OPCODE = OPCODE.OP_6)]
        public static void OP_6(ref ExecutionStack stack) => stack.Push((short) 6);

        [OpCode(OPCODE = OPCODE.OP_7)]
        public static void OP_7(ref ExecutionStack stack) => stack.Push((short) 7);

        [OpCode(OPCODE = OPCODE.OP_8)]
        public static void OP_8(ref ExecutionStack stack) => stack.Push((short) 8);

        [OpCode(OPCODE = OPCODE.OP_9)]
        public static void OP_9(ref ExecutionStack stack) => stack.Push((short) 9);

        [OpCode(OPCODE = OPCODE.OP_10)]
        public static void OP_10(ref ExecutionStack stack) => stack.Push((short) 10);

        [OpCode(OPCODE = OPCODE.OP_11)]
        public static void OP_11(ref ExecutionStack stack) => stack.Push((short) 11);

        [OpCode(OPCODE = OPCODE.OP_12)]
        public static void OP_12(ref ExecutionStack stack) => stack.Push((short) 12);

        [OpCode(OPCODE = OPCODE.OP_13)]
        public static void OP_13(ref ExecutionStack stack) => stack.Push((short) 13);

        [OpCode(OPCODE = OPCODE.OP_14)]
        public static void OP_14(ref ExecutionStack stack) => stack.Push((short) 14);

        [OpCode(OPCODE = OPCODE.OP_15)]
        public static void OP_15(ref ExecutionStack stack) => stack.Push((short) 15);

        [OpCode(OPCODE = OPCODE.OP_16)]
        public static void OP_16(ref ExecutionStack stack) => stack.Push((short) 16);

        [OpCode(OPCODE = OPCODE.PUSHDATA_1)]
        public static void PUSHDATA_1(ref ExecutionStack stack)
            => stack.Push(stack.Script.DequeueRange(stack.Script.Dequeue()));

        [OpCode(OPCODE = OPCODE.PUSHDATA_2)]
        public static void PUSHDATA_2(ref ExecutionStack stack)
            => stack.Push(stack.Script.DequeueRange(System.BitConverter.ToInt16(stack.Script.DequeueRange(2))));

        [OpCode(OPCODE = OPCODE.PUSHDATA_4)]
        public static void PUSHDATA_4(ref ExecutionStack stack)
            => stack.Push(stack.Script.DequeueRange(System.BitConverter.ToInt32(stack.Script.DequeueRange(4))));

        [OpCode(OPCODE = OPCODE.PUSH_INT)]
        public static void PUSH_INT(ref ExecutionStack stack)
            => stack.Push(System.BitConverter.ToInt32(stack.Script.DequeueRange(4)));

        [OpCode(OPCODE = OPCODE.PUSH_UINT)]
        public static void PUSH_UINT(ref ExecutionStack stack)
            => stack.Push(System.BitConverter.ToUInt32(stack.Script.DequeueRange(4)));

        [OpCode(OPCODE = OPCODE.PUSH_SHORT)]
        public static void PUSH_SHORT(ref ExecutionStack stack)
            => stack.Push(System.BitConverter.ToInt16(stack.Script.DequeueRange(2)));

        [OpCode(OPCODE = OPCODE.PUBKEY)]
        public static void PUBKEY(ref ExecutionStack stack)
            => stack.Push(stack.Script.DequeueRange(Script.PubKeySize));

        [OpCode(OPCODE = OPCODE.SIGNATURE)]
        public static void SIGNATURE(ref ExecutionStack stack)
            => stack.Push(stack.Script.DequeueRange(Script.SignatureSize));

        [OpCode(OPCODE = OPCODE.PUBKEY_HASH)]
        public static void PUBKEY_HASH(ref ExecutionStack stack)
            => stack.Push(stack.Script.DequeueRange(Script.AddressSize));
    }
}