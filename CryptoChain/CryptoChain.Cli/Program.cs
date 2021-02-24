using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain.Storage;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Cli
{
    static class Program
    {
        public async static Task Main(string[] args)
        {
            FileBlockStore store = new FileBlockStore("/home/maurict/Desktop/blocks", false);
            await store.Index();
            //mine some blocks and put them in the store

            bool CREATE = false;
            int AMOUNT_OF_BLOCKS = 5000;

            if (CREATE)
            {
                Target target = new Target(31); //EZ target (1 zero place, FF FF FF)
                var random = new RandomGenerator();
                byte[] prevHash = new byte[32];
                for (int i = 0; i < AMOUNT_OF_BLOCKS; i++)
                {
                    var header = new BlockHeader(prevHash, random.GetBytes(32), target, 255);
                    var block = await Miner.CreateBlock(header, random.GetBytes(100000), BlockDataIdentifier.RAW_DATA);
                    Debug.Assert(block != null);
                    await store.AddBlock(block);
                    prevHash = block.Hash;
                }
                
                await store.Index();
            }

            Console.WriteLine(store);
            Console.WriteLine(store.GetHeight(store.GetBlock(store.BlockHeight).Result.Hash));
        }
    }
}