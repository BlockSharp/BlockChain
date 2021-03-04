namespace CryptoChain.Core.Cryptography.Algorithms
{
    public enum Algorithm : byte
    {
        //Equals OPCODE 130 + x
        RSA = 0,
        //1-9 = other
        //10-15 = ECC
        ECDSA = 10,
    }
}