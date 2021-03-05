using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    public class TwistedEdwardsCurve : CurveBase
    {
        /// <summary>
        /// Coefficient A of the curve equation Ax^2 + y^2 = 1 + Dx^2 y^2.
        /// </summary>
        public BigInteger A { get; set; }
        
        /// <summary>
        /// Coefficient D of the curve equation Ax^2 + y^2 = 1 + Dx^2 y^2.
        /// </summary>
        public BigInteger D { get; set; }

        public override Point InfinityPoint => new(0, 1);
        
        public override bool IsInfinity(Point p)
            => p.Equals(InfinityPoint);

        public override bool Contains(Point point)
            => A * BigInteger.Pow(point.X, 2) + BigInteger.Pow(point.Y, 2)
               == 1 + D * BigInteger.Pow(point.X, 2) * BigInteger.Pow(point.Y, 2);

        
        public override Point Add(Point p, Point q)
        {
            var x = (p.X * q.Y + q.X * p.Y) / (1 + D * p.X * q.X * p.Y * q.Y);
            var y = (p.Y * q.Y - A * p.X + q.X) / (1 - D * p.X * q.X * p.Y * q.Y);
            return new Point(x, y);
        }

        public override Point Negate(Point p)
            => new (Mathematics.Mod(-p.X, P), Mathematics.Mod(p.Y, P));
    }
}