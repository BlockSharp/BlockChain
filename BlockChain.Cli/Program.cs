using BlockChain.Transactions;
using BlockChain.Transactions.Cryptography;
using BlockChain.Transactions.Cryptography.RSA;
using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using BlockChain.Transactions.Scripting.Scripts;
using System;

namespace BlockChain.Cli
{
    class Program
    {
        static void Main()
        {
            RSAKey key = new RSAKey(512);
            CryptoRSA rsa = new CryptoRSA(key, true);
            Interpreter.Initialize();

            Transaction t = new Transaction(new TxIn(new byte[32], 0), new TxOut(5));

            byte[] pubkeyhash;

            using (var rid = RIPEMD160.Create())
                pubkeyhash = rid.ComputeHash(key.publicKey);

            LockingScript ls = new LockingScript(key.publicKey);
            UnlockingScript us = new UnlockingScript(rsa.Sign(t.Hash()), key.publicKey);

            us.InsertScript(ls);
            Console.WriteLine(us.Run(t).ToString());

            //var sw = Stopwatch.StartNew();
            //for (int i = 0; i < 1000000; i++) script.Run();
            //Console.WriteLine(sw.ElapsedMilliseconds);
            /*
            var blockChain = new BlockChain<ExampleBlockData>("blockchain.db");
            //for(int i = 0;i<5000000;i++) 
            //    blockChain.Add(new Block<ExampleBlockData>(Convert.FromBase64String("AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz5B3jtcrfXCB4AAP/yBwEAMTIzNDY1NDIzNTE0M2UAAAA=")));

            var sw = Stopwatch.StartNew();
            int x = 0;
            //foreach (var b in blockChain) x++;

            //var y = 
            //    blockChain.ToList();
            //Console.WriteLine(y.Count);
            byte[] file = File.ReadAllBytes("blockchain.db");

            Console.WriteLine($"Read {x} blocks in {sw.ElapsedMilliseconds} milliseconds");*/
        }
    }
}