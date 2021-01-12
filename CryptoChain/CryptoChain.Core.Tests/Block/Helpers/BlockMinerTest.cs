using System;
using System.Security.Cryptography;
using System.Threading;
using CryptoChain.Core.Block;
using CryptoChain.Core.Block.Helpers;
using CryptoChain.Core.Tests.Block.BlockData;
using NUnit.Framework;

namespace CryptoChain.Core.Tests.Block.Helpers
{
    public class BlockMinerTest
    {
        private const int TimeoutInSec = 30;
        
        [Test]
        public void MinerTest()
        {
            Block<TestBlockData> block = new Block<TestBlockData>(Convert.FromBase64String(
                "AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz5B3jtcrfXCB4AAP8AAAAAMTIzNDY1NDIzNTE0M2UAAAA="));
            var hashPrevBlock = block.GetBlockHeader().HashPrevBlock;

            using var sha256 = SHA256.Create();
            Assert.IsFalse(block.IsValid(hashPrevBlock,sha256),"Block is already valid before mining it");
            
            var token = new CancellationTokenSource();
            token.CancelAfter(TimeSpan.FromSeconds(TimeoutInSec));
            block = block.Mine(token.Token).Result;
            
            Assert.IsNotNull(block.ToArray(),"Miner timed out. Run again or increase timeout");
            Assert.IsTrue(block.IsValid(hashPrevBlock,sha256),"Block is not valid after mining it");
        }
    }
}