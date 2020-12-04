using System;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.RSA
{
    /// <summary>
    /// (c) 2020 maurictg, job79
    /// This class is able to generate strong RSA keys, even from a seed
    /// </summary>
    public class RsaKey : ISerializable
    {
        public RSAParameters Parameters { get; }
        public bool IsPrivate => (Parameters.D != null);

        /// <summary>
        /// Get RsaKey with only public information
        /// </summary>
        public RsaKey PublicKey => new RsaKey(new RSAParameters { Exponent = Parameters.Exponent, Modulus = Parameters.Modulus }, KeySize);

        private int _keySize;

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
        /// <param name="keySize">The keySize. This must match the parameters</param>
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
        /// <param name="isPrivate">Indicates if it contains the private parameters</param>
        /// <returns>RsaKey</returns>
        public static RsaKey FromPem(string pem, bool isPrivate)
            => new RsaKey(PemUtilities.FromPem(pem));
        
        
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

        public override string ToString() => ToPemString(true);
        
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
            return (withPrivate) ? PemUtilities.ExportPrivateKey(csp) : PemUtilities.ExportPublicKey(csp);
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
            return csp.ExportCspBlob(withPrivate);
        }

        //Not a happy getter!
        public int Length => Serialize().Length;
        public byte[] Serialize() => ToArray();
    }
}