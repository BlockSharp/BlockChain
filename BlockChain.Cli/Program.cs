using BlockChain.Transactions;
using BlockChain.Transactions.Cryptography;
using BlockChain.Transactions.Cryptography.RSA;
using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using BlockChain.Transactions.Scripting.Scripts;
using System;
using System.Security.Cryptography;

namespace BlockChain.Cli
{
    class Program
    {
        static void Main()
        {
            //Dit demonstreerd een P2PKH script
            RSAKey receiver = new RSAKey(512);
            CryptoRSA rsa = new CryptoRSA(receiver, true);

            //Create address
            byte[] pubkeyhash;

            using (var rid = RIPEMD160.Create())
            using (var sha = SHA256.Create())
                pubkeyhash = rid.ComputeHash(sha.ComputeHash(receiver.publicKey));

            LockingScript ls = new LockingScript(BASE58.Encode(pubkeyhash));
            Transaction t = new Transaction(new TxIn(new byte[32], 0), new TxOut(0, ls));

            UnlockingScript us = new UnlockingScript(rsa.Sign(t.Hash()), receiver.publicKey);
            Transaction t2 = new Transaction(new TxIn(t.Hash(), 0, us), new TxOut(0));


            //Vervolgens wordt dit in een P2SH script gestopt. Let goed op de uitvoer
            LockingScript ls1 = new LockingScript(ls);
            Script us1 = new UnlockingScript(ls, us);

            us1.InsertScript(ls1);
            Console.WriteLine(us1.Run(t));



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