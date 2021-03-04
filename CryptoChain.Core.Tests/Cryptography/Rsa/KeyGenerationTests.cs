using System.Security.Cryptography;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.RSA;
using NUnit.Framework;

namespace CryptoChain.Core.Tests.Cryptography.Rsa
{
    public class KeyGenerationTests
    {
        private static RandomGenerator _r = new RandomGenerator();
        
        [Test]
        public void CanGenerateKeyFromPrimes()
        {
            var p = RsaGenerator.GeneratePrime(ref _r, 512);
            var q = RsaGenerator.GeneratePrime(ref _r, 512);
            var parameters = RsaGenerator.GenerateParameters(p.prime, q.prime);
            var rsaKey = new RsaKey(parameters);
            Assert.AreEqual(1024, rsaKey.KeySize);
        }

        [Test]
        public void CanGenerateAndRestoreSeededPrimePair()
        {
            var spp = SeededPrimePair.Generate(1024);

            var exported = spp.Serialize();
            var spp2 = new SeededPrimePair(exported);
            //p and q are not stored in serialization so we check them
            Assert.AreEqual(spp.P, spp2.P);
            Assert.AreEqual(spp.Q, spp2.Q);
            Assert.AreEqual(spp.Seed, spp2.Seed);
        }
    }
}