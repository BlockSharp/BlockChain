using System.Security.Cryptography;

namespace CryptoChain.Core.Abstractions
{
    /// <summary>
    /// Interface for data objects that are stored in a block
    /// </summary>
    public interface IBlockData
    {
        /// <summary>
        /// Convert instance to an array
        /// </summary>
        /// <returns>Instance of object as array</returns>
        public byte[] ToArray();

        /// <summary>
        /// Load object from a byte array
        /// </summary>
        /// <param name="data">Object as byte[], as returned by ToArray()</param>
        public void FromArray(byte[] data);

        /// <summary>
        /// Get hash of instance of object.
        /// Default: convert object to array and calculate hash.
        /// Hash must have a size of 256 bits (32 bytes)
        /// </summary>
        /// <param name="sha256"></param>
        /// <returns>Unique hash of object (32 bytes)</returns>
        public byte[] GetHashMerkleRoot(SHA256 sha256)
            => sha256.ComputeHash(ToArray());

        /// <summary>
        /// Determines if block data is valid.
        /// Default true
        /// </summary>
        /// <returns>True if block is valid</returns>
        public bool IsValid() => true;
    }
}