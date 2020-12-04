using System.Numerics;

namespace CryptoChain.Core.Cryptography.RSA
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
    }
}