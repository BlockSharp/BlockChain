using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.EdDSA
{
    public class CryptoEdDSA : ISignAlgorithm
    {
        public ICryptoKey Key { get; }

        public CryptoEdDSA(EccKey key)
        {
            Key = key;
        }

        public byte[] Sign(byte[] data)
            => Sign(data, SHA512.Create());

        public EdDSASignature CreateSignature(byte[] data, HashAlgorithm algorithm)
        {
            var key = (EccKey) Key;
            if (!key.IsPrivate)
                throw new ArgumentException("Cant sign data with a public key");
            if (key.Seed == null || key.Scalar == null)
                throw new ArgumentException("For EdDSA sign we need Seed AND Scalar");
            
            using (algorithm)
            {
                var h = algorithm.ComputeHash(key.Seed);
                var r = new BigInteger(algorithm.ComputeHash(h[32..64].Concat(data).ToArray()), true);
                var R = key.Curve.ScalarMult(key.Curve.G, r);
                
                var s1 = new BigInteger(algorithm.ComputeHash(key.Curve.Compress(R)[1..].Concat(key.Curve.Compress(key.PublicPoint)[1..]).Concat(data).ToArray()));
                var s = Mathematics.Mod(r + s1 * new BigInteger(key.Scalar), key.Curve.N);
                return new EdDSASignature(R, s, key.Curve);
            }
        }
        
        public byte[] Sign(byte[] data, HashAlgorithm algorithm)
            => CreateSignature(data, algorithm).Serialize();

        public bool Verify(byte[] data, byte[] signedData)
            => Verify(data, signedData, SHA512.Create());

        public bool VerifySignature(byte[] data, EdDSASignature signature, HashAlgorithm algorithm)
        {
            var key = (EccKey) Key;
            /*if (data.Length != 64) //TODO: signature.IsValid?
                throw new ArgumentException("Signature is not at the valid size (64 bytes)");*/

            using (algorithm)
            {
                var hash = algorithm.ComputeHash(key.Curve.Compress(signature.R)[1..].Concat(key.Curve.Compress(key.PublicPoint)[1..]).Concat(data).ToArray());
                var h = new BigInteger(hash);
                return key.Curve.ScalarMult(key.Curve.G, signature.S).Equals(
                    key.Curve.Add(signature.R, key.Curve.ScalarMult(key.PublicPoint, h)));
            }
        }

        public bool Verify(byte[] data, byte[] signedData, HashAlgorithm algorithm)
            => VerifySignature(data, new EdDSASignature(signedData, ((EccKey)Key).Curve), algorithm);
    }
}