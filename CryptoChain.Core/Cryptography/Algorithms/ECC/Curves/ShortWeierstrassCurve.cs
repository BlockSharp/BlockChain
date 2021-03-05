using System;
using System.Numerics;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    public class ShortWeierstrassCurve : CurveBase
    {
        /// <summary>
        /// Coefficient A of the curve equation y^2 = x^3 + Ax + B.
        /// </summary>
        public BigInteger A { get; set; }
        
        /// <summary>
        /// Coefficient B of the curve equation y^2 = x^3 + Ax + B.
        /// </summary>
        public BigInteger B { get; set; }

        public override bool Contains(Point point)
            => IsInfinity(point) || (point.Y * point.Y - point.X * point.X * point.X - A * point.X - B) % P == 0;

        public override Point Add(Point p, Point q)
        {
            //P is at infinity, O + Q = Q
            if (IsInfinity(p))
                return q;

            //Q is at infinity, P + O = P
            if (IsInfinity(q))
                return p;
            
            //P == -Q, return O (Infinity point)
            if(p.Equals(Negate(q))) //TODO: check
                return InfinityPoint;
            
            //Convert points to fields
            var px = new CurveField(p.X, P);
            var py = new CurveField(p.Y, P);
            
            //P == Q, double point
            if (p.Equals(q))
            {
                var s = ((3 * px.Pow(2)) + A) / (2 * py);
                var newX = s * s - (2 * px);
                var newY = s * (px - newX) - py;
                return new Point(newX.New(P), newY.New(P));
            }
            
            var qx = new CurveField(q.X, P);
            var qy = new CurveField(q.Y, P);
            
            // TODO
            //P != Q, point addition
            var s1 = (py - qy) / (px - qx);
            var x = s1.Pow(2) - px - qx;
            var y = s1 * (px - x) - py;
            return new Point(x.New(P), y.New(P));
        }

        public override Point Negate(Point p)
            => new (Mathematics.Mod(p.X, P), Mathematics.Mod(-p.Y, P));

        public override Point InfinityPoint => new (null, null);

        public override bool IsInfinity(Point p)
            => p.Equals(InfinityPoint);
    }
}