using System.Security.Cryptography;
using System.Text;

namespace CryptoChain.Core.Abstractions
{
    public interface ISignAlgorithm
    {
        /// <summary>
        /// The key used in signing
        /// </summary>
        ICryptoKey Key { get; }
        
        /// <summary>
        /// Sign data
        /// </summary>
        /// <param name="data">The data to be signed</param>
        /// <returns>Signature blob</returns>
        byte[] Sign(byte[] data);
        byte[] Sign(byte[] data, HashAlgorithm algorithm);
        
        /// <summary>
        /// Verify signature
        /// </summary>
        /// <param name="data">The data to be verified</param>
        /// <param name="signedData">The signature to check</param>
        /// <returns>True if combination of signature and data is correct</returns>
        bool Verify(byte[] data, byte[] signedData);
        bool Verify(byte[] data, byte[] signedData, HashAlgorithm algorithm);
    }
}