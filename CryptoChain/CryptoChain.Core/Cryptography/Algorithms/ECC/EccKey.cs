using System;
using System.Numerics;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class EccKey : ICryptoKey
    {
        public int Length { get; }
        public Curve Curve { get; }
        public Point PublicPoint { get; }
        
        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        public bool IsPrivate { get; }
        public byte[] PublicKey { get; }
        public byte[] PrivateKey { get; }
        public int KeySize => (int)Curve.LengthInBits;

        private EccKey(Curve curve)
            => Curve = curve;

        public EccKey(Curve curve, byte[] privateKey) : this(curve)
        {
            PrivateKey = privateKey;
            PublicPoint = new CurveMath(Curve).ScalarMult(new BigInteger(privateKey), curve.G) ?? throw new InvalidOperationException();
            PublicKey = CompressPubPoint();
            IsPrivate = true;
        }

        public EccKey(Curve curve, Point publicPoint) : this(curve)
        {
            PublicPoint = publicPoint;
            PublicKey = CompressPubPoint();
        }

        private byte[] CompressPubPoint()
            => PublicPoint.Compress();

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
    }
}