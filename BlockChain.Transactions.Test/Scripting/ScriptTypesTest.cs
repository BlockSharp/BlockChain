using BlockChain.Transactions;
using BlockChain.Core.Cryptography;
using BlockChain.Core.Cryptography.RSA;
using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using BlockChain.Transactions.Scripting.Scripts;
using NUnit.Framework;
using System.Security.Cryptography;

namespace BlockChain.Core.Test.Script
{
    public class ScriptTypesTest
    {
        private Transaction transaction = new Transaction(new TxIn(new byte[32], 0), new TxOut(10));
        private RSAKey key = new RSAKey(512);

        [Test]
        public void TestP2PK()
        {
            CryptoRSA rsa = new CryptoRSA(key, true);
            Interpreter.Initialize();

            LockingScript ls = new LockingScript(key.publicKey);
            UnlockingScript us = new UnlockingScript(rsa.Sign(transaction.Hash()));

            //Put locking script in unlocking script
            us.InsertScript(ls);
            Assert.AreEqual(EXECUTION_RESULT.SUCCESS, us.Run(transaction), "Execution of P2PK script failed");
        }

        [Test]
        public void TestP2PKH()
        {
            CryptoRSA rsa = new CryptoRSA(key, true);
            Interpreter.Initialize();

            //Create address
            byte[] pubkeyhash;

            using (var rid = RIPEMD160.Create())
            using (var sha = SHA256.Create())
                pubkeyhash = rid.ComputeHash(sha.ComputeHash(key.publicKey));

            LockingScript ls = new LockingScript(BASE58.Encode(pubkeyhash));
            UnlockingScript us = new UnlockingScript(rsa.Sign(transaction.Hash()), key.publicKey);

            //Put locking script in unlocking script
            us.InsertScript(ls);
            Assert.AreEqual(EXECUTION_RESULT.SUCCESS, us.Run(transaction), "Execution of P2PK script failed");
        }

        [Test]
        public void TestP2SH_P2PK()
        {
            //Create p2pk
            CryptoRSA rsa = new CryptoRSA(key, true);
            Interpreter.Initialize();

            LockingScript ls = new LockingScript(key.publicKey);
            UnlockingScript us = new UnlockingScript(rsa.Sign(transaction.Hash()));

            //Create p2sh and insert p2pk in it
            LockingScript ls1 = new LockingScript(ls);
            UnlockingScript us1 = new UnlockingScript(ls, us);

            us1.InsertScript(ls1);

            Assert.AreEqual(EXECUTION_RESULT.SUCCESS, us1.Run(transaction), "Execution of P2SH script failed");
        }

        [Test]
        public void TestP2SH_P2PKH()
        {
            //Create p2pkh
            CryptoRSA rsa = new CryptoRSA(key, true);
            Interpreter.Initialize();

            //Create address
            byte[] pubkeyhash;

            using (var rid = RIPEMD160.Create())
            using (var sha = SHA256.Create())
                pubkeyhash = rid.ComputeHash(sha.ComputeHash(key.publicKey));

            LockingScript ls = new LockingScript(BASE58.Encode(pubkeyhash));
            UnlockingScript us = new UnlockingScript(rsa.Sign(transaction.Hash()), key.publicKey);

            //Create p2sh and insert p2pkh in it
            LockingScript ls1 = new LockingScript(ls);
            UnlockingScript us1 = new UnlockingScript(ls, us);

            us1.InsertScript(ls1);

            Assert.AreEqual(EXECUTION_RESULT.SUCCESS, us1.Run(transaction), "Execution of P2SH script failed");
        }
    }
}
