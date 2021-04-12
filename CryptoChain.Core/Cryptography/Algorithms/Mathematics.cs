using System;
using System.Globalization;
using System.Numerics;
using CryptoChain.Core.Helpers;

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
        /// <param name="checkForNegative">Indicates if you want to invert the operation if a is negative</param>
        /// <returns>The modular multiplicative inverse</returns>
        public static BigInteger ModInverse(BigInteger a, BigInteger m, bool checkForNegative = true)
        {
            if (a == 0)
                throw new DivideByZeroException("Can't divide by zero");

            if (checkForNegative && a < 0)
                return m - ModInverse(-a, m);
            
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

        public static BigInteger Sqrt(this BigInteger n)
        {
            if (n == 0) return 0;
            if (n > 0)
            {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!IsSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        private static bool IsSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root*root;
            BigInteger upperBound = (root + 1)*(root + 1);

            return (n >= lowerBound && n < upperBound);
        }

        public static BigInteger Mod(BigInteger k, BigInteger p)
        {
            if (p == 0)
                return 0;
            
            BigInteger r = k % p;
            return r < 0 ? r + p : r;
        }

        /// <summary>
        /// Get biginteger from hexadecimal string
        /// </summary>
        /// <param name="hex">The hex string</param>
        /// <returns>Biginteger</returns>
        public static BigInteger FromHex(string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2);
            
            if (hex.Length % 2 == 1 || hex[0] != '0')
                hex = "0" + hex; //else its negative
            
            return BigInteger.Parse(hex, NumberStyles.HexNumber);
        }
        
        /// <summary>
        /// Get bit at specific position in data
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="pos">The position</param>
        /// <returns>1 or 0</returns>
        public static BigInteger BitAt(byte[] data, long pos)
            => (data[pos / 8] >> (int)(pos % 8)) & 1;
    }
}