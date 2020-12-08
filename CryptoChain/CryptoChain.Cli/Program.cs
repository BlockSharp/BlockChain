using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using CryptoChain.Core;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Block;
using CryptoChain.Core.Cryptography;
using CryptoChain.Core.Cryptography.RSA;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Data;
using CryptoChain.Core.Transactions.Scripting;
using CryptoChain.Core.Transactions.Scripting.Interpreter;

namespace CryptoChain.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            //Pay 2 script hash with pay 2 multisig inside
            Transaction t = new Transaction();
            
            var key = new RsaKey(512);
            var key2 = new RsaKey();
            var rsa = new CryptoRsa(key);
            var rsa2 = new CryptoRsa(key2);

            byte[] pubkey1 = key.ToArray(false);
            byte[] pubkey2 = key2.ToArray(false);

            var ls1 = ScriptBuilder.Lock_P2MS(2, pubkey1, pubkey2);
            var us1 = ScriptBuilder.Unlock_P2MS(rsa.Sign(t.Hash()), rsa2.Sign(t.Hash()));
            
            var script = new Script(us1, ls1);

            var ls = ScriptBuilder.Lock_P2SH(script.Hash());
            var _us = ScriptBuilder.Unlock_P2SH(us1, ls1);
            Script us = new Script();
            //us.Add(Opcode.OP_1); //uncomment this to see the script beeing invalid
            us.Add(_us);

            var i = new ScriptInterpreter();
            Console.WriteLine(i.Execute(ref t, us, ls));
        }
    }
}