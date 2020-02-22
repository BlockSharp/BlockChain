using System;
using System.Security.Cryptography;
using BlockChain.Core;
using BlockChain.Core.Block;

namespace BlockChain.Cli
{
    public static class TestHelper
    {
        public static Random rnd = new Random();

        public static Block<ExampleBlockData> GetBlock()
        {
            var data = GetRandomData();
            return new Block<ExampleBlockData>(GetRandomBlockHeader(data),data);
        }
        public static ExampleBlockData GetRandomData()
            => new ExampleBlockData(Convert.ToBase64String(GetRandomHash(rnd.Next(1,1000))));
        public static BlockHeader GetRandomBlockHeader()
            =>new BlockHeader(GetRandomHash(),GetRandomHash(),new Target(new byte[] {29,100,255,255}).ToBits());

        public static BlockHeader GetRandomBlockHeader(IBlockData data)
        {
            using var sha256 = SHA256.Create();
            return new BlockHeader(GetRandomHash(),data.GetHashMerkleRoot(sha256),new Target(new byte[] {29,75,0,0}).ToBits());
        }
        public static byte[] GetRandomHash(int size = 32)
        {
            byte[] b = new byte[size];
            rnd.NextBytes(b);
            return b;
        }
    }
}