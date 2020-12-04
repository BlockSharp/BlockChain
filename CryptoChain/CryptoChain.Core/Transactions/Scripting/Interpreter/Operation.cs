using System.Reflection;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter
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
        private delegate ExecutionResult? ReturnDelegate(ref ExecutionStack stack);

        private readonly bool _hasReturnType;
        private readonly System.Delegate _operation;
        private readonly OpCode _opCode;

        public Operation(MethodInfo methodInfo, OpCode opCode)
        {
            _hasReturnType = methodInfo.ReturnType == typeof(ExecutionResult?);
            _opCode = opCode;
            _operation = System.Delegate.CreateDelegate(_hasReturnType ? typeof(ReturnDelegate) : typeof(Delegate),
                methodInfo);
        }

        /// <summary>
        /// Execute this operation
        /// </summary>
        /// <param name="stack">Current execution stack</param>
        /// <returns>null or an EXECUTION_RESULT if method has a return type and didn't return null</returns>
        public ExecutionResult? Execute(ref ExecutionStack stack)
        {
            if (_opCode.MinLengthStack != 0 && _opCode.MinLengthStack > stack.Count) return ExecutionResult.INVALID_STACK;
            if (_opCode.MinLengthScript != 0 && _opCode.MinLengthScript > stack.CurrentScript.Length) return ExecutionResult.INVALID_SCRIPT;
            
            if (_hasReturnType) return (_operation as ReturnDelegate)?.Invoke(ref stack);
            (_operation as Delegate)?.Invoke(ref stack);
            return null;
        }
    }
}