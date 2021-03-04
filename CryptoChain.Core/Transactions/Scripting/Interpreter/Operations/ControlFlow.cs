namespace CryptoChain.Core.Transactions.Scripting.Interpreter.Operations
{
    internal static class ControlFlow
    {
        //For if, else etc you can use a tree
        //Maybe a control flow tree?
        
        public static ExecutionResult? Disabled()
        {
            return ExecutionResult.DISABLED_CODE;
        }
    }
}