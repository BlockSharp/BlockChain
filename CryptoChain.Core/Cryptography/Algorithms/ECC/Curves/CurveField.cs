using System;
using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    public class CurveField
    {
        public BigInteger Value { get; set; }
        public BigInteger Modulus { get; set; }

        public CurveField(BigInteger value, BigInteger modulus)
        {
            Value = Mathematics.Mod(value,modulus);
            Modulus = modulus;
        }

        public (BigInteger a, BigInteger u, BigInteger v) EEA(BigInteger a, BigInteger b)
        {
            var (s, t, u, v) = (BigInteger.One, BigInteger.Zero, BigInteger.Zero, BigInteger.One);
            while (b != 0)
            {
                var (q, r) = (a / b, a % b);
                var (un, vn) = (s, t);
                s = u - q * s;
                t = v - q * t;
                (a, b) = (b, r);
                (u, v) = (un, vn);
            }

            return (a, u, v);
        }

        public CurveField Inverse()
        {
            if (Value == 0)
                throw new ArgumentException("Cant invert zero");
            var (_, _,v) = EEA(Value, Modulus);
            return new CurveField(v, Modulus);
        }

        public static CurveField operator +(CurveField one, CurveField other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");
            return new CurveField(one.Value + other.Value, one.Modulus);
        }
        
        public static CurveField operator +(CurveField one, BigInteger other)
            => new (one.Value + other, one.Modulus);
        
        public static CurveField operator +(BigInteger one, CurveField other)
            => other + one;
        
        public static CurveField operator -(CurveField one, CurveField other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");
            return new CurveField(one.Value - other.Value, one.Modulus);
        }

        public static CurveField operator -(CurveField one, BigInteger other)
            => new (one.Value - other, one.Modulus);

        public static CurveField operator -(BigInteger one, CurveField other)
            => -other + one;
        
        public static CurveField operator *(CurveField one, CurveField other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");
            return new CurveField(one.Value * other.Value, one.Modulus);
        }
        
        public static CurveField operator *(CurveField one, BigInteger other)
            => new (one.Value * other, one.Modulus);

        public static CurveField operator *(BigInteger one, CurveField other)
            => other * one;

        public static CurveField operator /(CurveField one, CurveField other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");

            var x1 = new CurveField(other.Value, one.Modulus);
            return one * x1.Inverse();
        }

        public static CurveField operator /(BigInteger one, CurveField other)
            => other.Inverse() * one;
        
        public static CurveField operator /(CurveField one, BigInteger other)
            => one * new CurveField(other, one.Modulus).Inverse();

        public static CurveField operator -(CurveField self)
            => new (-self.Value, self.Modulus);

        public static bool operator ==(CurveField one, CurveField other)
            => one.Value == other.Value;

        public static bool operator !=(CurveField one, CurveField other) 
            => !(one == other);

        public static bool operator ==(BigInteger one, CurveField other)
            => one == other.Value;

        public static bool operator !=(BigInteger one, CurveField other) 
            => !(one == other);

        public static bool operator ==(CurveField one, BigInteger other)
            => one.Value == other;

        public static bool operator !=(CurveField one, BigInteger other) 
            => !(one == other);

        public CurveField Pow(BigInteger exponent)
            => new (BigInteger.ModPow(Value, exponent, Modulus), Modulus);

        public BigInteger New(BigInteger modulus)
            => new CurveField(Value, modulus).Value;

        public override string ToString()
        {
            return "0x" + Value.ToString("x2");
        }
    }
}