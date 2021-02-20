using System;
using System.Numerics;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class Point : ISerializable
    {
        public BigInteger X { get; }
        public BigInteger Y { get; }

        public static Point? Infinity => null;

        public static bool IsInfinity(Point? point)
            => point == Infinity;
        
        public Point(BigInteger x, BigInteger y)
        {
            X = x;
            Y = y;
        }

        public byte[] Compress()
        {
            throw new NotImplementedException();
        }

        public Point Decompress(byte[] compressed)
        {
            throw new NotImplementedException();
        }

        public int Length { get; }
        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}