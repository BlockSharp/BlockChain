using System;
using System.Numerics;
using System.Security.Cryptography;

namespace CryptoChain.Core.Cryptography
{
    /// <summary>
    /// This class provides a semi-random number generator which can be safely seeded.
    /// </summary>
    public class RandomGenerator
    {
        private readonly Random _rnd;
        private readonly Random _rnd2;
        public byte[] Seed { get; private set; }
        private readonly byte[] _data;
        private uint _nextSkip;
        public uint Iterations { get; private set; }
        public bool Active { get; set; }
        
        private const int SaltLength = 16;
        
        /// <summary>
        /// Create a new instance of this semirandom number generator
        /// </summary>
        /// <param name="seed">The seed to make it not fully random</param>
        public RandomGenerator(byte[]? seed = null)
        {
            if (seed == null)
            {
                seed = new byte[16];
                using var rng = new RNGCryptoServiceProvider();
                rng.GetBytes(seed);
            }
            
            if(seed.Length < 8)
                throw new ArgumentException("Seed is too small. Please provide a seed of 64 bits or greater (8 bytes)");
            
            _rnd = new Random(BitConverter.ToInt32(seed));
            _rnd2 = new Random(BitConverter.ToInt32(seed, 4));
            Seed = seed;
            _data = new byte[32 + seed.Length];
            Active = true;
        }

        public void Skip(uint amount)
            => _nextSkip = amount;

        /// <summary>
        /// Fill byte array with semi random bytes
        /// </summary>
        /// <param name="bytes">The bytes</param>
        public void GetBytes(byte[] bytes)
            => Buffer.BlockCopy(GetBytes(bytes.Length), 0, bytes, 0, bytes.Length);

        /// <summary>
        /// Composes the data needed for the hashing in GetBytes
        /// </summary>
        /// <param name="iterations">The amount of iterations to skip before the result</param>
        /// <returns>byte[] data and byte[] salt</returns>
        private (byte[] data, byte[] salt) GetData(uint iterations = 1)
        {
            byte[] buffer = new byte[16];
            byte[] salt = new byte[SaltLength];

            if (_nextSkip > 0)
            {
                iterations = _nextSkip;
                _nextSkip = 0;
            }

            for (uint i = 0; i < iterations; i++)
            {
                _rnd.NextBytes(buffer);
                buffer.CopyTo(_data, 0);
                _rnd2.NextBytes(buffer);
                buffer.CopyTo(_data, 16);
                Seed.CopyTo(_data, 32);
                _rnd.NextBytes(salt);
            }
            
            return (buffer, salt);
        }

        /// <summary>
        /// Get semirandom bytes from the generator
        /// </summary>
        /// <param name="count">The amount of bytes you want</param>
        /// <param name="iterations">Amount of iterations for the hash algorithm that generates the bytes</param>
        /// <returns>A byte array</returns>
        public byte[] GetBytes(int count, int iterations = 1)
        {
            if (Active)
            {
                Iterations++;
                var randomData = GetData();
                using var rfc = new Rfc2898DeriveBytes(randomData.Item1, randomData.Item2, iterations);
                return rfc.GetBytes(count);
            }

            //If not active, use RNG for strong cryptography. Not active = NOT using seed!
            using var rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[count];
            rng.GetBytes(buffer);
            return buffer;
        }
        
        /// <summary>
        /// Get random number within range
        /// </summary>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <returns>random BigInteger</returns>
        public BigInteger RandomInRange(BigInteger min, BigInteger max)
        {
            var bytes = new byte[max.GetByteCount()];
            while (true)
            {
                GetBytes(bytes);
                
                bytes[^1] &= 0x7F; // force sign bit to positive
                bytes[0] |= 1; // force last bit to 1

                var random = new BigInteger(bytes);
                if (random <= max && random >= min) return random;
            }
        }
    }
}