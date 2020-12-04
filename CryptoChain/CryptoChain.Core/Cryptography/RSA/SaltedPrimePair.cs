using System;
using System.Numerics;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.RSA
{
    /// <summary>
    /// The SaltedPrimePair class stores seeding information for the generation of primes.
    /// </summary>
    public class SaltedPrimePair : ISerializable
    {
        public int KeySize { get; private set; }
        public byte[] Seed { get; private set; }
        public uint IterationsP { get; private set; }
        public uint IterationsQ { get; private set; }

        /// <summary>
        /// Deserialize SaltedPrimePair
        /// </summary>
        /// <param name="serialized"></param>
        public SaltedPrimePair(byte[] serialized)
        {
            KeySize = BitConverter.ToInt32(serialized);
            IterationsP = BitConverter.ToUInt32(serialized, 4);
            IterationsQ = BitConverter.ToUInt32(serialized, 8);
            Seed = Serialization.FromBuffer(serialized, 12, false);
        }

        /// <summary>
        /// Generates a new SaltedPrimePair without returning the primes.
        /// Please use SaltedPrimePair.Generate() if you need the primes.
        /// This can be used to provide to the user and generate the primes later
        /// </summary>
        /// <param name="keySize"></param>
        public SaltedPrimePair(int keySize)
        {
            var data = SaltedPrimePair.Generate(keySize);
            KeySize = data.pair.KeySize;
            Seed = data.pair.Seed;
            IterationsP = data.pair.IterationsP;
            IterationsQ = data.pair.IterationsQ;
        }
        
        private SaltedPrimePair(){}

        /// <summary>
        /// Generate 2 new primes for RSA.
        /// This generates a SaltedPrimePair class which contains the salting information 
        /// </summary>
        /// <param name="keySize">The desired key size</param>
        /// <param name="saltSize">The salt size. default 16</param>
        /// <returns>(Prime p, Prime q, SaltedPrimePair)</returns>
        public static (BigInteger p, BigInteger q, SaltedPrimePair pair) Generate(int keySize = 2048, int saltSize = 16)
        {
            var pair = new SaltedPrimePair();
            pair.KeySize = keySize;
            pair.Seed = new byte[saltSize];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(pair.Seed);
            var random = new RandomGenerator(pair.Seed);
            var p = RsaGenerator.GeneratePrime(ref random, keySize / 2);
            uint iterationsBetween = random.Iterations;
            var q = RsaGenerator.GeneratePrime(ref random, keySize / 2);
            pair.IterationsP = p.iterations;
            pair.IterationsQ = q.iterations - iterationsBetween;

            return (p.prime, q.prime, pair);
        }

        /// <summary>
        /// Restores the two primes from the salting information inside
        /// </summary>
        /// <returns>(Prime p, Prime q)</returns>
        public (BigInteger p, BigInteger q) Restore()
        {
            var random = new RandomGenerator(Seed);
            random.Skip(IterationsP);
            var p = RsaGenerator.GeneratePrime(ref random, KeySize / 2);
            random.Skip(IterationsQ);
            var q = RsaGenerator.GeneratePrime(ref random, KeySize / 2);
            Console.WriteLine($"{p.iterations}, {q.iterations}");
            return (p.prime, q.prime);
        }

        /// <summary>
        /// Converts the information to a valid RsaKey
        /// </summary>
        /// <returns>RsaKey</returns>
        public RsaKey ToRsaKey()
        {
            var primes = Restore();
            var rsaParameters = RsaGenerator.GenerateKey(primes.p, primes.q);
            return new RsaKey(rsaParameters);
        }

        public int Length => 12 + Seed.Length;
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(KeySize), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(IterationsP), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(IterationsQ), 0, buffer, 8, 4);
            Buffer.BlockCopy(Seed, 0, buffer, 12, Seed.Length);
            return buffer;
        }
    }
}