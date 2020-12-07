using System;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    internal static class Constants
    {
        [OpCode(Opcode = Opcode.OP_1)]
        public static void OP_1(ref ExecutionStack stack) => stack.Push((short) 1);

        [OpCode(Opcode = Opcode.OP_2)]
        public static void OP_2(ref ExecutionStack stack) => stack.Push((short) 2);

        [OpCode(Opcode = Opcode.OP_3)]
        public static void OP_3(ref ExecutionStack stack) => stack.Push((short) 3);

        [OpCode(Opcode = Opcode.OP_4)]
        public static void OP_4(ref ExecutionStack stack) => stack.Push((short) 4);

        [OpCode(Opcode = Opcode.OP_5)]
        public static void OP_5(ref ExecutionStack stack) => stack.Push((short) 5);

        [OpCode(Opcode = Opcode.OP_6)]
        public static void OP_6(ref ExecutionStack stack) => stack.Push((short) 6);

        [OpCode(Opcode = Opcode.OP_7)]
        public static void OP_7(ref ExecutionStack stack) => stack.Push((short) 7);

        [OpCode(Opcode = Opcode.OP_8)]
        public static void OP_8(ref ExecutionStack stack) => stack.Push((short) 8);

        [OpCode(Opcode = Opcode.OP_9)]
        public static void OP_9(ref ExecutionStack stack) => stack.Push((short) 9);

        [OpCode(Opcode = Opcode.OP_10)]
        public static void OP_10(ref ExecutionStack stack) => stack.Push((short) 10);

        [OpCode(Opcode = Opcode.OP_11)]
        public static void OP_11(ref ExecutionStack stack) => stack.Push((short) 11);

        [OpCode(Opcode = Opcode.OP_12)]
        public static void OP_12(ref ExecutionStack stack) => stack.Push((short) 12);

        [OpCode(Opcode = Opcode.OP_13)]
        public static void OP_13(ref ExecutionStack stack) => stack.Push((short) 13);

        [OpCode(Opcode = Opcode.OP_14)]
        public static void OP_14(ref ExecutionStack stack) => stack.Push((short) 14);

        [OpCode(Opcode = Opcode.OP_15)]
        public static void OP_15(ref ExecutionStack stack) => stack.Push((short) 15);

        [OpCode(Opcode = Opcode.OP_16)]
        public static void OP_16(ref ExecutionStack stack) => stack.Push((short) 16);

        [OpCode(Opcode = Opcode.PUSHDATA_1)]
        public static void PushData_1(ref ExecutionStack stack)
            => stack.Push(stack.CurrentScript.NextRange(stack.CurrentScript.NextByte()));

        [OpCode(Opcode = Opcode.PUSHDATA_2)]
        public static void PushData_2(ref ExecutionStack stack)
            => stack.Push(stack.CurrentScript.NextRange(BitConverter.ToUInt16(stack.CurrentScript.NextRange(2))));

        [OpCode(Opcode = Opcode.PUSHDATA_4)]
        public static void PushData_4(ref ExecutionStack stack)
            => stack.Push(stack.CurrentScript.NextRange(BitConverter.ToInt32(stack.CurrentScript.NextRange(4))));
        
        
        [OpCode(Opcode = Opcode.PUSH_INT)]
        public static void PushInt(ref ExecutionStack stack)
            => stack.Push(BitConverter.ToInt32(stack.CurrentScript.NextRange(4)));

        [OpCode(Opcode = Opcode.PUSH_UINT)]
        public static void PushUint(ref ExecutionStack stack)
            => stack.Push(BitConverter.ToUInt32(stack.CurrentScript.NextRange(4)));

        [OpCode(Opcode = Opcode.PUSH_SHORT)]
        public static void PushShort(ref ExecutionStack stack)
            => stack.Push(BitConverter.ToInt16(stack.CurrentScript.NextRange(2)));
    }
}