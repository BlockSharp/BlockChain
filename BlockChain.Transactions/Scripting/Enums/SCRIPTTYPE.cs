namespace BlockChain.Transactions.Scripting.Enums
{
    /// <summary>
    /// Enum with all script types
    /// </summary>
    public enum SCRIPTTYPE
    {
        LOCK_P2PK,
        LOCK_P2PKH,
        LOCK_P2SH,
        UNLOCK_P2PK,
        UNLOCK_P2PKH,
        UNLOCK_P2SH,
        UNKNOWN
    }
}