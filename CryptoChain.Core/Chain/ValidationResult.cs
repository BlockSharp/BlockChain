namespace CryptoChain.Core.Chain
{
    public enum ValidationResult : byte
    {
        FAILED = 0, SUCCESS = 1, 
        TX_IS_COINBASE, TX_LOCK_TIME_ERROR, TX_SCRIPT_FAILURE, TX_UNBALANCED, TX_NO_INPUTS, TX_NO_OUTPUTS,
        TX_WRONG_DATA, TX_WRONG_REFERENCE
    }
}