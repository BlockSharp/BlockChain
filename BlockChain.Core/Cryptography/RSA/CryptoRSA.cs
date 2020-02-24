using System;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Core.Cryptography.RSA
{
    public class CryptoRSA
    {
        private readonly RSACryptoServiceProvider provider;
        public bool isPrivate { get; private set; }

        /// <summary>
        /// Create new CryptoRSA object
        /// </summary>
        /// <param name="key">The public or private key</param>
        /// <param name="isPrivate">Indicates if the encryption uses the key as an private or public key</param>
        public CryptoRSA(RSAKey key, bool isPrivate = false) : this(new RSACryptoServiceProvider(), (isPrivate) ? key.privateKey : key.publicKey, isPrivate) { }
        public CryptoRSA(byte[] key, bool isPrivate = false) : this(new RSACryptoServiceProvider(), key, isPrivate) { }
        public CryptoRSA(RSACryptoServiceProvider provider, RSAKey key, bool isPrivate = false) : this(provider, (isPrivate) ? key.privateKey : key.publicKey, isPrivate) { }
        public CryptoRSA(RSACryptoServiceProvider provider, byte[] key, bool isPrivate = false) : this(provider, isPrivate)
        {
            if (key == null) throw new ArgumentException("Invalid key, key is null.");
            this.provider.ImportCspBlob(key);
        }

        /// <summary>
        /// Create new CryptoRSA object using existing RSACryptoServiceProvider
        /// </summary>
        /// <param name="provider">The existing provider</param>
        /// <param name="isPrivate">Indicates if the encryption uses the key as an private or public key</param>
        public CryptoRSA(RSACryptoServiceProvider provider, bool isPrivate = false)
        {
            this.isPrivate = isPrivate;
            this.provider = provider ?? throw new ArgumentNullException("Invalid provider, provider is null.");
        }

        /// <summary>
        /// Encrypt text with RSA
        /// </summary>
        /// <param name="text">The input string (UTF-8)</param>
        /// <returns>Encrypted string (BASE-64)</returns>
        public string Encrypt(string text) => Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(text)));
        public string Encrypt(string text, Encoding encoder)
           => Convert.ToBase64String(Encrypt(encoder.GetBytes(text)));

        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <param name="data">The binary data</param>
        /// <returns>Encrypted data</returns>
        public byte[] Encrypt(byte[] data)
            => provider.Encrypt(data, false);

        /// <summary>
        /// Decrypt encrypted text
        /// </summary>
        /// <param name="text">The encrypted string (BASE-64)</param>
        /// <returns>Decrypted string (UTF-8)</returns>
        public string Decrypt(string text) => Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(text)));
        public string Decrypt(string text, Encoding encoder)
           => encoder.GetString(Decrypt(Convert.FromBase64String(text)));

        /// <summary>
        /// Decrypt encrypted data
        /// </summary>
        /// <param name="data">The encryped data</param>
        /// <returns>decrypted byte[]</returns>
        public byte[] Decrypt(byte[] data)
            => (isPrivate) ? provider.Decrypt(data, false) : throw new ArgumentException("Can't decrypt data with a public key");

        /// <summary>
        /// Sign binary data with the private key using SHA256
        /// </summary>
        /// <param name="data">The data to be signed</param>
        /// <returns>byte[32]</returns>
        public byte[] Sign(byte[] data) => Sign(data, SHA256.Create());
        public byte[] Sign(byte[] data, HashAlgorithm algorithm)
            => (isPrivate) ? provider.SignData(data, algorithm) : throw new ArgumentException("Can't sign data with a public key");

        /// <summary>
        /// Verify signed data with the public key
        /// </summary>
        /// <param name="data">The data to be validated with the signed data</param>
        /// <param name="signedData">byte[32] signed data</param>
        /// <returns>true or false</returns>
        public bool Verify(byte[] data, byte[] signedData) => Verify(data, SHA256.Create(), signedData);
        public bool Verify(byte[] data, HashAlgorithm algorithm, byte[] signedData)
            => provider.VerifyData(data, algorithm, signedData);

        /// <summary>
        /// Get key object from the provider. This does not work correctly yet
        /// </summary>
        /// <returns>RSAKey object</returns>
        public RSAKey GetKeyObject()
            => (isPrivate) ? new RSAKey(provider) : throw new ArgumentException("Can't get key object because private key is unknown");

        /// <summary>
        /// Get private or public key
        /// </summary>
        /// <returns>byte[] key</returns>
        public byte[] GetKey()
            => provider.ExportCspBlob(isPrivate);
    }
}