using System;
using System.Numerics;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Algorithms.ECC.Curves;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.EdDSA
{
    public class EdDSASignature : ISerializable
    {
        /// <summary>
        /// Length is not representative, it is variable
        /// </summary>
        public int Length => -1;
        public Point R { get; set; }
        public BigInteger S { get; set; }
        public Curve Curve { get; set; }

        public EdDSASignature(Point r, BigInteger s, Curve curve)
        {
            R = r;
            S = s;
            Curve = curve;
        }

        public EdDSASignature(byte[] serialized, Curve curve)
        {
            Curve = curve;
            byte len = serialized[1];
            if (serialized[0] == 0x02)
            {
                S = new BigInteger(serialized[2..(2 + len)]);
                R = curve.Decompress(serialized[(1 + len)..]); //one bit too early, will be stripped away
            }
            else
            {
                S = new BigInteger(serialized[2..(2 + len)]);
                R = new Point(serialized[(2 + len)..]);
            }
        }
        
        public byte[] Serialize()
        {
            byte[] s = S.ToByteArray();
            byte[] r = R.Serialize();
            byte flag = 0x00;
            
            try
            {
                if (Curve.Name == "Ed25519")
                {
                    r = Curve.Compress(R)[1..];
                    flag = 0x02;
                }
            }
            catch (NotImplementedException) {}
            
            byte[] buffer = new byte[2 + r.Length + s.Length];
            buffer[0] = flag;
            buffer[1] = (byte)s.Length;
            s.CopyTo(buffer, 2);
            r.CopyTo(buffer, s.Length + 2);
            return buffer;
        }
    }
}