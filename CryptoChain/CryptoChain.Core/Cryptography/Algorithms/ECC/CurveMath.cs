using System;
using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class CurveMath
    {
        private readonly Curve _curve;

        public CurveMath(Curve curve)
            => _curve = curve;
        
        public BigInteger InverseMod(BigInteger k, BigInteger p)
        {
            if (k ==0)
                throw new DivideByZeroException("Can't divide by zero");

            if (k < 0)
                return p - InverseMod(-k, p);

            return Mathematics.ModInverse(k, p);
        }

        public Point Add(Point point1, Point point2)
        {
            _curve.EnsureContains(point1);
            _curve.EnsureContains(point2);

            if (Point.IsInfinity(point1))
                return point2;

            if (Point.IsInfinity(point2))
                return point1;

            var x1 = point1.X;
            var y1 = point1.Y;
            var x2 = point2.X;
            var y2 = point2.Y;
            var p = _curve.P;
            var a = _curve.A;

            BigInteger m;

            if (x1 == x2)
            {
                if (y1 != y2) // point1 + (-point1) = 0
                    return Point.Infinity;;

                // This is the case point1 == point2.
                m = (3 * x1 * x1 + a) * InverseMod(2 * y1, p);
            }
            else // point1 != point2.
            {
                m = (y1 - y2) * InverseMod(x1 - x2, p);
            }

            BigInteger x3 = m * m - x1 - x2;
            BigInteger y3 = y1 + m * (x3 - x1);
            var result = new Point(Modular(x3, p), Modular(-y3, p));

            return result;
        }

        public Point PointNeg(Point point)
        {
            _curve.EnsureContains(point);

            // -0 = 0
            if (Point.IsInfinity(point))
                return point;

            var x = point.X;
            var y = point.Y;
            var p = _curve.P;
            var result = new Point(x, Modular(-y, p));

            _curve.EnsureContains(result);
            return result;
        }
        
        public Point ScalarMult(BigInteger k, Point point)
        {
            _curve.EnsureContains(point);

            var n = _curve.N;

            if (k % n == 0 || Point.IsInfinity(point))
                return Point.Infinity;

            // k * point = -k * (-point)
            if (k < 0)
                return ScalarMult(-k, PointNeg(point));

            Point result = Point.Infinity;
            Point addend = point;

            while (k != 0)
            {
                if ((k & 1) == 1)
                {
                    result = Add(result, addend);
                }

                addend = Add(addend, addend);

                k >>= 1;
            }

            _curve.EnsureContains(result);
            return result;
        }

        public BigInteger Modular(BigInteger k, BigInteger p)
        {
            BigInteger r = k % p;
            return r < 0 ? r + p : r;
        }
    }
}