using System;
using System.Numerics;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class CryptoECDSA : ISignAlgorithm
    {
        public ICryptoKey Key { get; }
        private readonly RandomGenerator _random;
        
        public CryptoECDSA(EccKey key)
        {
            _random = RandomGenerator.Secure;  //disable insecure random. Very important!
            Key = key;
        }
        
        //TODO: public key restore from signature? (P2PKH would be better)

        /// <summary>
        /// Sign the data using SHA256 as hashing algorithm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Sign(byte[] data) => Sign(data, SHA256.Create());
        public byte[] Sign(byte[] data, HashAlgorithm algorithm)
        {
            var key = (EccKey)Key;
            if (!key.IsPrivate || key.Scalar == null)
                throw new ArgumentException("Cannot sign data with a public key");
            
            using (algorithm)
            {
                var h = new BigInteger(algorithm.ComputeHash(data));
                
                while (true)
                {
                    var k = _random.RandomInRange(0, key.Curve.N - 1);
                    var rp = key.Curve.ScalarMult(key.Curve.G, k);
                    var r = rp.X % key.Curve.N;
                    var p = new BigInteger(key.Scalar);
                    if(r == BigInteger.Zero)
                        continue;
                
                    var s = Mathematics.ModInverse(k, key.Curve.N) 
                        * (h + r * p) % key.Curve.N;
                
                    if(s == BigInteger.Zero)
                        continue;

                    var signature = new Point(r, s);
                    
                    //(r, -s mod n) is also valid
                    return signature.Serialize();
                }
            }
        }

        /// <summary>
        /// Verify an ECDSA signature using the original data and the signature
        /// </summary>
        /// <param name="data">The data to be verified</param>
        /// <param name="signedData">The signature</param>
        /// <returns>True if combination of data and signature is right</returns>
        public bool Verify(byte[] data, byte[] signedData) => Verify(data, signedData, SHA256.Create());
        public bool Verify(byte[] data, byte[] signedData, HashAlgorithm algorithm)
        {
            using (algorithm)
            {
                var key = (EccKey)Key;
                var h = new BigInteger(algorithm.ComputeHash(data));
                var signature = new Point(signedData);
                key.Curve.EnsureContains(key.PublicPoint);
                
                var r = signature.X;
                var s = signature.Y;
                
                //Validate if signature is valid
                if (!(r > 1 && r < key.Curve.N - 1))
                    return false;
                
                if (!(s > 1 && s < key.Curve.N - 1))
                    return false;
                
                var c = Mathematics.ModInverse(s, key.Curve.N);
            
                //This can be calculated more efficient
                var u1 = (h * c) % key.Curve.N;
                var u2 = (r * c) % key.Curve.N;

                var xy = key.Curve.ScalarMult(key.Curve.G, u1);
                xy = key.Curve.Add(xy, key.Curve.ScalarMult(key.PublicPoint, u2));
                var v = xy.X % key.Curve.N;
                
                return v == r;
            }
        }

        /*
        public byte[] SignX(byte[] data)
        {
            var key = (EccKey) Key;
            using var sha = SHA256.Create();
            var h = sha.ComputeHash(data);
            var e = new BigInteger(h);
            var k = new RandomGenerator().RandomInRange(0, key.Curve.N);
            var rmp = key.Curve.ScalarMult(key.Curve.G, k);
            var r = rmp.X % key.Curve.N;
            Debug.Assert(r != 0);
            var s = new ModP(e + new BigInteger(key.Scalar) * r, key.Curve.N) / k;
            var sig = new Point(r, s.Value);
            return sig.Serialize();
        }

        public bool VerifyX(byte[] data, byte[] signature)
        {
            using var sha = SHA256.Create();
            var key = (EccKey)Key;
            var h = sha.ComputeHash(data);
            var e = new BigInteger(h);
            var sig = new Point(signature);
            var (rr, ss) = (sig.X, new ModP(sig.Y, key.Curve.N));
            var w = ss.Inverse();
            var u1 = (e * w).Value;
            var u2 = (rr * w).Value;
            var pt = key.Curve.Add(key.Curve.ScalarMult(key.Curve.G, u1),key.Curve.ScalarMult(key.PublicPoint, u2));
            var x1 = pt.X % key.Curve.N;
            return x1 == rr;
        }*/
    }
}