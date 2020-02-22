using System;
using NUnit.Framework;
using BlockChain.Core.Block;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BlockChain.Core.Test.Block
{
    public class BlockHeaderTest
    {
        [Test]
        public void ConstructorsTest()
        {
            using var sha256 = SHA256.Create();
            var data = new TestBlockData("1234654235143");
            var target = new Target(new byte[]{ 8, 21, 21, 21 });
            byte[] hashPrevBlock = Convert.FromBase64String("CYoIPu0innBYIdJgwQT1ceFccDgBCwfSAxCKKpCQiPA="),
                hashMerkleRoot = ((IBlockData)data).GetHashMerkleRoot(sha256);
            
            var headers = new List<BlockHeader>()
            {
                new BlockHeader(hashPrevBlock, hashMerkleRoot, target.ToBits()),
                BlockHeader.Create(hashPrevBlock, data, target, sha256)
            };
            headers.Add(new BlockHeader(headers[0].ToArray()));

            AreEqualTest(headers);
            HashPrevBlockTest(headers[0],hashPrevBlock);
            HashMerkleRootTest(headers[0],hashMerkleRoot);
            TimeTest(headers[0]);
            BitsTest(headers[0],target);
            NonceTest(headers[0]);
        }

        private void AreEqualTest(List<BlockHeader> headers)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                byte[] data = headers[i].ToArray();
                Buffer.BlockCopy(headers[0].Time,0,data,68,8 );//Replace time.
                Assert.IsTrue(TestHelper.ArrayEquals(headers[0].ToArray(),headers[i].ToArray()),
                    "Headers that are constructed with the same values are different");
            }
        }
        
        private void HashPrevBlockTest(BlockHeader header, byte[] hashPrevBlock)
        {
            Assert.IsTrue(TestHelper.ArrayEquals(header.HashPrevBlock,hashPrevBlock),
                "HashPrevBlock is different from the HashPrevBlock in the block header");
        }
        
        private void HashMerkleRootTest(BlockHeader header, byte[] hashMerkleRoot)
        {
            Assert.IsTrue(TestHelper.ArrayEquals(header.HashMerkleRoot,hashMerkleRoot),
                "HashMerkleRoot is different from the HashMerkleRoot in the block header");
        }
        
        private void TimeTest(BlockHeader header)
        {
            var time = DateTime.MinValue;
            header.SetTime(time);
            Assert.AreEqual(time, header.GetTime(),
                "Changing time failed");

            var time2 = DateTime.MaxValue;
            header.SetTime(time2);
            Assert.AreEqual(time2, header.GetTime(),
                "Changing time failed");
        }
        
        private void BitsTest(BlockHeader header, Target target)
        {
            Assert.IsTrue(TestHelper.ArrayEquals(header.GetTarget().ToBits(),target.ToBits()),
                "Target is different from the Target(Bits) in the block header");
        }
        
        private void NonceTest(BlockHeader header)
        {
            Assert.AreEqual(0, header.GetNonce(),
                "Nonce is different from the Nonce in the block header");

            var nonce2 = 500;
            header.SetNonce(nonce2);
            Assert.AreEqual(nonce2, header.GetNonce(),
                "Changing nonce failed");
        }
    }
}