using System;
using BlockChain.Transactions.Scripting.Enums;

namespace BlockChain.Transactions.Scripting
{
    /// <summary>
    /// Attribute used for implementing an OPCODE (operator)
    /// Use on a static method in the namespace/folder "Operations"
    /// Method will be detected with reflection
    ///
    /// Example usage of operator with no return value:
    /// [OpCode(OPCODE = OPCODE.OP_1)]
    /// public static void OP_1(ref ExecutionStack stack) {}
    /// 
    /// Example usage of operator with return value: (Script will not be stopped if return value is null)
    /// [OpCode(OPCODE = OPCODE.VERIFY)]
    /// public static EXECUTION_RESULT? VERIFY(ref ExecutionStack stack) {}
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class OpCode : Attribute
    {
        public OPCODE OPCODE { get; set; }
        public int minLengthStack { get; set; }
        public int minLengthScript { get; set; }
    }
}