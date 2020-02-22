using System;
using NUnit.Framework;
using BlockChain.Core.Block;
using System.Security.Cryptography;

namespace BlockChain.Core.Test.Block.Helpers
{
    public class BlockHelperTest
    {
        [Test]
        public void HashTest()
        {
            Block<TestBlockData> block = new Block<TestBlockData>(Convert.FromBase64String(
                "AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRwsVi115bbXCB7/AADQNAAAMTIzNDY1NDIzNTE0M2UAAAA="));
            byte[] expectedHash = Convert.FromBase64String("iKuu7YCjVUQmo3Ja2I0EPTg+rlYc0kwpZc+zzr+BAAA=");
            
            using var sha256 = SHA256.Create();
            Assert.IsTrue(TestHelper.ArrayEquals(block.Hash(sha256),expectedHash),"Hash did not match the expected hash");
        }

        [Test]
        public void IsValidTest()
        {
            Block<TestBlockData> validBlock = new Block<TestBlockData>(Convert.FromBase64String(
                    "AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz5B3jtcrfXCB4AAP/yBwEAMTIzNDY1NDIzNTE0M2UAAAA=")),
                blockWithInvalidHashForTarget = new Block<TestBlockData>(Convert.FromBase64String(
                    "AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz5B3jtcrfXCB4AAADyBwEAMTIzNDY1NDIzNTE0M2UAAAA=")),
                blockWithInvalidMerkleRoot = new Block<TestBlockData>(Convert.FromBase64String(
                    "AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHgCynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz5B3jtcrfXCB4AAP/yBwEAMTIzNDY1NDIzNTE0M2UAAAA=")),
                genesis = new Block<TestBlockData>(Constants.Genesis);
            var hashPrevBlock = validBlock.GetBlockHeader().HashPrevBlock;
            
            using var sha256 = SHA256.Create();
            Assert.IsTrue(validBlock.IsValid(hashPrevBlock,sha256),"Valid block was invalid");
            Assert.IsTrue(genesis.IsValid(genesis.GetBlockHeader().HashPrevBlock,sha256),"Genesis block was invalid");
            Assert.IsFalse(blockWithInvalidMerkleRoot.IsValid(hashPrevBlock,sha256),"Block with an invalid MerkleRoot was valid");
            Assert.IsFalse(blockWithInvalidHashForTarget.IsValid(hashPrevBlock,sha256),"Block with an invalid hash value for its target was valid");
            Assert.IsFalse(validBlock.IsValid(new byte[32],sha256),"Block with an invalid prevBlock was valid");
        }
    }
}