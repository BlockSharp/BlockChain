using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using BlockChain.Core.Block;
using BlockChain.Core.Test.Block;
using NUnit.Framework;

namespace BlockChain.Core.Test.BlockChain
{
    public class BlockChainTest
    {
        private const string file = "blockchain.db";
        private const string file2 = "blockchain.db";
        private const int TimeoutInSec = 30;

        [Test]
        public void CreateFileTest()
        {
            if(File.Exists(file)) File.Delete(file);
            var blockChain = new BlockChain<TestBlockData>(file);
            
            Assert.IsTrue(File.Exists(file),"Blockchain did not create a new file");
            Assert.IsTrue(TestHelper.ArrayEquals(blockChain.Last().ToArray(),Constants.Genesis),"Blockchain did not add genesis to new file");
        }
        
        [Test]
        public void AddAndReadTest()
        {
            if(File.Exists(file2)) File.Delete(file2);
            var blockChain = new BlockChain<TestBlockData>(file2);

            using var sha256 = SHA256.Create();
            TestBlockData data = new TestBlockData("12345678910");
            var prevBlock = blockChain.First();
            var block = Block<TestBlockData>.Create(prevBlock.Hash(sha256),data,prevBlock.GetBlockHeader().GetTarget(), sha256);
            
            var token = new CancellationTokenSource();
            token.CancelAfter(TimeSpan.FromSeconds(TimeoutInSec));
            var m =block.Mine(token.Token);
            m.Wait();
            block = m.Result;
            
            blockChain.Add(block);

            Assert.IsTrue(TestHelper.ArrayEquals(block.ToArray(),blockChain.First().ToArray()),
                "Block is not added to the blockchain, or could not read the blockchain");
            Assert.IsTrue(blockChain.First().IsValid(prevBlock.Hash(sha256),sha256),"Added block is not valid");

        }
    }
}