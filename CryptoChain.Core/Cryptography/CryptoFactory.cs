using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.ECC;
using CryptoChain.Core.Cryptography.Algorithms.RSA;

namespace CryptoChain.Core.Cryptography
{
    public static class CryptoFactory
    {
        public static ICryptoKey GetKey(byte[] data, Algorithm algorithm)
            => algorithm switch
            {
                Algorithm.RSA => new RsaKey(data),
                Algorithm.ECDSA => new EccKey(data),
                _ => throw new ArgumentException("Unknown algorithm", nameof(algorithm))
            };
        
        public static ISignAlgorithm GetSignAlgorithm(ICryptoKey key, Algorithm algorithm)
            => algorithm switch
            {
                Algorithm.RSA => new CryptoRsa((RsaKey)key),
                Algorithm.ECDSA => new CryptoECDSA((EccKey)key),
                _ => throw new ArgumentException("Unknown algorithm", nameof(algorithm))
            };
    }
}