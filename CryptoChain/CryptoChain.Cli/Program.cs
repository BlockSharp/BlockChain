using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
            var key1 = new RsaKey(Convert.FromBase64String("BwIAAACkAABSU0EyAAIAAAEAAQDbpuGYbUNxUtG18M1JehM3kdt5ntTMnnXZ7U0gLeQaPDw4Pv+f2rikjss8vIEFT4DAHNXRg5fv3d0AwJIUdyac67HaWbluTC5olGTE/1SEEFGzCoeJ9HxpF37nXOIVlszRsqNvd8AxQfB0KGYLQnYE/YCEsEpBmH3E4RwRTkhkw1WSzE6JcMtsGy/PF33GOlTlu+Ir8rsH3ps2s/PN5iko0YknDJbrwoaqVc6yvMR5BqsvWtI2o2LJVuVYAVZWcB/5rUywRlH3QO7/emXJs/gmJc/wgdOONADsZcS+djyUO6EYRQyyRWWZdMDyoebUUqf+K7nv5GnOSMmOFLMkwz84IMGIJ/QiJGVkb1mnCNK0Lwigg0H1ssrz/8Z6iysoa4M="));
            var key2 = new RsaKey(Convert.FromBase64String("BwIAAACkAABSU0EyAAIAAAEAAQDNv2YjL4w+LuzCcwFhyvFxUfiXzZCD9WOshtS/OHArESPRTo6OvU31gU/ZW59B11O5FTOcVQLx1hoJwW5Jm4y2FfG8CJk2M6+oCjiZ8JRtwJdAYX+6OUWLDOCKkB/vct/ZEUxphbSxWFyF+Aq6hwj+L/3adpZBtgHq1mTD/2Yk0YVGqF06PI3D3sXKLonhiJBWkzvXMX9IP5ZE5by2KB8miRYPBJbhFsQ0efWAPhZ2BPpDG5fdlQUFc4wdhjosl3fPLAZKlkufFTeO0TU81PBccDmVfec2hEEk0NF7Pd0g2aGk4WqpmXHpiF8XCRXgaplBA9bG1X8zNwXvvU+6bW8ne33r3e2aw88SpzkoeT0i3CRhcfO4tG4bRCQB5JSBK2M="));
            var rsa1 = new CryptoRsa(key1);
            var rsa2 = new CryptoRsa(key2);
            
            using var blockStore = new FileBlockStore("/home/maurict/Desktop/blocks");
            using var transactionStore = new TransactionStore(blockStore, "/home/maurict/Desktop/blocks");

            //await blockStore.CreateIndexes();
            //await transactionStore.CreateTxIndexes();
            //await transactionStore.CreateUtxoIndexes();
            
            /*
            var cb = Transaction.CoinBase(
                ScriptBuilder.Lock_P2PK(key1.PublicKey, Algorithm.RSA), blockStore.BlockHeight, 1000);
            var coinbase = await Miner.CreateBlock(new TransactionList() {cb}, new byte[32], new Target(30));
            await blockStore.AddBlocks(coinbase);

            var tx1 = new Transaction();
            tx1.Inputs.Add(new TxInput(coinbase.Transactions.First().TxId, 0, ScriptBuilder.Unlock_P2PK(rsa1.Sign(coinbase.Transactions.First().TxId))));
            tx1.Outputs.Add(new TxOutput(250, ScriptBuilder.Lock_P2PKH(key1.PublicKey, Algorithm.RSA)));
            tx1.Outputs.Add(new TxOutput(750, ScriptBuilder.Lock_P2PKH(key2.PublicKey, Algorithm.RSA)));

            var tx2 = Transaction.CoinBase(
                ScriptBuilder.Lock_P2PK(key2.PublicKey, Algorithm.RSA), blockStore.BlockHeight, 1000);
            var block2 = await Miner.CreateBlock(new TransactionList() {tx1, tx2}, coinbase.Hash, new Target(30));
            await blockStore.AddBlocks(block2);

            var tx3 = new Transaction();
            tx3.Inputs.Add(new TxInput(block2.Transactions[0].TxId, 0, ScriptBuilder.Unlock_P2PKH(key1.PublicKey, rsa1.Sign(block2.Transactions[0].TxId)))); //250
            tx3.Inputs.Add(new TxInput(block2.Transactions[0].TxId, 1, ScriptBuilder.Unlock_P2PKH(key2.PublicKey, rsa2.Sign(block2.Transactions[0].TxId)))); //750
            tx3.Inputs.Add(new TxInput(block2.Transactions[1].TxId, 0, ScriptBuilder.Unlock_P2PK(rsa2.Sign(block2.Transactions[1].TxId)))); //1000
            tx3.Outputs.Add(new TxOutput(2000, ScriptBuilder.Lock_P2PKH(key2.PublicKey, Algorithm.RSA)));
            var tx4 = Transaction.CoinBase(ScriptBuilder.Lock_P2PKH(key2.PublicKey, Algorithm.RSA),
                blockStore.BlockHeight, 500);
            var block3 = await Miner.CreateBlock(new TransactionList() {tx3, tx4}, block2.Hash, new Target(30));
            await blockStore.AddBlocks(block3);
            */

        }

        static void fill(byte[] buffer)
        {
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
        }
    }
}