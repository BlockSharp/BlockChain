using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Cryptography.RSA;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var pair = new SeededPrimePair(
                Convert.FromBase64String("AAgAAEn5AQC1GwEA/02AKSNnV093NwH75a0Rqw=="));

            var key = pair.ToRsaKey();

            var pubPem = key.ToPemString(true);

            var pubKey = RsaKey.FromPem(pubPem);
            Console.WriteLine(key.ToXmlString() == pubKey.ToXmlString());
        }
    }
}