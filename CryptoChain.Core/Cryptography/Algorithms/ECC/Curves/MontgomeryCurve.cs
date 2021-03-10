using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    public class MontgomeryCurve : Curve
    {
        /// <summary>
        /// Coefficient A of the curve equation By^2 = x^3 + Ax^2 + x
        /// </summary>
        public BigInteger A { get; set; }
        
        /// <summary>
        /// Coefficient B of the curve equation By^2 = x^3 + Ax^2 + x
        /// </summary>
        public BigInteger B { get; set; }

        public override bool Contains(Point point)
            => IsInfinity(point) || B * BigInteger.Pow(point.Y, 2)
                == BigInteger.Pow(point.X, 2) + A * BigInteger.Pow(point.X, 2) + point.X;

        public override Point Add(Point p, Point q)
        {
            //P is at infinity, O + Q = Q
            if (IsInfinity(p))
                return q;
            
            //P == -Q, return O (Infinity point)
            if(p.Equals(Negate(q)))
                return InfinityPoint;
            
            //P == Q, double point
            if (p.Equals(q))
            {
                var x = -2 * p.X - A +
                           (3 * BigInteger.Pow(p.X, 2) + 2 * p.X * A + 1) / (4 * BigInteger.Pow(p.Y, 2) * B);
                var y = Negate(p).Y + (3 * BigInteger.Pow(p.X, 2) + 2 * p.X * A + 1) * (3 * p.X + A)
                    / (2 * p.Y * B) - BigInteger.Pow(3 * BigInteger.Pow(p.X, 2) + 2 * p.X * A + 1, 3) / 8 *
                    BigInteger.Pow(p.Y, 3) * BigInteger.Pow(B, 2);
                return new Point(x, y);
            }
            
            //else, P != Q, add point
            var newX = Negate(p).X - q.X - A + BigInteger.Pow(p.Y - q.Y, 2) * B / BigInteger.Pow(p.X - q.Y, 2);
            var newY = (2 * p.X + q.X + A) * (p.Y - q.Y) / (p.X - q.X) - p.Y -
                       BigInteger.Pow(p.Y - q.Y, 3) * B / BigInteger.Pow(p.X - q.X, 3);
            return new Point(newX, newY);
        }

        public override Point Negate(Point p)
            => new (Mathematics.Mod(p.X, P), Mathematics.Mod(-p.Y, P));

        public override Point InfinityPoint => new (null, null);
        public override bool IsInfinity(Point p)
            => p.Equals(InfinityPoint);

        public override byte[] Compress(Point p)
            => throw new System.NotImplementedException();

        public override Point Decompress(byte[] compressed)
            => throw new System.NotImplementedException();
    }
}