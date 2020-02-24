namespace BlockChain.Transactions.Scripting.Enums
{
    /// <summary>
    /// Enum with all the OPCODES of a script (operators)
    /// </summary>
    public enum OPCODE
    {
        FALSE, TRUE,
        OP_1, OP_2, OP_3, OP_4, OP_5, OP_6, OP_7, OP_8, OP_9, OP_10, OP_11, OP_12, OP_13, OP_14, OP_15, OP_16,
        PUSHDATA_1, PUSHDATA_2, PUSHDATA_4, 
        PUSH_INT, PUSH_UINT, PUSH_SHORT,

        ADD, SUBSTRACT, MULTIPLY, DIVIDE, MODULO, ABS, NEGATE,
        LESSTHAN, GREATERTHAN, LESSOREQUAL, GREATEROREQUAL, MIN, MAX,
        EQ_NUM, EQ_VERIFY_NUM, OP_AND, OP_OR, OP_NOT,

        EVAL_SCRIPT,

        SHA256, HASH256, HASH160, RIPEMD160,
        SEPERATOR,

        CHECKLOCKTIME,
        CHECKLOCKTIME_VERIFY,
        CHECKSIG,
        CHECKSIG_VERIFY,
        EQUALS,
        EQ_VERIFY,
        VERIFY,

        DUP,
        DUP2,
        PUBKEY,
        PUBKEY_HASH,
        SIGNATURE,

        DO_NOTHING,
        RETURN
    }
}