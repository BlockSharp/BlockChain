using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CryptoChain.Core.Cryptography.Hashing;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Data;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Transaction coinBase1 = Transaction.CoinBase(new Script(), 1, 50);
            Transaction coinBase2 = Transaction.CoinBase(new Script(), 2, 50);
            Transaction coinBase3 = Transaction.CoinBase(new Script(), 3, 50);
            Transaction coinBase4 = Transaction.CoinBase(new Script(), 4, 50);
            Transaction coinBase5 = Transaction.CoinBase(new Script(), 5, 50);
            Transaction coinBase6 = Transaction.CoinBase(new Script(), 6, 50);
            Transaction coinBase7 = Transaction.CoinBase(new Script(), 7, 50);
            Transaction coinBase8 = Transaction.CoinBase(new Script(), 8, 50);

            MerkleTree.Node n1 = new MerkleTree.Node(coinBase1.TxId);
            MerkleTree.Node n2 = new MerkleTree.Node(coinBase2.TxId);
            MerkleTree.Node n3 = new MerkleTree.Node(coinBase3.TxId);
            MerkleTree.Node n4 = new MerkleTree.Node(coinBase4.TxId);
            MerkleTree.Node n5 = new MerkleTree.Node(coinBase5.TxId);
            MerkleTree.Node n6 = new MerkleTree.Node(coinBase6.TxId);
            MerkleTree.Node n7 = new MerkleTree.Node(coinBase7.TxId);
            MerkleTree.Node n8 = new MerkleTree.Node(coinBase8.TxId);
            
            /*Console.WriteLine("1 - 8:");
            Console.WriteLine(Convert.ToBase64String(n1.Hash));
            Console.WriteLine(Convert.ToBase64String(n2.Hash));
            Console.WriteLine(Convert.ToBase64String(n3.Hash));
            Console.WriteLine(Convert.ToBase64String(n4.Hash));
            Console.WriteLine(Convert.ToBase64String(n5.Hash));
            Console.WriteLine(Convert.ToBase64String(n6.Hash));
            Console.WriteLine(Convert.ToBase64String(n7.Hash));
            Console.WriteLine(Convert.ToBase64String(n8.Hash));*/
            
            
            MerkleTree.Node n12 = new MerkleTree.Node(n1, n2);
            MerkleTree.Node n34 = new MerkleTree.Node(n3, n4);
            MerkleTree.Node n56 = new MerkleTree.Node(n5, n6);
            MerkleTree.Node n78 = new MerkleTree.Node(n7, n8);
            
            /*Console.WriteLine("12 - 78:");
            Console.WriteLine(Convert.ToBase64String(n12.Hash));
            Console.WriteLine(Convert.ToBase64String(n34.Hash));
            Console.WriteLine(Convert.ToBase64String(n56.Hash));
            Console.WriteLine(Convert.ToBase64String(n78.Hash));*/

            MerkleTree.Node n1234 = new MerkleTree.Node(n12, n34);
            MerkleTree.Node n5678 = new MerkleTree.Node(n56, n78);
            
            /*Console.WriteLine("1234 & 5678:");
            Console.WriteLine(Convert.ToBase64String(n1234.Hash));
            Console.WriteLine(Convert.ToBase64String(n5678.Hash));*/

            MerkleTree.Node n12345678 = new MerkleTree.Node(n1234, n5678);
            /*Console.WriteLine("12345678:");
            Console.WriteLine(Convert.ToBase64String(n12345678.Hash));*/

            var tl = new TransactionList() {coinBase1, coinBase2, coinBase3, coinBase4, coinBase5, coinBase6, coinBase7, coinBase8};
            //Console.WriteLine(Convert.ToBase64String(tl.MerkleRoot));
            

            var tree = new MerkleTree(tl);
            //Console.WriteLine(Convert.ToBase64String(tree.Root.Hash));
            
            Console.WriteLine(tree.Root.LeafCount);
            Console.WriteLine(tree.Root.Count);

            Console.WriteLine("\n");

            
            Console.WriteLine("Checking: "+Convert.ToBase64String(coinBase3.TxId));

            Console.WriteLine("");

            var options = new JsonSerializerOptions {Converters = {new MerkleProofQueueConverter()}};
            
            var test = tree.GetProof(5);
            var json = JsonSerializer.Serialize(test, options);
            
            Console.WriteLine(test.Count);
            
            Console.WriteLine(json);

            var obj = JsonSerializer.Deserialize<Queue<(byte[], bool)>>(json, options);


            MerkleTree newTree = new MerkleTree(tree.MerkleRoot);
            
            Console.WriteLine(newTree.ValidateInclusionProof(coinBase6.TxId, obj));
            
        }
    }
}