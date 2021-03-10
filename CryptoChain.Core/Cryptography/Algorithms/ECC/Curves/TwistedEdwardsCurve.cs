using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    public class TwistedEdwardsCurve : Curve
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
        {
            var a = new CurveField(A, P);
            var d = new CurveField(D, P);
            var px = new CurveField(point.X, P);
            var py = new CurveField(point.Y, P);
            return (a * px.Pow(2)) + py.Pow(2)
                   == 1 + d * px.Pow(2) * py.Pow(2);
        }

        
        public override Point Add(Point p, Point q)
        {
            var a = new CurveField(A, P);
            var d = new CurveField(D, P);
            var px = new CurveField(p.X, P);
            var py = new CurveField(p.Y, P);
            var qx = new CurveField(q.X, P);
            var qy = new CurveField(q.Y, P);
            
            var x = (px * qy + qx * py) / (1 + d * px * qx * py * qy);
            var y = (py * qy - a * px * qx) / (1 - d * px * qx * py * qy);
            return new Point(x.New(P), y.New(P));
        }

        public override Point Negate(Point p)
            => new (Mathematics.Mod(-p.X, P), Mathematics.Mod(p.Y, P));
        
        public override byte[] Compress(Point p)
            => throw new System.NotImplementedException();

        public override Point Decompress(byte[] compressed)
            => throw new System.NotImplementedException();
    }
}