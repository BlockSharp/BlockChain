using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Block;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Cryptography.Algorithms;
using CryptoChain.Core.Cryptography.Algorithms.ECC;
using CryptoChain.Core.Cryptography.Algorithms.ECC.ECDSA;
using CryptoChain.Core.Cryptography.Algorithms.RSA;
using CryptoChain.Core.Cryptography.Hashing;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Data;
using CryptoChain.Core.Transactions.Scripting;
using CryptoChain.Core.Transactions.Scripting.Interpreter;

namespace CryptoChain.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Full blockchain process
            
            //Sender
            var rsaKey = new RsaKey(512);
            var rsa = new CryptoRsa(rsaKey);
            var eccKey = new EccKey(Curve.Secp251K1);
            var ecdsa = new CryptoECDSA(eccKey);
            
            var coinbaseOut = ScriptBuilder.Lock_P2PKH(eccKey.PublicKey, Algorithm.ECDSA);
            var coinbase = Transaction.CoinBase(coinbaseOut, 0, 100);
            var anotherUnusedCoinbase = Transaction.CoinBase(coinbaseOut, 1, 150);

            var coinbaseIn = ScriptBuilder.Unlock_P2PKH(eccKey.PublicKey, ecdsa.Sign(coinbase.TxId));
            var secondOut = ScriptBuilder.Lock_P2PK(rsaKey.PublicKey, Algorithm.RSA);
            var second = new Transaction() {
                Inputs = new List<TxInput> {
                    new TxInput(coinbase.TxId, 0, coinbaseIn)
                },
                Outputs = new List<TxOutput> {
                    new TxOutput(100, secondOut)
                }
            };
            
            //Validate that script is working correctly. Clone must occur because else the scripts are emptied
            var interpreter = new ScriptInterpreter();
            Debug.Assert(interpreter.Execute(ref coinbase, coinbaseIn.Clone(), coinbaseOut.Clone()) == ExecutionResult.SUCCESS);

            //Create transaction list
            var transactions = new TransactionList {anotherUnusedCoinbase, coinbase, second};
            
            //Building a tree (just for fun, check if the root is correct)
            var cbIdx = transactions.IndexOf(coinbase);
            var tree = new MerkleTree(transactions);
            var proof = tree.GetProof(cbIdx);
            
            Console.WriteLine("Transactions: ");
            foreach (var t in transactions)
                Console.WriteLine(t.TxId.ToHexString());
            Console.WriteLine($"Proof for transaction coinbase ({coinbase.TxId.ToHexString()}):");
            while (proof.Any())
                Console.WriteLine(proof.Peek().hash.ToHexString() + " "+ proof.Dequeue().side);

            proof = tree.GetProof(cbIdx);
            Debug.Assert(tree.ValidateInclusionProof(coinbase.TxId, proof));
            proof = tree.GetProof(cbIdx);
            
            //And check if it is correct
            Debug.Assert(tree.MerkleRoot.SequenceEqual(transactions.MerkleRoot));

            //Packing transactions into block!
            var target = new Target(32);
            var block = await Miner.MineBlock(transactions, new byte[32], target);

            Debug.Assert(block != null, nameof(block) + " != null");
            Console.WriteLine("\n\n");
            Console.WriteLine(block.ToString());

            /*
            //https://en.bitcoin.it/wiki/List_of_address_prefixes
            //see length
            

            List<string> res = new List<string>(255);

            for (int i = 0; i < byte.MaxValue + 1; i++)
            {
                for (int j = 0; j < byte.MaxValue + 1; j++)
                {
                    res.Add(TestAddress(64, (byte)i, (byte)j));
                }

                char t = res.First()[0];
                if(res.All(x => x.StartsWith(t)))
                    Console.WriteLine($"prefix {i} starts with {t}");
                
                res.Clear();
            }*/
        }

        static string TestAddress(int count, byte prefix = 0, byte leading = 0)
        {
            byte[] buffer = new byte[1 + count + 4];
            buffer[0] = prefix;
            var data = FillRandom(count, leading);
            data.CopyTo(buffer, 1);
            var checksum = new Checksum(data, 4);
            checksum.Value.CopyTo(buffer, 1 + count);
            return Base58.Encode(buffer);
        }

        static byte[] FillRandom(int count, byte leading = 0)
        {
            byte[] buffer = new byte[count];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);
            buffer[0] = leading;
            return buffer;
        }
    }
}