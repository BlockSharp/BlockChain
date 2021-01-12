using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Block;
using CryptoChain.Core.Tests.Block.BlockData;
using NUnit.Framework;

namespace CryptoChain.Core.Tests.Block
{
    public class BlockTest
    {
        [Test]
        public void ConstructorsTest()
        {
            using var sha256 = SHA256.Create();
            var data = new TestBlockData("1234654235143");
            var target = new Target(new byte[]{ 30, 255, 0, 0 });
            byte[] hashPrevBlock = Convert.FromBase64String("CYoIPu0innBYIdJgwQT1ceFccDgBCwfSAxCKKpCQiPA="),
                hashMerkleRoot = ((IBlockData) data).GetHashMerkleRoot(sha256);
            var header = new BlockHeader(hashPrevBlock,hashMerkleRoot,target.ToBits());
            
            var blocks = new List<Block<TestBlockData>>()
            {
                new Block<TestBlockData>(header,data),
                Block<TestBlockData>.Create(hashPrevBlock, data, target, sha256)
            };
            blocks.Add(new Block<TestBlockData>(blocks[0].ToArray()));

            AreEqualTest(blocks);
            GetBlockHeaderTest(blocks[0], header);
            GetDataTest(blocks[0],data);
            GetSizeTest(blocks[0]);
        }

        private void AreEqualTest(List<Block<TestBlockData>> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                byte[] data = blocks[i].ToArray();
                Buffer.BlockCopy(blocks[0].GetBlockHeader().Time,0,data,68,8 );//Replace time in blockheader.
                Assert.IsTrue(TestHelper.ArrayEquals(blocks[0].ToArray(),data),
                    "Blocks that are constructed with the same values are different");
            }
        }
        
        private void GetBlockHeaderTest(Block<TestBlockData> block, BlockHeader header)
        {
            Assert.IsTrue(TestHelper.ArrayEquals(block.GetBlockHeader().ToArray(),header.ToArray()),
                "BlockHeader is different from BlockHeader in the block");
        }
        
        private void GetDataTest(Block<TestBlockData> block, TestBlockData data)
        {
            Assert.IsTrue(TestHelper.ArrayEquals(block.GetData().ToArray(),data.ToArray()),
                "Data is different from Data in the block");
        }
        
        private void GetSizeTest(Block<TestBlockData> block)
        {
            Assert.AreEqual(block.ToArray().Length,block.GetSize(),
                "Block size is invalid");
        }
    }
}