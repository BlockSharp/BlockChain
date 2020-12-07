namespace CryptoChain.Core.Transactions.Scripting
{
    /// <summary>
    /// Enum with all the possible results of a script
    /// </summary>
    public enum ExecutionResult : byte
    {
        FAILURE = 0,
        SUCCESS = 1,
        
        INVALID_STACK,
        INVALID_SCRIPT,
        
        MORE_ITEMS_ON_BOTTOM,
        NO_TRANSACTION_GIVEN,
        DISABLED_CODE,
        UNKNOWN_CODE,
        EXECUTION_STOPPED,
        INVALID_BYTE_SIZE,
        UNKNOWN_ERROR
    }
}