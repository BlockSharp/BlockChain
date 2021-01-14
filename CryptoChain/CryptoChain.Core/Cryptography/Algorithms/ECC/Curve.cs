using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public static class Curves
    {
        /// <summary>
        /// Curve sepc256k1, also used by Bitcoin
        /// </summary>
        public static Curve SEPC256K1
        {
            get
            {
                return new()
                {
                    A = MathUtils.FromHex("0000000000000000000000000000000000000000000000000000000000000000"),
                    B = MathUtils.FromHex("0000000000000000000000000000000000000000000000000000000000000007"),
                    P = MathUtils.FromHex("fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f"),
                    N = MathUtils.FromHex("fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141"),
                    G = new Point(MathUtils.FromHex("79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798"),
                        MathUtils.FromHex("483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8")),
                    Oid = new[] {1, 3, 132, 0, 10}
                };
            }
        }
    }
    
    public class Curve
    {
        public BigInteger A { get; set; }
        public BigInteger B { get; set; }
        public BigInteger P { get; set; }
        public BigInteger N { get; set; }
        public Point G { get; set; }
        public int[] Oid { get; set; }

        public bool Contains(Point p)
            => BigInteger.Pow(p.Y, 2) - (BigInteger.Pow(p.X, 3) + A * p.X + B) % P == BigInteger.Zero;
        
        
    }
}