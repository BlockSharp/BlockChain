using System;
using System.IO;
using System.Linq;
using System.Threading;
using BlockChain.Core.Block;
using BlockChain.Core.Test.Block;
using NUnit.Framework;

namespace BlockChain.Core.Test.BlockChain.Helpers
{
    public class AddHelpersTest
    {
        const string file = "blockchain3.db";
        const int TimeoutInSec = 30;
        [Test]
        public void AddTest()
        {
            if(File.Exists(file)) File.Delete(file);
            var blockChain = new BlockChain<TestBlockData>(file);
            
            var data = new TestBlockData("12345678910");
            var token = new CancellationTokenSource();
            token.CancelAfter(TimeSpan.FromSeconds(TimeoutInSec));
            
            blockChain.Add(data, token: token.Token).Wait();

            Assert.IsTrue(TestHelper.ArrayEquals(data.ToArray(),blockChain.First().GetData().ToArray()),"Block is not added to the blockchain, or could not read the blockchain");
            Assert.IsTrue(blockChain.First().IsValid(blockChain.First().GetBlockHeader().HashPrevBlock,blockChain.GetHashingAlgorithm()),"Added block is not valid");

        }
    }
}