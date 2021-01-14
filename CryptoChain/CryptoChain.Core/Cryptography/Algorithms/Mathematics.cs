using System.Numerics;
using CryptoChain.Core.Cryptography.Algorithms.ECC;

namespace CryptoChain.Core.Cryptography.Algorithms
{
    public static class Mathematics
    {
        /// <summary>
        /// Calculate greatest common factor using Euclid's Algorithm
        /// THIS IS THE SAME AS THE GCD (Greatest Common Divisor)
        /// GCF = The largest positive integer that divides a AND b
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>The greatest common factor</returns>
        public static BigInteger GCF(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static BigInteger GCD(BigInteger a, BigInteger b)
            => BigInteger.GreatestCommonDivisor(a, b);
        
        /// <summary>
        /// Calculate Least Common Multiple
        /// LCM = smallest positive integer that is divisible by a AND b
        /// </summary>
        /// <param name="a">The first number</param>
        /// <param name="b">The second number</param>
        /// <returns>The lowest common multiple</returns>
        public static BigInteger LCM(BigInteger a, BigInteger b)
            => (a / GCF(a, b)) * b;


        /// <summary>
        /// Calculates the modular multiplicative inverse of two bigintegers
        /// Uses the Extended Euclidean Algorithm
        /// Complexity: O(log(m))
        /// </summary>
        /// <param name="a">The first number (must be co-prime)</param>
        /// <param name="m">The other number (must be co-prime)</param>
        /// <returns>The modular multiplicative inverse</returns>
        public static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m, y = 0, x = 1;
            if (m == BigInteger.One) return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }

            if (x < 0) x += m0;
            
            return x;
        }
        
        /// <summary>
        /// Indicates if integer a and b are co-prime
        /// </summary>
        /// <param name="a">Integer a</param>
        /// <param name="b">Integer b</param>
        /// <returns>True if co-prime (Greatest Common Factor = 1)</returns>
        public static bool AreCoPrime(BigInteger a, BigInteger b)
            => GCF(a, b) == 1;
        
        
        /// <summary>
        /// Double a point in elliptic curves
        /// </summary>
        /// <param name="point">Point you want to double</param>
        /// <param name="a">Coefficient of the first-order term of the equation Y ^ 2 = X ^ 3 + A * X + B(mod p)</param>
        /// <param name="p">Prime number in the module of the equation Y^2 = X ^ 3 + A * X + B(mod p)</param>
        /// <returns>Point that represents the doubled point</returns>
        public static Point DoubleJacobian (Point point, BigInteger a, BigInteger p) {
            if (point.Y.IsZero)
                return new Point(BigInteger.Zero, BigInteger.Zero, BigInteger.Zero);

            BigInteger ysq = BigInteger.Pow(point.Y, 2) % p;
            BigInteger s = 4 * point.X * ysq % p;
            BigInteger m = 3 * BigInteger.Pow(point.X, 2) + a * BigInteger.Pow(point.Z, 4) % p;
            BigInteger nx = BigInteger.Pow(m, 2) - 2 * s % p;
            BigInteger ny = m * (s - nx) - 8 * BigInteger.Pow(ysq, 2) % p;
            BigInteger nz = 2 * point.Y * point.Z % p;
            return new Point(nx, ny, nz);
        }

        /// <summary>
        /// Add two points in elliptic curves
        /// </summary>
        /// <param name="p">First Point you want to add</param>
        /// <param name="q">Second Point you want to add</param>
        /// <param name="a">Coefficient of the first-order term of the equation Y^2 = X^3 + A*X + B (mod p)</param>
        /// <param name="prime">Prime number in the module of the equation Y^2 = X^3 + A*X + B (mod p)</param>
        /// <returns>Point that represents the sum of p and q</returns>
        public static Point AddJacobian(Point p, Point q, BigInteger a, BigInteger prime)
        {
            if (p.Y.IsZero) return q;
            if (q.Y.IsZero) return p;

            BigInteger u1 = p.X * BigInteger.Pow(q.Z, 2) % prime;
            BigInteger u2 = q.X * BigInteger.Pow(p.Z, 2) % prime;
            BigInteger s1 = p.Y * BigInteger.Pow(q.Z, 3) % prime;
            BigInteger s2 = q.Y * BigInteger.Pow(p.Z, 3) % prime;
            
            if (u1 == u2) {
                if (s1 != s2)
                    return new Point(BigInteger.Zero, BigInteger.Zero, BigInteger.One);
                
                return DoubleJacobian(p, a, prime);
            }

            BigInteger h = u2 - u1;
            BigInteger r = s2 - s1;
            BigInteger h2 = h * h % prime;
            BigInteger h3 = h * h2 % prime;
            BigInteger u1H2 = u1 * h2 % prime;
            BigInteger nx = BigInteger.Pow(r, 2) - h3 - 2 * u1H2 % prime;
            BigInteger ny = r * (u1H2 - nx) - s1 * h3 % prime;
            BigInteger nz = h * p.Z * q.Z % prime;

            return new Point(nx, ny, nz);
        }

        /// <summary>
        /// Multily point and scalar in elliptic curves
        /// </summary>
        /// <param name="p">First Point to multiply</param>
        /// <param name="n"> Scalar to multiply</param>
        /// <param name="N">Order of the elliptic curve</param>
        /// <param name="A">Coefficient of the first-order term of the equation Y^2 = X^3 + A*X + B (mod p)</param>
        /// <param name="P">Prime number in the module of the equation Y^2 = X^3 + A*X + B (mod p)</param>
        /// <returns>Point that represent the multiplication of point and scalar</returns>
        public static Point MultiplyJacobian(Point p, BigInteger n, BigInteger N, BigInteger A, BigInteger P) {
            if (p.Y.IsZero | n.IsZero) {
                return new Point(
                    BigInteger.Zero,
                    BigInteger.Zero,
                    BigInteger.One
                );
            }

            if (n.IsOne)
                return p;

            if (n < 0 | n >= N)
                return MultiplyJacobian(p, n % N, N, A, P);

            if ((n % 2).IsZero) {
                return DoubleJacobian(
                    MultiplyJacobian(p, n / 2, N, A, P), A, P
                );
            }

            // (n % 2) == 1:
            return AddJacobian(
                DoubleJacobian(MultiplyJacobian(p, n / 2, N, A, P), A, P), p, A, P
            );
        }
    }
}