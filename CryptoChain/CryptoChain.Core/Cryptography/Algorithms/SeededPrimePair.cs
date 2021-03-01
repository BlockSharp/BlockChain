using System;
using System.Numerics;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Algorithms.RSA;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms
{
    /// <summary>
    /// The SeededPrimePair class stores seeding information for the generation of primes.
    /// </summary>
    public class SeededPrimePair : ISerializable
    {
        public int KeySize { get; private set; }
        public byte[] Seed { get; private set; }
        public uint IterationsP { get; private set; }
        public uint IterationsQ { get; private set; }

        private BigInteger _p = BigInteger.Zero;
        private BigInteger _q = BigInteger.Zero;

        public BigInteger P
        {
            get
            {
                if(_p == BigInteger.Zero) Restore();
                return _p;
            }
        }

        public BigInteger Q
        {
            get
            {
                if(_q == BigInteger.Zero) Restore();
                return _q;
            }
        }

        /// <summary>
        /// Deserialize SeededPrimePair
        /// </summary>
        /// <param name="serialized"></param>
        public SeededPrimePair(byte[] serialized)
        {
            KeySize = BitConverter.ToInt32(serialized);
            IterationsP = BitConverter.ToUInt32(serialized, 4);
            IterationsQ = BitConverter.ToUInt32(serialized, 8);
            Seed = Serialization.FromBuffer(serialized, 12, false);
        }
        
        private SeededPrimePair()
            => Seed = new byte[0];

        /// <summary>
        /// Generate 2 new primes for RSA.
        /// This generates a SeededPrimePair class which contains the salting information 
        /// </summary>
        /// <param name="keySize">The desired key size</param>
        /// <param name="seedSize">The seed size. default 16</param>
        /// <returns>A brand new SeededPrimePair</returns>
        public static SeededPrimePair Generate(int keySize = 2048, int seedSize = 16)
        {
            var seed = new byte[seedSize];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(seed);
            return Generate(seed, keySize);
        }

        /// <summary>
        /// Generates a SeededPrimePair from a seed
        /// </summary>
        /// <param name="seed">The seed</param>
        /// <param name="keySize">The desired key size</param>
        /// <returns>The SeededPrimePair</returns>
        public static SeededPrimePair Generate(byte[] seed, int keySize = 2048)
        {
            RsaKey.IsValidKeySize(keySize, true);
            
            var pair = new SeededPrimePair();
            pair.KeySize = keySize;
            pair.Seed = seed;
            var random = new RandomGenerator(pair.Seed);
            var p = RsaGenerator.GeneratePrime(ref random, keySize / 2);
            uint iterationsBetween = random.Iterations;
            var q = RsaGenerator.GeneratePrime(ref random, keySize / 2);
            pair.IterationsP = p.iterations;
            pair.IterationsQ = q.iterations - iterationsBetween;
            pair._p = p.prime;
            pair._q = q.prime;
            return pair;
        }

        /// <summary>
        /// Restores the two primes from the salting information inside
        /// </summary>
        /// <returns>(Prime p, Prime q)</returns>
        private void Restore()
        {
            var random = new RandomGenerator(Seed);
            random.Skip(IterationsP);
            var p = RsaGenerator.GeneratePrime(ref random, KeySize / 2, true, false);
            random.Skip(IterationsQ);
            var q = RsaGenerator.GeneratePrime(ref random, KeySize / 2, true, false);
            _p = p.prime;
            _q = q.prime;
        }

        /// <summary>
        /// Converts the information to a valid RsaKey
        /// </summary>
        /// <returns>RsaKey</returns>
        public RsaKey ToRsaKey()
        {
            var rsaParameters = RsaGenerator.GenerateParameters(P, Q);
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