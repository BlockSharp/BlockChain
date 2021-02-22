using System;
using System.Linq;
using System.Numerics;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class Point : ISerializable
    {
        public BigInteger X { get; }
        public BigInteger Y { get; }

        public static Point? Infinity => null;

        public static bool IsInfinity(Point? point)
            => point == Infinity;
        
        public Point(BigInteger x, BigInteger y)
        {
            X = x;
            Y = y;
        }

        public Point(byte[] serialized)
        {
            int xlen = BitConverter.ToInt32(serialized);
            byte[] x = serialized[4..(4 + xlen)];
            byte[] y = serialized[(4 + xlen)..];
            X = new BigInteger(x);
            Y = new BigInteger(y);
        }

        /// <summary>
        /// Compress EC point to single X coordinate with sign
        /// </summary>
        /// <returns></returns>
        public byte[] Compress()
        {
            var x = X.ToByteArray();
            byte[] buffer = new byte[x.Length + 1];
            x.CopyTo(buffer, 1);
            buffer[0] = Y.IsEven ? 0x02 : 0x03;
            return buffer;
        }

        /// <summary>
        /// Decompress compressed coordinate to Point
        /// </summary>
        /// <param name="compressed">The compressed point</param>
        /// <param name="curve">The desired curve</param>
        /// <returns>The uncompressed X and Y coordinates</returns>
        public static Point Decompress(byte[] compressed, Curve curve)
        {
            byte sig = compressed[0];
            if (sig != 0x02 && sig != 0x03)
                throw new ArgumentException("Key is not compressed");
            
            var x = new BigInteger(compressed[1..]);

            var y2 = (BigInteger.ModPow(x, 3, curve.P)
                      + curve.A * x + curve.B) % curve.P;
            
            var y3 = BigInteger.ModPow(y2, (curve.P + 1) / 4, curve.P);
            
            var y = (y3 % 2 != sig - 2) ? curve.P - y3 : y3;

            return new Point(x, y);
        }

        public int Length => 4 + X.GetByteCount() + Y.GetByteCount();
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            var x = X.ToByteArray();
            var y = Y.ToByteArray();
            Buffer.BlockCopy(BitConverter.GetBytes(x.Length), 0, buffer, 0, 4);
            x.CopyTo(buffer, 4);
            y.CopyTo(buffer, 4 + x.Length);
            return buffer;
        }
    }
}