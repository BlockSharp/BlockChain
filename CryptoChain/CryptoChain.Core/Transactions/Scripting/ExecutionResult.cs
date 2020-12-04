namespace CryptoChain.Core.Transactions.Scripting
{
    /// <summary>
    /// Enum with all the possible results of a script
    /// </summary>
    public enum ExecutionResult : byte
    {
        FAILURE = 0,
        SUCCESS = 1,
        VERIFIED = 2,
        
        INVALID_STACK,
        INVALID_SCRIPT,
        
        OP_NOT_IN_USE,
        MORE_ITEMS_ON_BOTTOM,
        NO_TRANSACTION_GIVEN,
        MATH_ERROR, 
        DISABLED_CODE,
        EXECUTION_STOPPPED,
        LOCKTIME_ERROR,
        INVALID_BYTE_SIZE,
        UNKNOWN_ERROR
    }
}