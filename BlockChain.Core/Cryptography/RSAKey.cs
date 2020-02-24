using System;
using System.Security.Cryptography;

namespace Blockchain.Core.Cryptography
{
    public class RSAKey
    {
        public byte[] publicKey { get; private set; }
        public byte[] privateKey { get; private set; }

        /// <summary>
        /// Create a new RSAKey object
        /// --------Key sizes:--------
        /// [1024] (default):
        /// public: 148 B, private 596 B, signature: 128 B
        /// [512]:
        /// public: 84 B, private: 308 B, signature: 64 B
        /// [384]:
        /// public: 68 B, private: 236 B, signature: 32 B
        /// </summary>
        /// <param name="keySize">The keysize. Default = 1024</param>
        public RSAKey(int keySize = 1024) : this(new RSACryptoServiceProvider(keySize))
        {
            if (keySize <= 0)
                throw new ArgumentException("Key size must be greater than zero");
        }

        /// <summary>
        /// Get RSA key from provider. Don't use this if you have no private key!
        /// </summary>
        /// <param name="provider">The RSACryptoServiceProvider including public and private key</param>
        public RSAKey(RSACryptoServiceProvider provider) : this(provider.ExportCspBlob(false), provider.ExportCspBlob(true)) { }

        /// <summary>
        /// Create RSA key object from the binary keys
        /// </summary>
        /// <param name="publicKey">The public key</param>
        /// <param name="privateKey">The private key</param>
        public RSAKey(byte[] publicKey, byte[] privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }
    }
}