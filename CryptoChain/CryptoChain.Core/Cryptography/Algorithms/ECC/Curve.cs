using System;
using System.Globalization;
using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class Curve
    {
        public BigInteger P { get; }
        public BigInteger A { get; }
        public BigInteger B { get; }
        public Point G { get; }
        public BigInteger N { get; }
        public short H { get; }
        
        /// <summary>
        /// Length in bits
        /// </summary>
        public uint LengthInBits { get; }
        
        public Curve(BigInteger p, BigInteger a, BigInteger b, Point g, BigInteger n, short h, uint length)
        {
            P = p;
            A = a;
            B = b;
            G = g;
            N = n;
            H = h;
            LengthInBits = length;
        }

        /// <summary>
        /// Checks if point is on curve
        /// </summary>
        /// <param name="point">The point to be checked</param>
        /// <returns>true if point is on curve. If point is null it will return false</returns>
        public bool Contains(Point? point)
        {
            if (point == Point.Infinity)
                return true;
            
            if (point == null)
                return false;

            var rem = (point.Y * point.Y - point.X * point.X * point.X - A * point.X - B) % P;
            return rem == 0;
        }
        
        public void EnsureContains(Point? point)
        {
            if(!Contains(point))
                throw new ArgumentException($"Point does not exist on curve");
        }

        //Hardcoded curves
        public static Curve Sepc251K1 => new(
            BigInteger.Parse("00fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f", NumberStyles.HexNumber),
            new BigInteger(0), new BigInteger(7),
            new Point(BigInteger.Parse("79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", NumberStyles.HexNumber),
                BigInteger.Parse("483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", NumberStyles.HexNumber)),
            BigInteger.Parse("00fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141", NumberStyles.HexNumber),
            1, 256
        );
    }
}