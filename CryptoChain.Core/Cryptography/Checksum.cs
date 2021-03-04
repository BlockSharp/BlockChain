using System;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Cryptography
{
    public class Checksum : ISerializable
    {
        public byte[] Value { get; }
        public int Length => Value.Length;

        public Checksum(byte[] serialized)
            => Value = serialized;

        /// <summary>
        /// Create a new checksum
        /// </summary>
        /// <param name="data">The data to be checked</param>
        /// <param name="length">The length of the checksum. Normally it is 4 bytes</param>
        public Checksum(byte[] data, int length)
        {
            if (length > 32 || length < 4)
                throw new ArgumentException("Checksum size must be at least 4 bytes and max 32 bytes.");
            Value = GetChecksum(data)[..length];
        }

        /// <summary>
        /// Validate the data through the checksum
        /// </summary>
        /// <param name="data">The data to be validated</param>
        /// <returns>true when the data/checksum is right</returns>
        public bool Validate(byte[] data)
            => GetChecksum(data)[..Value.Length].SequenceEqual(Value);
        
        private byte[] GetChecksum(byte[] data)
            => Hash.HASH_256(data);

        public byte[] Serialize()
            => Value;
    }
}