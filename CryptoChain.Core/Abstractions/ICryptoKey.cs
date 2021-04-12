namespace CryptoChain.Core.Abstractions
{
    public interface ICryptoKey : ISerializable
    {
        /// <summary>
        /// Indicates if the key is private or not
        /// </summary>
        bool IsPrivate { get; }
        
        /// <summary>
        /// The serialized public key data
        /// </summary>
        byte[] PublicKey { get; }
        
        /// <summary>
        /// The serialized private key data
        /// </summary>
        byte[] PrivateKey { get; }
        
        /// <summary>
        /// Indicates the bit size of the key
        /// </summary>
        int KeySize { get; }
        
        /// <summary>
        /// Convert key to XML representation
        /// </summary>
        /// <param name="withPrivate">Indicates if you want to include the private parameters</param>
        /// <returns>Key as XML string</returns>
        string ToXmlString(bool withPrivate = true);
        
        /// <summary>
        /// Convert key to PEM representation
        /// </summary>
        /// <param name="withPrivate">Indicates if you want to include the private parameters</param>
        /// <returns>Key as PEM string</returns>
        string ToPemString(bool withPrivate = true);
        
        /// <summary>
        /// Convert key to binary representation
        /// </summary>
        /// <param name="withPrivate">Indicates if you want to include the private part</param>
        /// <returns>byte[] with serialized key</returns>
        byte[] ToArray(bool withPrivate = true);
    }
}