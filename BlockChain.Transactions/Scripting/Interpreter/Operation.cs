using System.Linq;
using System.Reflection;
using BlockChain.Transactions.Scripting.Enums;

namespace BlockChain.Transactions.Scripting
{
    /// <summary>
    /// Wrapper class around delegate of an operation
    /// This because an operation can have different types of delegates
    /// </summary>
    internal class Operation
    {
        /// <summary>
        /// Different types of functions that are valid for an operation.
        /// </summary>
        private delegate void Delegate(ref ExecutionStack stack);
        private delegate EXECUTION_RESULT? ReturnDelegate(ref ExecutionStack stack);

        private readonly bool _hasReturnType;
        private readonly System.Delegate _operation;
        private readonly OpCode _opCode;

        public Operation(MethodInfo methodInfo, OpCode opCode)
        {
            _hasReturnType = methodInfo.ReturnType == typeof(EXECUTION_RESULT?);
            _opCode = opCode;
            _operation = System.Delegate.CreateDelegate(_hasReturnType ? typeof(ReturnDelegate) : typeof(Delegate),
                methodInfo);
        }

        /// <summary>
        /// Execute this operation
        /// </summary>
        /// <param name="stack">Current execution stack</param>
        /// <returns>null or an EXECUTION_RESULT if method has a return type and didn't return null</returns>
        public EXECUTION_RESULT? Execute(ref ExecutionStack stack)
        {
            if (_opCode.minLengthStack != 0 && _opCode.minLengthStack > stack.Count) return EXECUTION_RESULT.INVALID_STACK;

            if (_hasReturnType) return (_operation as ReturnDelegate)?.Invoke(ref stack);
            (_operation as Delegate)?.Invoke(ref stack);
            return null;
        }
    }
}