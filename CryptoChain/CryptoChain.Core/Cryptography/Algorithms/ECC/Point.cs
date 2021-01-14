using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class Point
    {
        public BigInteger X { get; }
        public BigInteger Y { get; }
        public BigInteger Z { get; }

        public Point(BigInteger x, BigInteger y, BigInteger? z = null)
        {
            X = x;
            Y = y;
            Z = z ?? BigInteger.Zero;
        }
        
        /// <summary>
        /// Convert point to Jacobian coordinates
        /// </summary>
        /// <returns>point in Jacobian coordinates</returns>
        public Point ToJacobian()
            => new Point(X, Y, 1);

        /// <summary>
        /// Create point from Jacobian coordinates
        /// </summary>
        /// <param name="point">The point you want to convert</param>
        /// <param name="p">Prime number in the module of the equation Y^2 = X ^ 3 + A * X + B(mod p)</param>
        public Point(Point point, BigInteger p)
        {
            BigInteger z = Mathematics.ModInverse(point.Z, p);
            X = point.X * BigInteger.Pow(z, 2) % p;
            Y = point.Y * BigInteger.Pow(z, 3) % p;
        }
        

        /// <summary>
        /// Fast way to multily point and scalar in elliptic curves
        /// </summary>
        /// <param name="scalar">Scalar to mutiply</param>
        /// <param name="order">Order of the elliptic curve</param>
        /// <param name="prime">Prime number in the module of the equation Y^2 = X ^ 3 + A * X + B(mod p)</param>
        /// <param name="a">Coefficient of the first-order term of the equation Y ^ 2 = X ^ 3 + A * X + B(mod p)</param>
        /// <returns>Multiplied point</returns>
        public Point Multiply(BigInteger scalar, BigInteger order, BigInteger prime, BigInteger a)
            => new Point(Mathematics.MultiplyJacobian(this.ToJacobian(), scalar, order, a, prime), prime);

        /// <summary>
        /// Fast way to add two points in elliptic curves
        /// </summary>
        /// <param name="p">First Point you want to add</param>
        /// <param name="q">Second Point you want to add</param>
        /// <param name="a">Coefficient of the first-order term of the equation Y ^ 2 = X ^ 3 + A * X + B(mod p)</param>
        /// <param name="prime">Prime number in the module of the equation Y^2 = X ^ 3 + A * X + B(mod p)</param>
        /// <returns>Point that represents the sum of First and Second Point</returns>
        public static Point Add(Point p, Point q, BigInteger a, BigInteger prime)
            => new Point(Mathematics.AddJacobian(p.ToJacobian(), q.ToJacobian(), a, prime), prime);
    }
}