using System;
using System.Collections.Generic;
using System.Text;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Transactions.Scripting
{
    public class ScriptBuilder : Script
    {
        /// <summary>
        /// Pushes x amount of bytes to the script. Automatically identifies the OP_PUSHDATA code
        /// for an optimized, dynamic script size
        /// </summary>
        /// <param name="bytes"></param>
        public void PushData(byte[] bytes)
        {
            if (bytes.Length <= 255)
            {
                Add(Opcode.PUSHDATA_1);
                Add((byte)bytes.Length);
            }
            else if(bytes.Length <= 65535)
            {
                Add(Opcode.PUSHDATA_2);
                AddRange(BitConverter.GetBytes((ushort)bytes.Length));
            }
            else
            {
                Add(Opcode.PUSHDATA_4);
                AddRange(BitConverter.GetBytes(bytes.Length));
            }
            
            AddRange(bytes);
        }
        
        
        /// <summary>
        /// Create a locking pay to public key script
        /// </summary>
        /// <param name="publicKey">The public key</param>
        /// <returns>P2PK Locking script</returns>
        public static IScript Lock_P2PK(byte[] publicKey)
        {
            var script = new ScriptBuilder();
            script.PushData(publicKey);
            script.Add(Opcode.CHECKSIG);
            return script;
        }
        
        /// <summary>
        /// Create a unlocking pay to public key script by providing a signature
        /// </summary>
        /// <param name="signature">The signature</param>
        /// <returns>A unlocking P2PK script</returns>
        public static IScript Unlock_P2PK(byte[] signature)
        {
            var script = new ScriptBuilder();
            script.PushData(signature);
            return script;
        }

        /// <summary>
        /// Create a locking pay to public key hash script
        /// </summary>
        /// <param name="publicKey">The public key to be hashed</param>
        /// <returns>The locking P2PKH script</returns>
        public static IScript Lock_P2PKH(byte[] publicKey)
        {
            var script = new ScriptBuilder();
            script.Add(Opcode.DUP, Opcode.HASH160);
            script.PushData(Cryptography.Hashing.Hash.HASH_160(publicKey));
            script.Add(Opcode.EQ_VERIFY, Opcode.CHECKSIG);
            return script;
        }
        
        /// <summary>
        /// Unlock a pay to public key hash script
        /// </summary>
        /// <param name="publicKey">The public key</param>
        /// <param name="signature">A signature for the public key (from the transaction)</param>
        /// <returns>A unlocking P2PKH script</returns>
        public static IScript Unlock_P2PKH(byte[] publicKey, byte[] signature)
        {
            var script = new ScriptBuilder();
            script.PushData(signature);
            script.PushData(publicKey);
            return script;
        }

        /// <summary>
        /// Create a locking pay to multi-signature script
        /// </summary>
        /// <param name="requiredSignatureCount">The minimum amount of signatures to be provided to unlock this script</param>
        /// <param name="publicKeys">The public keys</param>
        /// <returns>A P2MS locking script</returns>
        public static IScript Lock_P2MS(byte requiredSignatureCount, params byte[][] publicKeys)
        {
            if(publicKeys.Length > 16)
                throw new ArgumentException("The maximum amount of pubKeys for a P2MS script is 16");
            if(requiredSignatureCount > publicKeys.Length || requiredSignatureCount <= 0)
                throw new ArgumentException("The amount of required signatures must match the amount of pubkeys or less. Must be at least 1");
            
            var script = new ScriptBuilder();
            script.Add((Opcode)(requiredSignatureCount + 1)); //OP_x
            
            foreach (var pk in publicKeys) 
                script.PushData(pk);
            
            script.Add((Opcode)(publicKeys.Length + 1)); //OP_x
            script.Add(Opcode.CHECKMULTISIG);
            return script;
        }

        /// <summary>
        /// Create a unlocking pay to multi-signature script
        /// </summary>
        /// <param name="signatures">The signatures to provide</param>
        /// <returns>A unlocking P2MS script</returns>
        public static IScript Unlock_P2MS(params byte[][] signatures)
        {
            var script = new ScriptBuilder();
            foreach (var sig in signatures)
                script.PushData(sig);
            script.Add((Opcode)(signatures.Length + 1)); //OP_x
            return script;
        }

        /// <summary>
        /// Create a new locking pay to script hash script
        /// </summary>
        /// <param name="scriptHash">The hashed script (HASH160)</param>
        /// <returns></returns>
        public static IScript Lock_P2SH(byte[] scriptHash)
        {
            var script = new ScriptBuilder();
            script.Add(Opcode.DUP);
            script.Add(Opcode.HASH160);
            script.PushData(scriptHash);
            script.Add(Opcode.EQ_VERIFY);
            script.Add(Opcode.EVAL_SCRIPT);
            return script;
        }

        /// <summary>
        /// The unlocking pay to script hash script
        /// </summary>
        /// <param name="scripts">The script to unlock the locking script and some scripts to be executed after it</param>
        /// <returns>A unlocking P2SH script</returns>
        public static IScript Unlock_P2SH(params IScript[] scripts)
        {
            var s = new Script();
            foreach (var i in scripts)
                s.Add(i);
            var script = new ScriptBuilder();
            script.PushData(s.Serialize());
            return script;
        }
    }
}