using System;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.RSA
{
    /// <summary>
    /// This class is able to generate RSA parameters from 2 primes using math
    /// With help from Wikipedia: https://en.wikipedia.org/wiki/RSA_(cryptosystem)
    /// </summary>
    public static class RsaGenerator
    {
        /// <summary>
        /// The GeneratePrime generates a prime using a seeded RandomGenerator
        /// </summary>
        /// <param name="rg">The (seeded) randomGenerator</param>
        /// <param name="bitSize">The bitsize of the prime. Default = 1024</param>
        /// <param name="millerRabin">Indicates if you want to use millerRabin tests</param>
        /// <param name="logToConsole">Enable logging</param>
        /// <returns>(prime: BigInteger, iterations: long => indicates how much rounds it took before the prime was found)</returns>
        public static (BigInteger prime, uint iterations) GeneratePrime(ref RandomGenerator rg, int bitSize = 1024, bool millerRabin = true, bool logToConsole = true)
        {
#if !DEBUG
            logToConsole = false;
#endif
            
            var primeUtils = new PrimeUtils(ref rg);
            var sw = Stopwatch.StartNew();
            uint iterations = 0;

            for (int i = 0;; i++)
            {
                if(logToConsole)
                    Console.WriteLine(i);
                
                BigInteger candidate = primeUtils.GetLowLevelPrime(bitSize);
                iterations = rg.Iterations;

                bool isPrime = millerRabin
                    ? primeUtils.IsPrimeMillerRabin(candidate)
                    : primeUtils.IsPrime(candidate);
                
                if (isPrime)
                {
                    if(logToConsole) Console.WriteLine($"PRIME FOUND in {sw.Elapsed}!! " + candidate);
                    return (candidate, iterations);
                }
            }
        }
        
        
        /// <summary>
        /// This function generates RSAParameters from 2 primes!!!
        /// The default fermat number is 65537
        /// Also RsaCryptoServiceProvider uses this preselected value.
        /// </summary>
        /// <param name="p">The first prime</param>
        /// <param name="q">The second prime</param>
        /// <param name="fermatIndex">The index in the fermatNumbers array. Default = 1 (65537)</param>
        /// <returns>RSAParameters used in the RsaCSP</returns>
        public static RSAParameters GenerateParameters(BigInteger p, BigInteger q, int fermatIndex = 1)
        {
            /*
             * prime = an integer that's only dividable by 1 and itselve
             * co-prime = two integers that are only dividable by 1 (GCD(a,b) = 1)
             */
            
            //1. Choose 2 big primes => p and q
            //2. Compute n = pq
            var n = p * q;
            
            //3. Compute λ(n) where λ is Carmichael's totient function.
            //Since n = pq, λ(n) = lcm(λ(p),λ(q)),
            //and since p and q are prime, λ(p) = φ(p) = p − 1
            //and likewise λ(q) = q − 1.
            //Hence λ(n) = lcm(p − 1, q − 1). 
            var λn = Mathematics.LCM(p - 1, q - 1);
            
            //4. Choose an integer e such that 1 < e < λ(n) and gcd(e, λ(n)) = 1; that is, e and λ(n) are coprime. 
            //e having a short bit-length and small Hamming weight results in more efficient encryption  – the most commonly chosen value for e is 2^16 + 1 = 65,537. The smallest (and fastest) possible value for e is 3,
            //but such a small value for e has been shown to be less secure in some settings
            //Commonly a fermat number is used, nearly always 65537. Default it is 65537
            var e = PrimeUtils.FermatNumbers[fermatIndex];
            
            //Check if e is correct, just for sure
            if(!Mathematics.AreCoPrime(e, λn))
                throw new ArgumentException("The value e is wrongly chosen.");
            
            //5. Determine d as d ≡ e−1 (mod λ(n)); that is, d is the modular multiplicative inverse of e modulo λ(n). 
            //This means: solve for d the equation d⋅e ≡ 1 (mod λ(n));
            //d can be computed efficiently by using the Extended Euclidean algorithm,
            //since, thanks to e and λ(n) being coprime, said equation is a form of Bézout's identity, where d is one of the coefficients.
            var d = Mathematics.ModInverse(e, λn);
            
            //Now the other private parameters
            var dp = d % (p - 1);
            var dq = d % (q - 1);
            var invQ = Mathematics.ModInverse(q, p);

            return new RSAParameters
            {
                D = d.ToInvByteArray(),
                DP = dp.ToInvByteArray(),
                DQ = dq.ToInvByteArray(),
                Exponent = e.ToInvByteArray(),
                InverseQ = invQ.ToInvByteArray(),
                Modulus = n.ToInvByteArray(),
                P = p.ToInvByteArray(),
                Q = q.ToInvByteArray()
            };
        }
    }
}