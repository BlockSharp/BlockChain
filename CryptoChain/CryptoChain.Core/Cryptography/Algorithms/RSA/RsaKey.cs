using System;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.RSA
{
    /// <summary>
    /// This class is able to generate strong RSA keys, even from a seed
    /// </summary>
    public class RsaKey : ICryptoKey
    {
        public RSAParameters Parameters { get; }
        public bool IsPrivate => Parameters.D != null;

        private byte[]? _privateKey;
        private byte[]? _publicKey;
        private int _keySize;

        public byte[] PublicKey
        {
            get
            {
                if (_publicKey == null)
                    _publicKey = ToArray(false);
                return _publicKey;
            }
        }
        
        public byte[] PrivateKey
        {
            get
            {
                if (!IsPrivate)
                    throw new ArgumentException("Key is not a private key");
                if (_privateKey == null)
                    _privateKey = ToArray(true);
                return _privateKey;
            }
        }

        /// <summary>
        /// Get the RsaKey with only the public information
        /// </summary>
        public RsaKey PublicRsaKey => 
            new (new RSAParameters { Exponent = Parameters.Exponent, Modulus = Parameters.Modulus }, KeySize);

        /// <summary>
        /// Returns KeySize in bits
        /// </summary>
        public int KeySize
        {
            get
            {
                if (_keySize > 0) return _keySize;
                using var csp = new RSACryptoServiceProvider();
                csp.ImportParameters(Parameters);
                _keySize = csp.KeySize;
                return csp.KeySize;
            }
        }
        
        /// <summary>
        /// Create new RSAKey object from generated RSAParameters
        /// </summary>
        /// <param name="parameters">Generated parameters</param>
        /// <param name="keySize">The keySize. Optional, it can also be generated</param>
        public RsaKey(RSAParameters parameters, int keySize = 0)
        {
            Parameters = parameters;
            _keySize = KeySize;
        }

        /// <summary>
        /// Deserialize RSA key
        /// </summary>
        /// <param name="serialized">The serialized RSA blob, private or public</param>
        public RsaKey(byte[] serialized)
        {
            using var csp = new RSACryptoServiceProvider();
            csp.ImportCspBlob(serialized);
            Parameters = csp.ExportParameters(!csp.PublicOnly);
            _keySize = csp.KeySize;
            csp.Dispose();
        }
        
        /// <summary>
        /// Create new random RSAKey
        /// </summary>
        /// <param name="keySize">The keysize (default = 1024)</param>
        public RsaKey(int keySize = 1024) : this(new RSACryptoServiceProvider(keySize)) {}
        
        /// <summary>
        /// Create RSAKey object from CryptoServiceProvider
        /// </summary>
        /// <param name="csp">The created RSACryptoServiceProvider</param>
        public RsaKey(RSACryptoServiceProvider csp) : this(csp.ExportParameters(!csp.PublicOnly), csp.KeySize){}

        /// <summary>
        /// Create RsaKey from PEM format
        /// </summary>
        /// <param name="pem">The PEM string</param>
        /// <returns>RsaKey</returns>
        public static RsaKey FromPem(string pem)
        {
            using var csp = new RSACryptoServiceProvider();
            csp.ImportFromPem(pem);
            return new RsaKey(csp);
        }

        /// <summary>
        /// Create RsaKey from XML
        /// </summary>
        /// <param name="xml">The XML string</param>
        /// <returns>RsaKey</returns>
        public static RsaKey FromXml(string xml)
        {
            using var csp = new RSACryptoServiceProvider();
            csp.FromXmlString(xml);
            return new RsaKey(csp);
        }

        public override string ToString() => ToPemString();
        
        /// <summary>
        /// Converts key to XML string
        /// </summary>
        /// <param name="withPrivate">Include private parameters</param>
        /// <returns>XML key</returns>
        public string ToXmlString(bool withPrivate = true)
        {
            using var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(Parameters);
            return csp.ToXmlString(withPrivate && IsPrivate);
        }

        /// <summary>
        /// Converts key to PEM format
        /// </summary>
        /// <param name="withPrivate">Indicates if to include private parameters</param>
        /// <returns>PEM string</returns>
        public string ToPemString(bool withPrivate = true)
        {
            using var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(Parameters);
            return withPrivate && IsPrivate ? PemUtilities.ExportPrivateKey(csp) : PemUtilities.ExportPublicKey(csp);
        }

        /// <summary>
        /// Serialize key to byte array
        /// </summary>
        /// <param name="withPrivate">If to include private parameters</param>
        /// <returns>byte[]</returns>
        public byte[] ToArray(bool withPrivate = true)
        {
            using var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(Parameters);
            return csp.ExportCspBlob(withPrivate && IsPrivate);
        }
        
        /// <summary>
        /// Checks if a keysize is in the valid key size range
        /// </summary>
        /// <param name="keySize">The keysize to check</param>
        /// <param name="throwError">Indicates if you want to throw an error if it is not</param>
        /// <returns></returns>
        public static bool IsValidKeySize(int keySize, bool throwError = false)
        {
            using var csp = new RSACryptoServiceProvider();
            foreach (var sizes in csp.LegalKeySizes)
            {
                DebugUtils.Log("min: "+sizes.MinSize+" max: "+sizes.MaxSize);
                if (keySize >= sizes.MinSize && keySize <= sizes.MaxSize)
                {
                    if (keySize % sizes.SkipSize != 0)
                    {
                        if (throwError) 
                            throw new ArgumentException("Keysize is invalid");
                        return false;
                    }
                }
                else
                {
                    if(throwError)
                        throw new ArgumentException("Key is not in the legal keysize range");
                    return false;
                }
                
                return true;
            }
            csp.Dispose();

            return true;
        }

        //Not a happy getter!
        public int Length => Serialize().Length;
        public byte[] Serialize() => ToArray();
    }
}