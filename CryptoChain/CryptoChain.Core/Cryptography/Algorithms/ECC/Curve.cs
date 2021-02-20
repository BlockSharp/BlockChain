using System;
using System.Globalization;
using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class Curve
    {
        public BigInteger P { get; } //Prime
        public BigInteger A { get; } //A
        public BigInteger B { get; } //B
        public Point G { get; } //Generator point G
        public BigInteger N { get; } //Order N
        public short H { get; } //useless?
        
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
        public static Curve Secp251K1 => new(
            BigInteger.Parse("00fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f", NumberStyles.HexNumber),
            new BigInteger(0), new BigInteger(7),
            new Point(BigInteger.Parse("79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", NumberStyles.HexNumber),
                BigInteger.Parse("483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", NumberStyles.HexNumber)),
            BigInteger.Parse("00fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141", NumberStyles.HexNumber),
            1, 256
        );
        
        public static Curve Secp251R1 => new(
            BigInteger.Parse("00FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF", NumberStyles.HexNumber),
            BigInteger.Parse("00FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFC", NumberStyles.HexNumber),
            BigInteger.Parse("005AC635D8AA3A93E7B3EBBD55769886BC651D06B0CC53B0F63BCE3C3E27D2604B", NumberStyles.HexNumber),
            new Point(BigInteger.Parse("006B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", NumberStyles.HexNumber),
                BigInteger.Parse("004FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", NumberStyles.HexNumber)),
            BigInteger.Parse("00FFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551", NumberStyles.HexNumber),
            1, 256
        );
        
    }
}