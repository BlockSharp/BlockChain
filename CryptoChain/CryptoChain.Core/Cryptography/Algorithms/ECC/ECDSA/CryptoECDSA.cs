using System;
using System.Numerics;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.ECDSA
{
    public class CryptoECDSA : ISignAlgorithm
    {
        public bool IsPrivate { get; }
        public ICryptoKey Key { get; }

        private CurveMath _math;
        private RandomGenerator _random;
        private PrimeUtils _utils;

        public CryptoECDSA(EccKey key)
        {
            _math = new CurveMath(key.Curve);
            _random = new RandomGenerator {Active = false};  //disable insecure random
            _utils = new PrimeUtils(ref _random);
            Key = key;
        }

        public byte[] Sign(byte[] data) => Sign(data, SHA256.Create());
        public byte[] Sign(byte[] data, HashAlgorithm algorithm)
        {
            var key = (EccKey)Key;
            if (!key.IsPrivate)
                throw new ArgumentException("Cannot sign data with a public key");
            
            using (algorithm)
            {
                var h = new BigInteger(algorithm.ComputeHash(data));
                
                while (true)
                {
                    var k = _utils.RandomInRange(0, key.Curve.N - 1);
                    var R = _math.ScalarMult(k, key.Curve.G);
                    var r = R.X % key.Curve.N;
                    var p = new BigInteger(key.PrivateKey);
                    if(r == BigInteger.Zero)
                        continue;
                
                    var s = Mathematics.ModInverse(k, key.Curve.N) 
                        * (h + r * p) % key.Curve.N;
                
                    if(s == BigInteger.Zero)
                        continue;

                    var signature = new Point(r, s);
                    //(r, -s mod n) is also valid
                    return signature.Serialize(); //Compress?
                }
            }
        }

        public bool Verify(byte[] data, byte[] signedData) => Verify(data, SHA256.Create(), signedData);
        public bool Verify(byte[] data, HashAlgorithm algorithm, byte[] signedData)
        {
            var key = (EccKey)Key;
            var h = new BigInteger(algorithm.ComputeHash(data));
            var signature = Point.Infinity; //Replace with decompressed point or deserialized point
            key.Curve.EnsureContains(key.PublicPoint);
            
            var r = signature.X;
            var s = signature.Y;

            //Validate if signature is valid
            if (r > 1 && r < key.Curve.N - 1)
                return false;
            if (s > 1 && s < key.Curve.N - 1)
                return false;
            
            var c = Mathematics.ModInverse(s, key.Curve.N);
            
            //This can be calculated more efficient
            var u1 = (h * c) % key.Curve.N;
            var u2 = (r * c) % key.Curve.N;

            var xy = _math.ScalarMult(u1, key.Curve.G);
            xy = _math.Add(xy, _math.ScalarMult(u2, key.PublicPoint));
            var v = xy.X % key.Curve.N;
            
            return v == r;
        }
    }
}