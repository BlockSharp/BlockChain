using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain;
using CryptoChain.Core.Chain.Storage;
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
            var spp = new SeededPrimePair(
                Convert.FromBase64String("AAQAAPDTAADyDQEAUGV0cmEgaXMgZGUgYWxsZXJsaWVmc3RlIQ=="));
            var key = spp.ToRsaKey();
            var rsa = new CryptoRsa(key);
            
            return;

            var key1 = new RsaKey(512);
            var key2 = new RsaKey(512);
            var rsa1 = new CryptoRsa(key1);
            var rsa2 = new CryptoRsa(key2);
            
            IBlockStore blockStore = new FileBlockStore("/home/maurict/Desktop/blocks");
            ITransactionStore transactionStore = new TransactionStore(ref blockStore);
            Console.WriteLine(blockStore);
            
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


            var latest = await blockStore.GetBlock(blockStore.BlockHeight);
            var latestTx = latest.Transactions[0];

            var newTx = new Transaction();
            newTx.Inputs.Add(new TxInput(latestTx.TxId, 0, ScriptBuilder.Unlock_P2PKH(key2.PublicKey, rsa2.Sign(newTx.TxId))));
            newTx.Outputs.Add(new TxOutput(2001, new Script()));

            var finished = new Transaction(newTx.Serialize());

            var bc = new Blockchain(blockStore);
            Console.WriteLine(await bc.Validate(finished));
        }
    }
}