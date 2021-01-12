using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using CryptoChain.Core.Block;
using CryptoChain.Core.Block.Helpers;
using CryptoChain.Core.BlockChain;
using CryptoChain.Core.Tests.Block.BlockData;
using NUnit.Framework;

namespace CryptoChain.Core.Tests.BlockChain
{
    public class BlockChainTest
    {
        private const string File = "blockchain.db";
        private const string File2 = "blockchain2.db";
        private const int TimeoutInSec = 30;

        [Test]
        public void CreateFileTest()
        {
            if(System.IO.File.Exists(File)) System.IO.File.Delete(File);
            if(System.IO.File.Exists($"{File}.h")) System.IO.File.Delete($"{File}.h");

            var blockChain = new BlockChain<TestBlockData>(File);
            
            Assert.IsTrue(System.IO.File.Exists(File),"Blockchain did not create a new file");
            Assert.IsTrue(TestHelper.ArrayEquals(blockChain.Last().ToArray(),Constants.Genesis),"Blockchain did not add genesis to new file");
        }
        
        [Test]
        public void AddAndReadTest()
        {
            if(System.IO.File.Exists(File2)) System.IO.File.Delete(File2);
            if(System.IO.File.Exists($"{File2}.h")) System.IO.File.Delete($"{File2}.h");
            var blockChain = new BlockChain<TestBlockData>(File2);
            

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