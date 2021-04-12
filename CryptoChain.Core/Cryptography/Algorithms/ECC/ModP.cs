using System;
using System.Diagnostics;
using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class ModP
    {
        private bool Equals(ModP other)
        {
            return Value.Equals(other.Value) && Modulus.Equals(other.Modulus);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ModP) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Modulus);
        }

        public BigInteger Value { get; set; }
        public BigInteger Modulus { get; set; }
        private bool? _qnr;

        /// <summary>
        /// Indicates if value is quadratic non-residue mod p
        /// </summary>
        public bool IsQnr
            => _qnr ??= Pow((Modulus - 1) / 2) != 1;

        public ModP(BigInteger value, BigInteger modulus)
        {
            Value = Mathematics.Mod(value,modulus);
            Modulus = modulus;
        }

        public static ModP Empty
            => new (0,0);
        

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

        public ModP Inverse()
        {
            if (Value == 0)
                throw new ArgumentException("Cant invert zero");
            var (_, _,v) = EEA(Value, Modulus);
            return new ModP(v, Modulus);
        }

        public static ModP operator +(ModP one, ModP other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");
            return new ModP(one.Value + other.Value, one.Modulus);
        }
        
        public static ModP operator +(ModP one, BigInteger other)
            => new (one.Value + other, one.Modulus);
        
        public static ModP operator +(BigInteger one, ModP other)
            => other + one;
        
        public static ModP operator -(ModP one, ModP other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");
            return new ModP(one.Value - other.Value, one.Modulus);
        }

        public static ModP operator -(ModP one, BigInteger other)
            => new (one.Value - other, one.Modulus);

        public static ModP operator -(BigInteger one, ModP other)
            => -other + one;
        
        public static ModP operator *(ModP one, ModP other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");
            return new ModP(one.Value * other.Value, one.Modulus);
        }
        
        public static ModP operator *(ModP one, BigInteger other)
            => new (one.Value * other, one.Modulus);

        public static ModP operator *(BigInteger one, ModP other)
            => other * one;

        public static ModP operator /(ModP one, ModP other)
        {
            if (one.Modulus != other.Modulus)
                throw new ArgumentException("Cant perform operations on curve fields with different modulus");

            var x1 = new ModP(other.Value, one.Modulus);
            return one * x1.Inverse();
        }

        public static ModP operator /(BigInteger one, ModP other)
            => other.Inverse() * one;
        
        public static ModP operator /(ModP one, BigInteger other)
            => one * new ModP(other, one.Modulus).Inverse();

        public static ModP operator -(ModP self)
            => new (-self.Value, self.Modulus);

        public static bool operator ==(ModP one, ModP other)
            => one.Value == other.Value;

        public static bool operator !=(ModP one, ModP other) 
            => !(one == other);

        public static bool operator ==(BigInteger one, ModP other)
            => one == other.Value;

        public static bool operator !=(BigInteger one, ModP other) 
            => !(one == other);

        public static bool operator ==(ModP one, BigInteger other)
            => one.Value == other;

        public static bool operator !=(ModP one, BigInteger other) 
            => !(one == other);

        public ModP Pow(BigInteger exponent)
            => new (BigInteger.ModPow(Value, exponent, Modulus), Modulus);

        public BigInteger New(BigInteger modulus)
            => new ModP(Value, modulus).Value;
        
        public BigInteger New()
            => new ModP(Value, Modulus).Value;

        private ModP TonelliShanksSqrt()
        {
            var random = RandomGenerator.Secure;
            var q = Modulus - 1;
            var s = BigInteger.Zero;
            while (q % 2 == 0)
            {
                s += 1;
                q >>= 1;
            }
            
            Debug.Assert(q * BigInteger.Pow(2, (int)s) == Modulus - 1);
            
            ModP z;
            while (true)
            {
                z = new ModP(random.RandomInRange(1, Modulus - 1), Modulus);
                if(z.IsQnr)
                    break;
            }

            var c = z.Pow(q);
            var r = Pow((q + 1) / 2);
            var t = Pow(q);
            var m = s;
            while (t.Value != 1)
            {
                int i;
                for (i = 1; i <= m; i++)
                {
                    if(t.Pow((1 << i)) == 1)
                        break;
                }

                var b = c.Pow(BigInteger.One << (int)(m - i - 1));
                r = r * b;
                t = t * b.Pow(2);
                c = b.Pow(2);
                m = i;
            }

            return r;
        }
        
        public (ModP, ModP) Sqrt()
        {
            if (IsQnr)
                throw new ArgumentException("Cant do Square root on QNR");
            
            var root = (Modulus % 4 == 3)
                ? Pow((Modulus + 1) / 4)
                : TonelliShanksSqrt();

            if ((root.Value & 1) == 0)
                return(root, -root);

            return(-root, root);
        }

        public override string ToString()
        {
            return "0x" + Value.ToString("x2");
        }
    }
}