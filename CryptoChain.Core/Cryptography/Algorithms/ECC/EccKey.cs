using System;
using System.Numerics;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Algorithms.ECC.Curves;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    /// <summary>
    /// Elliptic Curve Key
    /// First byte in compressed form is the FLAG:
    /// 0x00: private key (just containing private key scalar)
    /// 0x01: contains only seed for EdDSA to generate private key from
    /// 0x02 & 0x03: compressed public key point
    /// 0x04: uncompressed public key point
    /// </summary>
    public class EccKey : ICryptoKey
    {
        /// <summary>
        /// Length is not representative.
        /// </summary>
        public int Length => -1;
        public Curve Curve { get; private set; }
        public Point PublicPoint { get; private set; }
        
        /// <summary>
        /// The key's private scalar (private key)
        /// </summary>
        public byte[]? Scalar { get; private set; }
        
        public EccKey PublicEccKey => new (Curve, PublicPoint);
        
        /// <summary>
        /// The private key seed (used in EdDSA to generate scalar)
        /// </summary>
        public byte[]? Seed { get; private set; }
        
        /// <summary>
        /// Serialize ECC key
        /// </summary>
        /// <returns>byte[] with serialized key</returns>
        public byte[] Serialize()
        {
            if (IsPrivate)
            {
                if (Scalar == null)
                    throw new ArgumentException("Cant serialize private key, scalar is null");
                
                if (Seed == null)
                {
                    byte[] buffer = new byte[2 + Scalar.Length];
                    buffer[0] = Curve.Id;
                    buffer[1] = 0x00;
                    Scalar.CopyTo(buffer, 2);
                    return buffer;
                }
                else
                {
                    byte[] buffer = new byte[2 + Seed.Length];
                    buffer[0] = Curve.Id;
                    buffer[1] = 0x01;
                    Seed.CopyTo(buffer, 2);
                    return buffer;
                }
            }

            //If not private, try public key compression
            try
            {
                if (Curve.Name != "Ed25519")
                    throw new NotImplementedException();
                
                byte[] compressed = Curve.Compress(PublicPoint);
                byte[] buffer = new byte[1 + compressed.Length];
                buffer[0] = Curve.Id;
                compressed.CopyTo(buffer,1);
                return buffer; //compressed already contains 0x03 or 0x04 flag
            }
            catch (NotImplementedException)
            {
                byte[] serialized = PublicPoint.Serialize();
                byte[] buffer = new byte[2 + serialized.Length];
                buffer[0] = Curve.Id;
                buffer[1] = 0x04;
                serialized.CopyTo(buffer, 2);
                return buffer;
            }
        }

        /// <summary>
        /// Deserialize serialized EccKey
        /// </summary>
        /// <param name="serialized">The serialized EccKey</param>
        public EccKey(byte[] serialized)
        {
            Curve = CurveCollection.GetById(serialized[0]);
            var flag = serialized[1];
            
            switch (flag)
            {
                case 0x00:
                    Scalar = serialized[2..];
                    break;
                case 0x01:
                    Seed = serialized[2..];
                    GenerateScalar();
                    break;
                case 0x02:
                case 0x03:
                    PublicPoint = Curve.Decompress(serialized[1..]);
                    break;
                case 0x04:
                    PublicPoint = new Point(serialized[2..]);
                    break;
            }

            //Assign public point if not already done
            PublicPoint ??= Curve.ScalarMult(Curve.G,
                new BigInteger(Scalar ?? throw new ArgumentException("Cant calculate public point without scalar")));
        }

        public bool IsPrivate => Scalar != null;
        
        private byte[]? _privateKey;
        private byte[]? _publicKey;
        
        public byte[] PublicKey 
            => _publicKey ??= ToArray(false);

        public byte[] PrivateKey
            => !IsPrivate ? throw new ArgumentException("Key is not a private key")
            : _privateKey ??= ToArray();

        /// <summary>
        /// Get key size
        /// </summary>
        public int KeySize => (int)Curve.N.GetBitLength() / 2;

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
            Scalar = privateKey;
            PublicPoint = Curve.ScalarMult(curve.G, p);
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
        
        private EccKey(){}

        /// <summary>
        /// Generate new EdDSA key pair from seed
        /// </summary>
        /// <param name="curve">The curve to be used</param>
        /// <param name="seed">The seed, optional. If left empty it will be generated</param>
        /// <returns>EccKey containing EdDSA key pair</returns>
        public static EccKey GenerateEdDsa(Curve curve, byte[]? seed = null)
        {
            var key = new EccKey
            {
                Curve = curve,
                Seed = seed ?? RandomGenerator.Secure.GetBytes((int) (curve.P.GetBitLength() + 1 / 8))
            };
            key.GenerateScalar();
            return key;
        }

        /// <summary>
        /// Generate scalar/private key from seed
        /// <param name="seed">Optional: seed if not already set</param>
        /// </summary>
        public void GenerateScalar(byte[]? seed = null)
        {
            Seed ??= seed;
            if (Seed == null)
                throw new ArgumentNullException(nameof(Seed));

            var h = Hash.SHA_512(Seed);

            BigInteger a = 0;
            for (int i = 0; i < Curve.P.GetBitLength(); i++)
                a += Mathematics.BitAt(h, i) << i;

            if (Curve.Flag.HasFlag(CurveFlags.NEED_SET_PSG))
            {
                a &= ~(Curve.H - 1);
            }

            if (Curve.Flag.HasFlag(CurveFlags.NEED_SET_MSB))
            {
                var bit = (int)Curve.N.GetBitLength() + 1;
                a |= BigInteger.One << bit;
            }
            
            Scalar = a.ToByteArray();
            PublicPoint = Curve.ScalarMult(Curve.G, new BigInteger(Scalar));
        }
        
        public string ToXmlString(bool withPrivate = true)
            => throw new NotImplementedException();

        public string ToPemString(bool withPrivate = true)
            => throw new NotImplementedException();

        public byte[] ToArray(bool withPrivate = true)
            => withPrivate ? Serialize() : PublicEccKey.Serialize();
    }
}