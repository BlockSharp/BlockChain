using System;
using System.Numerics;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class Point : ISerializable
    {
        protected BigInteger? _x;
        protected BigInteger? _y;

        public BigInteger X => _x ?? throw new ArgumentException("Point does not contain X");
        public BigInteger Y => _y ?? throw new ArgumentException("Point does not contain Y");

        //Delete these when finished with new curve system
        public static Point Infinity => new (null, null);
        public bool IsInfinity => Equals(Infinity);
        
        public Point(BigInteger? x, BigInteger? y)
        {
            _x = x;
            _y = y;
        }

        public Point(byte[] serialized)
        {
            int xlen = BitConverter.ToInt32(serialized);
            _x = new BigInteger(serialized[4..(4 + xlen)]);
            _y = new BigInteger(serialized[(4 + xlen)..]);
        }

        public int Length => 4 + X.GetByteCount() + Y.GetByteCount();
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            var x = X.ToByteArray();
            var y = Y.ToByteArray();
            Buffer.BlockCopy(BitConverter.GetBytes(x.Length), 0, buffer, 0, 4);
            x.CopyTo(buffer, 4);
            y.CopyTo(buffer, 4 + x.Length);
            return buffer;
        }

        public bool Equals(Point other)
            => _x.Equals(other._x) && _y.Equals(other._y);

        public override string ToString() => $"X: 0x{_x?.ToString("x2")}, Y: 0x{_y?.ToString("x2")}";
    }
}