using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Cryptography.Algorithms.ECC.Curves
{
    /// <summary>
    /// Elliptic Curve base class.
    /// </summary>
    public abstract class Curve
    {
        /// <summary>
        /// The curve's name
        /// </summary>
        public string? Name { get; set; }
        
        /// <summary>
        /// Prime modulus
        /// </summary>
        public BigInteger P { get; init; }
        
        /// <summary>
        /// Order of the subgroup that is created by the generator point G
        /// </summary>
        public BigInteger N { get; init; }
        
        /// <summary>
        /// Generator point G of the curve
        /// </summary>
        [AllowNull]
        public Point G { get; init; }
        
        /// <summary>
        /// Cofactor of the generator subgroup.
        /// </summary>
        public int H { get; set; }
        
        /// <summary>
        /// The number of the curve in CryptoChain's curve database
        /// </summary>
        public byte Id { get; set; }
        
        /// <summary>
        /// The flag, used to set some values
        /// </summary>
        public CurveFlags Flag { get; set; }
        
        /// <summary>
        /// Indicates if a point is on the curve
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is on the curve</returns>
        public abstract bool Contains(Point point);

        /// <summary>
        /// Ensure that the curve contains a point
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <exception cref="ArgumentException">Thrown if point is not on curve</exception>
        public void EnsureContains(Point point)
        {
            if (!Contains(point))
                throw new ArgumentException("Curve does not contain point");
        }

        /// <summary>
        /// Sum two points on the curve
        /// </summary>
        /// <param name="p">The first point</param>
        /// <param name="q">The other point</param>
        /// <returns>The sum of the two points</returns>
        public abstract Point Add(Point p, Point q);

        /// <summary>
        /// Conjugate point
        /// </summary>
        /// <param name="p">The point to be conjugated</param>
        /// <returns>Point</returns>
        public abstract Point Negate(Point p);

        /// <summary>
        /// The infinity point, O.
        /// </summary>
        public abstract Point InfinityPoint { get; }

        /// <summary>
        /// Indicates if a point is the infinity point
        /// </summary>
        /// <param name="p">The point to be checked</param>
        /// <returns>True if the point equals the infinity point</returns>
        public abstract bool IsInfinity(Point p);

        /// <summary>
        /// Compress a public key point (or signature, in case of EdDSA). Note that not all curves will support this.
        /// When a curve doesnt support compression, it will throw an NotImplementedException
        /// </summary>
        /// <param name="p">The public key point to compress</param>
        /// <returns>Compressed data with flag</returns>
        public abstract byte[] Compress(Point p);

        /// <summary>
        /// Decompress a compressed point (or signature, in case of EdDSA). Note that not all curves will support this
        /// When a curve doesnt support compression, it will throw an NotImplementedException
        /// </summary>
        /// <param name="compressed">The compressed point</param>
        /// <returns>The decompressed public key point including the flag on position 0</returns>
        public abstract Point Decompress(byte[] compressed);

        /// <summary>
        /// Multiply point by scalar
        /// </summary>
        /// <param name="p">The point to be multiplied</param>
        /// <param name="scalar">The scalar</param>
        /// <returns>The multiplied point</returns>
        public Point ScalarMult(Point p, BigInteger scalar)
        {
            EnsureContains(p);

            if (scalar % N == 0 || IsInfinity(p))
            {
                DebugUtils.WriteLine("Infinity!",DebugUtils.MessageState.WARNING);
                return Point.Infinity;
            }

            if (scalar < 0)
            {
                DebugUtils.WriteLine("Negative! (curveBase)",DebugUtils.MessageState.WARNING);
                return ScalarMult(Negate(p), -scalar);
                //return ScalarMult(new Point(p.X, Mathematics.Mod(-p.Y, P)), -scalar);
            }
            
            var res = InfinityPoint;
            var n = p;

            if (scalar > 0)
            {
                var k = scalar;
                for (int i = 0; i < scalar.GetBitLength(); i++)
                {
                    if ((k & 1) == 1)
                        res = Add(res, n);
                    
                    n = Add(n, n);
                    k >>= 1;
                }
            }
            
            EnsureContains(res);
            return res;
        }
    }
}