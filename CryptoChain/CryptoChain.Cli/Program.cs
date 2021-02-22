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
using CryptoChain.Core.Cryptography.Algorithms.ECC.ECDSA;
using CryptoChain.Core.Cryptography.Algorithms.RSA;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var curve = Curve.Secp251R1;
            var random = new RandomGenerator() {Active = false};

            var priv = random.RandomInRange(0, curve.N - 1);

            var key = new EccKey(curve, priv.ToByteArray());
            var ecdsa = new CryptoECDSA(key);

            var sign = ecdsa.Sign(Encoding.UTF8.GetBytes("Petra is lief"));
            
            Console.WriteLine(ecdsa.Verify(Encoding.UTF8.GetBytes("Petra is lief"), sign));
            
            
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