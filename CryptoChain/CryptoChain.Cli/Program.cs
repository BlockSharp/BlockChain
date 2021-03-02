using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain;
using CryptoChain.Core.Chain.Storage;
using CryptoChain.Core.Chain.Storage.Indexes;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.RSA;
using CryptoChain.Core.Cryptography.Hashing;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Data;
using CryptoChain.Core.Transactions.Scripting;
using CryptoChain.Core.Transactions.Scripting.Interpreter;

namespace CryptoChain.CLI
{
    static class Program
    {
        public static async Task Main(string[] args)
        {
            
            var idx = new BlockIndexes("/home/maurict/Desktop/blocks/blocks.idx");
            var fsb = new FileBlockStore("/home/maurict/Desktop/blocks", idx , false);
            
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine(fsb.BlockHeight);


            using var e = fsb.All().GetEnumerator();
            while (e.MoveNext()) //Memory problem!!!
            {
                //do sth
            }
            e.Dispose();
            
            Console.WriteLine(sw.Elapsed);


            Console.WriteLine("Getting block");
            Block latest = await fsb.GetBlock(fsb.BlockHeight);
            Console.WriteLine("Done!");

            return;
            byte[] data = new byte[1000];
            List<Block> blocks = new List<Block>(10000);

            for (int i = 0; i < 1000; i++)
            {
                sw.Restart();
                for (int j = 0; j < 10000; j++)
                {
                    fill(data);
                    var b = await Miner.CreateBlock(new BlockHeader(latest.Hash, Hash.SHA_256(data), new Target()), data, BlockDataIdentifier.RAW_DATA);
                    blocks.Add(b);
                    latest = b;
                }

                Console.WriteLine("Writing...");
                await fsb.AddBlocks(blocks.ToArray());
                blocks.Clear();
                Console.WriteLine("Written 10000 blocks "+sw.Elapsed);
            }
            
            return;
            

            return;
            var spp = new SeededPrimePair(
                Convert.FromBase64String("AAQAAPDTAADyDQEAUGV0cmEgaXMgZGUgYWxsZXJsaWVmc3RlIQ=="));
            var key = spp.ToRsaKey();
            var rsa = new CryptoRsa(key);
            
            return;

            var key1 = new RsaKey(512);
            var key2 = new RsaKey(512);
            var rsa1 = new CryptoRsa(key1);
            var rsa2 = new CryptoRsa(key2);
            
            
            /*
            var cb = Transaction.CoinBase(
                ScriptBuilder.Lock_P2PK(key1.PublicKey, Algorithm.RSA), blockStore.BlockHeight, 1000);
            var coinbase = await Miner.CreateBlock(new TransactionList() {cb}, new byte[32], new Target(30));
            //await blockStore.AddBlock(coinbase);

            var tx1 = new Transaction();
            tx1.Inputs.Add(new TxInput(coinbase.Transactions.First().TxId, 0, ScriptBuilder.Unlock_P2PK(rsa1.Sign(coinbase.Transactions.First().TxId))));
            tx1.Outputs.Add(new TxOutput(250, ScriptBuilder.Lock_P2PKH(key1.PublicKey, Algorithm.RSA)));
            tx1.Outputs.Add(new TxOutput(750, ScriptBuilder.Lock_P2PKH(key2.PublicKey, Algorithm.RSA)));

            var tx2 = Transaction.CoinBase(
                ScriptBuilder.Lock_P2PK(key2.PublicKey, Algorithm.RSA), blockStore.BlockHeight, 1000);
            var block2 = await Miner.CreateBlock(new TransactionList() {tx1, tx2}, coinbase.Hash, new Target(30));
            //await blockStore.AddBlock(block2);

            var tx3 = new Transaction();
            tx3.Inputs.Add(new TxInput(block2.Transactions[0].TxId, 0, ScriptBuilder.Unlock_P2PKH(key1.PublicKey, rsa1.Sign(block2.Transactions[0].TxId)))); //250
            tx3.Inputs.Add(new TxInput(block2.Transactions[0].TxId, 1, ScriptBuilder.Unlock_P2PKH(key2.PublicKey, rsa2.Sign(block2.Transactions[0].TxId)))); //750
            tx3.Inputs.Add(new TxInput(block2.Transactions[1].TxId, 0, ScriptBuilder.Unlock_P2PK(rsa2.Sign(block2.Transactions[1].TxId)))); //1000
            tx3.Outputs.Add(new TxOutput(2000, ScriptBuilder.Lock_P2PKH(key2.PublicKey, Algorithm.RSA)));
            var tx4 = Transaction.CoinBase(ScriptBuilder.Lock_P2PKH(key2.PublicKey, Algorithm.RSA),
                blockStore.BlockHeight, 500);
            var block3 = await Miner.CreateBlock(new TransactionList() {tx3, tx4}, block2.Hash, new Target(30));
            //await blockStore.AddBlock(block3);*/

        }

        static void fill(byte[] buffer)
        {
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
        }
    }
}