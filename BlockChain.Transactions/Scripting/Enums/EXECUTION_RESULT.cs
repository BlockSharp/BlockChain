namespace BlockChain.Transactions.Scripting.Enums
{
    /// <summary>
    /// Enum with all the possible results of a script
    /// </summary>
    public enum EXECUTION_RESULT
    {
        SUCCESS,
        VERIFIED,
        FAILURE,
        
        UNKNOWN_ERROR,
        INVALID_STACK,
        OP_NOT_IN_USE,
        MORE_ITEMS_ON_BOTTOM,
        NO_TRANSACTION_GIVEN,
        MATH_ERROR, 
        DISABLED_CODE,
        EXECUTION_STOPPPED,
        LOCKTIME_ERROR,
        INVALID_BYTE_SIZE
    }
}