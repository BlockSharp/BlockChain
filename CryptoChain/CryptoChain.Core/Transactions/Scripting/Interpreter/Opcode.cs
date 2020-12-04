using System;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter
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
    /// public static ExecutionResult? Verify(ref ExecutionStack stack) {}
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class OpCode : Attribute
    {
        public Opcode Opcode { get; set; }
        public int MinLengthStack { get; set; }
        public int MinLengthScript { get; set; }
    }
}