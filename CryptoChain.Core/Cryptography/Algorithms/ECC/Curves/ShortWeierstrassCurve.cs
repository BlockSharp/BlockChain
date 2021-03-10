using System;
using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    public class ShortWeierstrassCurve : Curve
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
            
            /*
            //Convert points to fields
            var px = new CurveField(p.X, P);
            var py = new CurveField(p.Y, P);
            
            //P == Q, double point
            if (p.Equals(q))
            {
                var s = (3 * px.Pow(2) + A) / (2 * py);
                var newX = s * s - 2 * px;
                var newY = s * (px - newX) - py;
                return new Point(newX.New(P), newY.New(P));
            }
            
            var qx = new CurveField(q.X, P);
            var qy = new CurveField(q.Y, P);
            
            //P != Q, point addition
            var s1 = (py - qy) / (px - qx);
            var x = s1.Pow(2) - px - qx;
            var y = s1 * (px - x) - py;
            return new Point(x.New(P), y.New(P));*/
            
            BigInteger m;

            if (p.X == q.X)
            {
                if (p.Y != q.Y) // point1 + (-point1) = 0
                    return Point.Infinity;;

                // This is the case point1 == point2.
                m = (3 * p.X * p.X + A) * Mathematics.ModInverse(2 * p.Y, P);
            }
            else // point1 != point2.
            {
                m = (p.Y - q.Y) * Mathematics.ModInverse(p.X - q.X, P);
            }

            BigInteger x3 = m * m - p.X - q.X;
            BigInteger y3 = p.Y + m * (x3 - p.X);
            var result = new Point(Mathematics.Mod(x3, P), Mathematics.Mod(-y3, P));

            return result;
        }

        public override Point Negate(Point p)
            => IsInfinity(p) ? p : new (Mathematics.Mod(p.X, P), Mathematics.Mod(-p.Y, P));

        public override Point InfinityPoint => new (null, null);

        public override bool IsInfinity(Point p)
            => p.Equals(InfinityPoint);

        public override byte[] Compress(Point p)
        {
            var x = p.X.ToByteArray();
            byte[] buffer = new byte[x.Length + 1];
            x.CopyTo(buffer, 1);
            buffer[0] = p.Y.IsEven ? (byte)0x02 : (byte)0x03;
            return buffer;
        }

        public override Point Decompress(byte[] compressed)
        {
            byte sig = compressed[0];
            if (sig != 0x02 && sig != 0x03)
                throw new ArgumentException("Key is not compressed");
            
            var x = new BigInteger(compressed[1..]);

            var y2 = (BigInteger.ModPow(x, 3, P)
                      + A * x + B) % P;
            
            var y3 = BigInteger.ModPow(y2, (P + 1) / 4, P);
            var y = (y3 % 2 != sig - 2) ? P - y3 : y3;

            return new Point(x, y);
        }
    }
}