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

        public byte[] Compress()
        {
            var x = X.ToByteArray();
            byte[] buffer = new byte[x.Length + 1];
            x.CopyTo(buffer, 1);
            buffer[0] = Y.IsEven ? 0x02 : 0x03;
            return buffer;
        }

        public static Point Decompress(byte[] compressed, Curve curve)
        {
            bool isEven = compressed[0] == 0x02;
            var x = new BigInteger(compressed[1..]);
            
            var y2 = (BigInteger.ModPow(x, 3, curve.P)
                      + curve.A * x + curve.B) % curve.P;
            
            var y3 = BigInteger.ModPow(y2, (curve.P + 1) / 4, curve.P);
            var y = isEven ? y3 : curve.P - y3;

            var even = y % 2 == 0;
            isEven = even;//???
            
            Console.WriteLine($"Even: {isEven}, {even}");
            Console.WriteLine("Other: "+ (!isEven ? y3 : curve.P - y3).ToString("X2"));
            return new Point(x, y);
            
            //Find out if there is any way to check if it needs to use the one or the other
        }

        public int Length { get; }
        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}