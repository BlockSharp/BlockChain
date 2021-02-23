using System.Numerics;
using System.Security.Cryptography;
using Math = System.Math;

namespace CryptoChain.Core.Cryptography.Algorithms
{
    /// <summary>
    /// To test, use: https://bigprimes.org/primality-test
    /// </summary>
    public class PrimeUtils
    {
        private RandomGenerator _generator;

        public PrimeUtils(ref RandomGenerator rg)
            => _generator = rg;

        /// <summary>
        /// First known primes
        /// </summary>
        public static readonly int[] FirstPrimes =
        {
            /*2,*/ 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101,
            103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 
            223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337,
            347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461
        };

        public static readonly BigInteger[] FermatNumbers =
        {
            new  BigInteger(257), new BigInteger(65537), BigInteger.Parse("4294967297"), BigInteger.Parse("18446744073709551617"),
            BigInteger.Parse("340282366920938463463374607431768211457"), BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007913129639937")
        };

        /// <summary>
        /// Generate low level prime
        /// </summary>
        /// <param name="size">size of low level prime</param>
        /// <returns></returns>
        public BigInteger GetLowLevelPrime(int size)
        {
            Start:
            var pc = _generator.RandomInRange(BigInteger.Pow(2, size - 1) + 1, BigInteger.Pow(2, size) - 1);
            foreach (var divisor in FirstPrimes)
                if (pc % divisor == 0 && BigInteger.Pow(divisor, 2) <= pc)
                    goto Start;
            return pc;
        }
        

        /// <summary>
        /// Determines whether biginteger is a prime
        /// </summary>
        /// <param name="n">a biginteger</param>
        /// <param name="k">the number of tests to be executed</param>
        /// <returns></returns>
        public bool IsPrime(BigInteger n, long k = 128)
        {
            if (n == 2 || n == 3) return true;
            if (n <= 1 || n % 2 == 0) return false;

            BigInteger s = 0, r = n - 1;
            while ((r & 1) == 0)
            {
                s += 1;
                r /= 2;
            }

            for (int i = 0; i < k; i++)
            {
                BigInteger a = _generator.RandomInRange(0, n - 1), x = BigInteger.ModPow(a, r, n);

                if (x != 1 && x != n - 1)
                {
                    var j = BigInteger.One;

                    while (j < s && x != n - 1)
                    {
                        x = BigInteger.ModPow(x, 2, n);
                        if (x == 1) return false;
                        j += 1;
                    }

                    if (x != n - 1) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a biginteger is a prime using the MillerRabin test
        /// From: https://www.geeksforgeeks.org/how-to-generate-large-prime-numbers-for-rsa-algorithm/
        /// </summary>
        /// <param name="mrc">Number to check</param>
        /// <returns>True or false</returns>
        public bool IsPrimeMillerRabin(BigInteger mrc)
        {
            int maxDevisionsByTwo = 0;
            BigInteger ec = mrc - BigInteger.One;

            while (ec % 2 == 0)
            {
                ec >>= 1;
                maxDevisionsByTwo++;
            }

            //Assert
            if (BigInteger.Pow(2, maxDevisionsByTwo) * ec != mrc - BigInteger.One)
                return false;

            bool TrialComposite(BigInteger roundTester)
            {
                if (BigInteger.ModPow(roundTester, ec, mrc) == BigInteger.One)
                    return false;

                for (int i = 0; i < maxDevisionsByTwo; i++)
                {
                    if (BigInteger.ModPow(roundTester, BigInteger.Pow(2, i) * ec, mrc) == mrc - BigInteger.One)
                        return false;
                }

                return true;
            }

            //Set number of trials here
            int numberOfRabinTrials = 20;
            for (int i = 0; i < numberOfRabinTrials; i++)
            {
                BigInteger roundTest = _generator.RandomInRange(2, mrc);
                if (TrialComposite(roundTest))
                    return false;
            }

            return true;
        }

        public static bool IsSmallPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (int i = 3; i <= (int)Math.Floor(Math.Sqrt(number)); i+=2)
                if (number % i == 0)
                    return false;

            return true;
        }
    }
}