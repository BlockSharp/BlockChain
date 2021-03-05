using System;
using System.Numerics;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC
{
    public class Point : ISerializable
    {
        protected BigInteger? _x;
        protected BigInteger? _y;

        public BigInteger X => _x ?? throw new ArgumentException("Point does not contain X");
        public BigInteger Y => _y ?? throw new ArgumentException("Point does not contain Y");

        //Delete these when finished with new curve system
        public static Point Infinity => new (null, null);
        public bool IsInfinity => Equals(Infinity);
        
        public Point(BigInteger? x, BigInteger? y)
        {
            _x = x;
            _y = y;
        }

        public Point(byte[] serialized)
        {
            int xlen = BitConverter.ToInt32(serialized);
            _x = new BigInteger(serialized[4..(4 + xlen)]);
            _y = new BigInteger(serialized[(4 + xlen)..]);
        }

        public int CompressedLength => X.GetByteCount() + 1;

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

        public bool Equals(Point other)
            => _x.Equals(other._x) && _y.Equals(other._y);

        public override string ToString() => $"X: 0x{_x?.ToString("x2")}, Y: 0x{_y?.ToString("x2")}";
    }
}