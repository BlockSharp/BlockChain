using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.ECC;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var curve = Curve.Sepc251K1;
            var math = new CurveMath(curve);
            var random = new RandomGenerator();
            var utils = new PrimeUtils(ref random);
            var priv = utils.RandomInRange(0, curve.N - 1);
            var pub = math.ScalarMult(priv, curve.G);
            
            //ECDSA signature
            byte[] message = Encoding.UTF8.GetBytes("Petra is lief");
            var h = new BigInteger(Hash.SHA_256(message));

            Point signature;

            while (true)
            {
                var k = utils.RandomInRange(0, curve.N - 1);
                var R = math.ScalarMult(k, curve.G);
                var r = R.X % curve.N;
                if(r == BigInteger.Zero)
                    continue;
                
                var s = Mathematics.ModInverse(k, curve.N) 
                    * (h + r * priv) % curve.N;
                
                if(s == BigInteger.Zero)
                    continue;

                signature = new Point(r, s);
                //(r, -s mod n) is also valid
                break;
            }
            
            Console.WriteLine(signature.X);
            Console.WriteLine(signature.Y);


            {
                h = new BigInteger(Hash.SHA_256(Encoding.UTF8.GetBytes("Petra is lief")));
                //ECDSA verify
                curve.EnsureContains(pub);
                var r = signature.X;
                var s = signature.Y;
                
                //check if signature is valid
                Debug.Assert(r > 1 && r < curve.N - 1);
                Debug.Assert(s > 1 && s < curve.N - 1);

                var c = Mathematics.ModInverse(s, curve.N);
                var u1 = (h * c) % curve.N;
                var u2 = (r * c) % curve.N;

                var xy = math.ScalarMult(u1, curve.G);
                xy = math.Add(xy, math.ScalarMult(u2, pub));
                var v = xy.X % curve.N;
                bool valid = v == r;
                Console.WriteLine("Valid: "+valid);
            }

            /*
            //https://en.bitcoin.it/wiki/List_of_address_prefixes
            //see length

            List<string> res = new List<string>(255);

            for (int i = 0; i < byte.MaxValue + 1; i++)
            {
                for (int j = 0; j < byte.MaxValue + 1; j++)
                {
                    res.Add(TestAddress(64, (byte)i, (byte)j));
                }

                char t = res.First()[0];
                if(res.All(x => x.StartsWith(t)))
                    Console.WriteLine($"prefix {i} starts with {t}");
                
                res.Clear();
            }*/
        }

        static string TestAddress(int count, byte prefix = 0, byte leading = 0)
        {
            byte[] buffer = new byte[1 + count + 4];
            buffer[0] = prefix;
            var data = FillRandom(count, leading);
            data.CopyTo(buffer, 1);
            var checksum = new Checksum(data, 4);
            checksum.Value.CopyTo(buffer, 1 + count);
            return Base58.Encode(buffer);
        }

        static byte[] FillRandom(int count, byte leading = 0)
        {
            byte[] buffer = new byte[count];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            buffer[0] = leading;
            return buffer;
        }
    }
}