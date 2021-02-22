using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.ECC;
using CryptoChain.Core.Cryptography.Algorithms.ECC.ECDSA;
using CryptoChain.Core.Cryptography.Algorithms.RSA;
using CryptoChain.Core.Cryptography.Hashing;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Data;
using CryptoChain.Core.Transactions.Scripting;
using CryptoChain.Core.Transactions.Scripting.Interpreter;

namespace CryptoChain.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var rsakey = new RsaKey();
            var ecckey = new EccKey(Curve.Secp251K1);

            ISignAlgorithm rsa = new CryptoRsa(rsakey);
            ISignAlgorithm ecdsa = new CryptoECDSA(ecckey);

            byte[] data = Encoding.UTF8.GetBytes("This data will be signed in a moment");
            byte[] signature;

            var sw = Stopwatch.StartNew();
            signature = rsa.Sign(data);
            Console.WriteLine("RSA sign: "+sw.Elapsed);
            sw.Restart();
            Debug.Assert(rsa.Verify(data, signature));
            Console.WriteLine("RSA verify: "+sw.Elapsed);
            sw.Restart();
            signature = ecdsa.Sign(data);
            Console.WriteLine("ECDSA sign: "+sw.Elapsed);
            sw.Restart();
            Debug.Assert(ecdsa.Verify(data, signature));
            Console.WriteLine("ECDSA verify: "+sw.Elapsed);

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