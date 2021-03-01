namespace CryptoChain.Core.Transactions.Scripting
{
    /// <summary>
    /// Enum with all the OPCODES (operators)
    /// </summary>
    public enum Opcode : byte
    {
        FALSE = 0, TRUE = 1,
        OP_1 = 2, OP_2, OP_3, OP_4, OP_5, OP_6, OP_7, OP_8, OP_9, OP_10, OP_11, OP_12, OP_13, OP_14, OP_15, OP_16 = 17,
        PUSH_DATA_1 = 18, PUSH_DATA_2 = 19, PUSH_DATA_4 = 20, 
        PUSH_INT = 21, PUSH_UINT = 22, PUSH_SHORT = 23,
        //24-29
        PUSH_TIMESTAMP = 30, PUSH_BLOCK_HEIGHT = 31,
        //32-49
        ADD = 50, SUBTRACT, MULTIPLY, DIVIDE, MODULO, ABS, NEGATE,
        LESS_THAN, GREATER_THAN, LESS_OR_EQ, GREATER_OR_EQ, MIN, MAX = 62,
        EQ_NUM = 63, EQ_VERIFY_NUM, OP_AND, OP_OR, OP_NOT = 67,
        //68-78
        EVAL_SCRIPT = 79,
        SHA256 = 80, HASH256, HASH160, RIPEMD160 = 83,
        //84-90
        
        //91-99
        CHECK_LOCKTIME = 100,
        CHECK_LOCKTIME_VERIFY,
        CHECK_SIG,
        CHECK_SIG_VERIFY,
        CHECK_MULTI_SIG = 104,
        //105-109
        EQUAL = 110,
        EQ_VERIFY,
        VERIFY = 112,
        //113-119
        DUP = 120, DUP2 = 121, CONCAT = 122,
        //123 - 129
        ALG_RSA = 130, 
        //Other algorithms: 131-139
        //ECC algorithms: 140 - 145
        ALG_ECDSA = 140,
        //146-200
        SET_VAR = 201, GET_VAR = 202, 
        /*IF, FI, ELSE, ELSE_IF, IF_VAR, ELSE_IF_VAR, WHILE, LOOP, BREAK,
        FOR, END,*/
        //-253
        RETURN = 254, EXIT = 255
    }
}