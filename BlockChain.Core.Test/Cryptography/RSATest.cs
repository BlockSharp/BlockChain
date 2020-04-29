using System;
using System.Collections.Generic;
using System.Text;
using BlockChain.Core.Cryptography.RSA;
using NUnit.Framework;

namespace BlockChain.Core.Test.Cryptography
{
    public class RSATest
    {
        [Test]
        public void TestKeySizes()
        {
            RSAKey key512 = new RSAKey(512);
            RSAKey key1024 = new RSAKey(1024);
            RSAKey key2048 = new RSAKey(2048);

            Assert.AreEqual(308, key512.privateKey.Length);
            Assert.AreEqual(84, key512.publicKey.Length);
            Assert.AreEqual(64, new CryptoRSA(key512, true).Sign(new byte[1]).Length);

            Assert.AreEqual(596, key1024.privateKey.Length);
            Assert.AreEqual(148, key1024.publicKey.Length);
            Assert.AreEqual(128, new CryptoRSA(key1024, true).Sign(new byte[1]).Length);

            Assert.AreEqual(1172, key2048.privateKey.Length);
            Assert.AreEqual(276, key2048.publicKey.Length);
            Assert.AreEqual(256, new CryptoRSA(key2048, true).Sign(new byte[1]).Length);
        }
    }
}
