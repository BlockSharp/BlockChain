using System;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    [Flags]
    public enum CurveFlags : byte
    {
        NONE = 0,
        NEED_SET_MSB = 1, //Most Significant Bit
        NEED_SET_PSG = 2, //Prime order sub group
        //4, 8, 16, 32, 64, 128 are free
    }
}