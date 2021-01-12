using System;
using System.Security.Cryptography;
using CryptoChain.Core.Block;
using CryptoChain.Core.Block.Helpers;
using NUnit.Framework;

namespace CryptoChain.Core.Tests.Block.Helpers
{
    public class BlockHeaderHelperTest
    {
        [Test]
        public void CloneTest()
        {
            var header = new BlockHeader(Convert.FromBase64String("AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz/Pzf0dSjKKw9MTEwAAAAA"));
            
            var clone = header.Clone();
            header.SetNonce(10);
            Assert.IsFalse(TestHelper.ArrayEquals(header.ToArray(), clone.ToArray()),
                "The cloned header changed while modifying the original header");
        }
        
        [Test]
        public void HashTest()
        {
            using var sha256 = SHA256.Create();
            var header = new BlockHeader(Convert.FromBase64String("AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz/Pzf0dSjKKw9MTEwAAAAA"));
            var hash = Convert.FromBase64String("QhSalWNkyW/zow7GkAcjLQ6jkc850ewcMPlZZ/Zp/Og=");
            
            Assert.IsTrue(TestHelper.ArrayEquals(header.Hash(sha256),hash),
                "Calculated hash did not match suspected hash");
        }
    }
}