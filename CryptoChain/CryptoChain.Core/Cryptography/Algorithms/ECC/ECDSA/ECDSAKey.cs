using System.Numerics;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.ECDSA
{
    public class ECDSAKey : ICryptoKey
    {
        public int Length { get; }
        public bool IsPrivate => _secret != null;
        public byte[] PublicKey { get; }
        public byte[] PrivateKey { get; }
        public int KeySize { get; }
        
        public Curve Curve { get; }
        public Point Point { get; }

        private BigInteger? _secret;

        public ECDSAKey(Curve curve, BigInteger secret)
        {
            Curve = curve;
            _secret = secret;
            Point = Curve.G.Multiply(secret, curve.N, curve.A, curve.P);
        }
        
        
        
        public string ToXmlString(bool withPrivate = true)
        {
            throw new System.NotImplementedException();
        }

        public string ToPemString(bool withPrivate = true)
        {
            throw new System.NotImplementedException();
        }

        public byte[] ToArray(bool withPrivate = true)
        {
            throw new System.NotImplementedException();
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }
    }
}