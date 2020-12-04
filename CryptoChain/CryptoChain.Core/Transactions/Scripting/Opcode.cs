namespace CryptoChain.Core.Transactions.Scripting
{
    /// <summary>
    /// Enum with all the OPCODES (operators)
    /// </summary>
    public enum Opcode : byte
    {
        FALSE = 0, TRUE = 1,
        OP_1 = 2, OP_2, OP_3, OP_4, OP_5, OP_6, OP_7, OP_8, OP_9, OP_10, OP_11, OP_12, OP_13, OP_14, OP_15, OP_16 = 17,
        PUSHDATA_1 = 18, PUSHDATA_2, PUSHDATA_4 = 20, 
        PUSH_INT = 21, PUSH_UINT, PUSH_SHORT = 23,

        ADD = 50, SUBTRACT, MULTIPLY, DIVIDE, MODULO, ABS, NEGATE,
        LESSTHAN, GREATERTHAN, LESSOREQUAL, GREATEROREQUAL, MIN, MAX = 62,
        EQ_NUM = 63, EQ_VERIFY_NUM, OP_AND, OP_OR, OP_NOT = 67,

        EVAL_SCRIPT = 79,

        SHA256 = 80, HASH256, HASH160, RIPEMD160 = 83,
        
        SEPERATOR = 90,

        CHECKLOCKTIME = 100,
        CHECKLOCKTIME_VERIFY,
        CHECKSIG,
        CHECKSIG_VERIFY,
        EQUALS,
        EQ_VERIFY,
        VERIFY = 106,

        DUP = 110, DUP2,

        RETURN
    }
}