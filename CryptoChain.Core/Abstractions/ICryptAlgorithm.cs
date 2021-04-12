using System.Text;

namespace CryptoChain.Core.Abstractions
{
    public interface ICryptAlgorithm
    {
        /// <summary>
        /// Encrypt string
        /// </summary>
        /// <param name="text">The string to be encrypted</param>
        /// <returns>Encrypted string</returns>
        string Encrypt(string text);
        string Encrypt(string text, Encoding encoder);
        
        /// <summary>
        /// Encrypt data with crypt algorithm
        /// </summary>
        /// <param name="data">The data to be encrypted</param>
        /// <returns>The encrypted blob</returns>
        byte[] Encrypt(byte[] data);
        
        /// <summary>
        /// Decrypt encrypted string
        /// </summary>
        /// <param name="text">The text to be decrypted</param>
        /// <returns>The decrypted string</returns>
        string Decrypt(string text);
        string Decrypt(string text, Encoding encoder);
        
        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <param name="data">The encrypted data to be decrypted</param>
        /// <returns>The decrypted blob</returns>
        byte[] Decrypt(byte[] data);
    }
}