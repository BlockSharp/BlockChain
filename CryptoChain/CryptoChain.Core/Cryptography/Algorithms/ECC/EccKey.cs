using System;
using System.Numerics;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class EccKey : ICryptoKey
    {
        public int Length => 2 + (IsPrivate ? PrivateKey.Length : PublicPoint.CompressedLength);
        public Curve Curve { get; }
        public Point PublicPoint { get; }

        public EccKey PublicEccKey => new (Curve, PublicPoint);
        
        /// <summary>
        /// Serialize ECC key
        /// </summary>
        /// <returns>byte[] with serialized key</returns>
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            buffer[0] = IsPrivate ? 0x01 : 0x00;
            buffer[1] = Curve.Id;
            if(IsPrivate)
                PrivateKey.CopyTo(buffer, 2);
            else
                PublicPoint.Compress().CopyTo(buffer, 2);
            return buffer;
        }

        /// <summary>
        /// Deserialize serialized EccKey
        /// </summary>
        /// <param name="serialized">The serialized EccKey</param>
        public EccKey(byte[] serialized)
        {
            IsPrivate = serialized[0] == 0x01;
            Curve = Curve.GetById(serialized[1]);
            byte[] data = serialized[2..];
            if (IsPrivate)
            {
                var ecc = new EccKey(Curve, data);
                PrivateKey = data;
                PublicPoint = ecc.PublicPoint;
            }
            else
            {
                PublicPoint = Point.Decompress(data, Curve);
            }
        }

        public bool IsPrivate { get; }
        public byte[] PublicKey => PublicEccKey.Serialize();
        public byte[] PrivateKey { get; } = new byte[0];
        public int KeySize => (int)Curve.LengthInBits;
        

        /// <summary>
        /// Initialize new EccKey
        /// </summary>
        /// <param name="curve">The curve</param>
        /// <param name="privateKey">The private key, a BigInteger</param>
        public EccKey(Curve curve, byte[] privateKey)
        {
            var p = new BigInteger(privateKey);
            if (!(p > 0 && p < curve.N - 1))
                throw new ArgumentException("Private key is not in the valid range p > 0 && p < curve.N");
            
            Curve = curve;
            PrivateKey = privateKey;
            PublicPoint = new CurveMath(Curve).ScalarMult(p, curve.G) ?? throw new InvalidOperationException();
            IsPrivate = true;
        }

        /// <summary>
        /// Create a new random Ecc keypair
        /// </summary>
        /// <param name="curve">The curve to be used</param>
        public EccKey(Curve curve) : 
            this(curve, new RandomGenerator {Active = false}.RandomInRange(0, curve.N - 1).ToByteArray())
        { }

        /// <summary>
        /// Initialize new EccKey from public point
        /// </summary>
        /// <param name="curve">The used curve</param>
        /// <param name="publicPoint">The public key Point</param>
        public EccKey(Curve curve, Point publicPoint)
        {
            Curve = curve;
            PublicPoint = publicPoint;
        }

        public string ToXmlString(bool withPrivate = true)
        {
            throw new NotImplementedException();
        }

        public string ToPemString(bool withPrivate = true)
        {
            throw new NotImplementedException();
        }

        public byte[] ToArray(bool withPrivate = true)
            => Serialize();
    }
}