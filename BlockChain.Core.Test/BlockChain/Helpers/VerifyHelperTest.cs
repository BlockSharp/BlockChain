using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BlockChain.Core.Block;
using BlockChain.Core.Test.Block;
using NUnit.Framework;

namespace BlockChain.Core.Test.BlockChain.Helpers
{
    public class VerifyHelperTest
    {
        const string file = "blockchain4.db";
        const int TimeoutInSec = 30;
        
        [Test]
        public async Task VerifyTest()
        {
            if(File.Exists(file)) File.Delete(file);
            var blockChain = new BlockChain<TestBlockData>(file);
            
            var data = new[] { new TestBlockData("12345678910"),new TestBlockData("12345678911230"),new TestBlockData("123455678910"),new TestBlockData("1234567891120") };
            var token = new CancellationTokenSource();
            token.CancelAfter(TimeSpan.FromSeconds(TimeoutInSec));
            foreach (var d in data) await blockChain.Add(d, token: token.Token);
            
            //Check blockchain
            var x = await blockChain.Verify(token.Token);
            Assert.IsTrue(x,"Valid blockchain was marked invalid");
            
            //Make blockchain invalid
            MakeBlockChainInvalid(blockChain);
            foreach (var d in data) await blockChain.Add(d, token: token.Token); //Add some random blocks
            
            //Check blockchain again
            x = await blockChain.Verify(token.Token);
            Assert.IsFalse(x,"Invalid blockchain was marked valid");
        }

        private void MakeBlockChainInvalid(BlockChain<TestBlockData> blockChain)
        {
            var blockWithInvalidMerkleRoot = new Block<TestBlockData>(Convert.FromBase64String(
                "AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHgCynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz5B3jtcrfXCB4AAP/yBwEAMTIzNDY1NDIzNTE0M2UAAAA="));
            byte[] data = blockWithInvalidMerkleRoot.ToArray();
            using var stream = new FileStream(file, FileMode.Append);
            stream.Write(data, 0, data.Length);
        }
    }
}