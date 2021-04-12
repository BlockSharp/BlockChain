using System;
using System.Numerics;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    public class TwistedEdwardsCurve : Curve
    {
        /// <summary>
        /// Coefficient A of the curve equation Ax^2 + y^2 = 1 + Dx^2 y^2.
        /// </summary>
        public BigInteger A { get; set; }
        
        /// <summary>
        /// Coefficient D of the curve equation Ax^2 + y^2 = 1 + Dx^2 y^2.
        /// </summary>
        public BigInteger D { get; set; }

        public override Point InfinityPoint => new(0, 1);
        
        public override bool IsInfinity(Point p)
            => p.Equals(InfinityPoint);

        public override bool Contains(Point point)
        {
            var a = new ModP(A, P);
            var d = new ModP(D, P);
            var px = new ModP(point.X, P);
            var py = new ModP(point.Y, P);
            return (a * px.Pow(2)) + py.Pow(2)
                   == 1 + d * px.Pow(2) * py.Pow(2);
        }
        
        public override Point Add(Point p, Point q)
        {
            var a = new ModP(A, P);
            var d = new ModP(D, P);
            var px = new ModP(p.X, P);
            var py = new ModP(p.Y, P);
            var qx = new ModP(q.X, P);
            var qy = new ModP(q.Y, P);
            
            var x = (px * qy + qx * py) / (1 + d * px * qx * py * qy);
            var y = (py * qy - a * px * qx) / (1 - d * px * qx * py * qy);
            return new Point(x.New(), y.New());
        }

        public override Point Negate(Point p)
            => new (Mathematics.Mod(-p.X, P), Mathematics.Mod(p.Y, P));

        /// <summary>
        /// Compress a point. Note that it will add the prefix 0x02 to indicate that it is compressed
        /// </summary>
        /// <param name="p">The point to be compressed</param>
        /// <returns>Compressed point</returns>
        public override byte[] Compress(Point p)
        {
            /*if (Name != "Ed25519")
                throw new NotImplementedException();*/
            
            var len = (int)P.GetBitLength();
            var val = p.Y;
            val &= (BigInteger.One << len) - 1;
            val |= (p.X & 1) << len;
            var data = val.ToByteArray(true); //Removes trailing zeroes
            
            byte[] buffer = new byte[1 + data.Length];
            buffer[0] = 0x02; //compressed flag
            data.CopyTo(buffer, 1);
            return buffer;
        }

        private BigInteger RecoverX(BigInteger y)
        {
            var d = new ModP(D, P);
            var xx = (y * y - 1) / (d * y * y + 1);
            var x = xx.Pow((P + 3) / 8);
            if (x * x != xx)
            {
                var i = new ModP(-1, P).Sqrt().Item1;
                x = x * i;
            }

            if (x.Value % 2 != 0)
                x = -x;

            return x.Value;
            /*var d = new ModP(D, P);
            var a = new ModP(A, P);
            var x = ModP.Empty;
            var xx = (y * y - 1) / (d * y * y * a);
            if (P % 8 == 5)
            {
                x = xx.Pow((P + 3) / 8);
                if (x * x == -xx)
                    x = x * new ModP(2, P).Pow((P - 1) / 4);
            }
            else if (P % 4 == 3)
            {
                x = xx.Pow((P + 1) / 4);
                if (x * x != xx)
                    x = ModP.Empty;
            }

            return x.Value;*/
        }

        /// <summary>
        /// Decompress a compressed point. Note that it will ignore any flag at [0]
        /// </summary>
        /// <param name="compressed">Compressed data, prefixed with flag</param>
        /// <returns>Decompressed point</returns>
        public override Point Decompress(byte[] compressed)
        {
            /*if (Name != "Ed25519")
                throw new NotImplementedException();*/
                
            var len = (int) P.GetBitLength();
            var val = new BigInteger(compressed[1..], true);
            var y = val & ((BigInteger.One << len) - 1); //ok
            var x = RecoverX(y);
            var hibit = (val >> len) & 1;
            if ((x & 1) != hibit)
                x = P - x;
            return new Point(Mathematics.Mod(x, P), Mathematics.Mod(y, P));
        }
    }
}