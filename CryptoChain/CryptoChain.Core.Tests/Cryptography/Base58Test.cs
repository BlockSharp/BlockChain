using CryptoChain.Core.Cryptography;
using NUnit.Framework;

namespace CryptoChain.Core.Tests.Cryptography
{
    public class Base58Test
    {
        [Test]
        public void CanEncodeBase58()
        {
            byte[] data = {1,2,3,4,5,6,7,8,9,10};
            var encoded = Base58.Encode(data);
            Assert.AreEqual("4HUtbHhN2TkpR", encoded);
        }

        [Test]
        public void CanDecodeBase58()
        {
            byte[] data = {1,2,3,4,5,6,7,8,9,10};
            var decoded = Base58.Decode("4HUtbHhN2TkpR");
            Assert.AreEqual(data, decoded);
        }
    }
}