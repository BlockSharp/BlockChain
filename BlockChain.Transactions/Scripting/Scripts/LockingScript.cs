using BlockChain.Transactions.Cryptography;
using BlockChain.Transactions.Scripting.Enums;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace BlockChain.Transactions.Scripting.Scripts
{
    public class LockingScript : Script
    {
        /// <summary>
        /// This will initialize the script as an P2PK-locking script (Pay To Private Key)
        /// </summary>
        /// <param name="publicKey">The receiver's public key, byte[84]</param>
        public LockingScript(byte[] publicKey) : base(SCRIPTTYPE.LOCK_P2PK)
        {
            if (publicKey.Length != PubKeySize) throw new ArgumentException("Public key length is invalid. Please provide a key with keysize=512");
            
            this.Add(OPCODE.PUBKEY);
            this.AddRange(publicKey);
            this.Add(OPCODE.CHECKSIG); //OPCODE = Check public key with signature
        }

        /// <summary>
        /// This will initialize the script as an P2PKH-locking script (Pay To Private Key Hash)
        /// </summary>
        /// <param name="address">The hashed and BASE58 encoded address</param>
        public LockingScript(string address) : base(SCRIPTTYPE.LOCK_P2PKH)
        {
            if (address.Length < 20) throw new ArgumentException("Address is invalid: too short");
            if (address.Length > 255) throw new ArgumentException("Address is invalid: too big");

            byte[] addressHash = BASE58.Decode(address);
            if (addressHash.Length != 20) throw new ArgumentException("Invalid address. Address byte[] size must be 20 (HASH160)");

            this.Add(OPCODE.DUP); //OPCODE = Duplicate the original public key, first instruction
            this.Add(OPCODE.HASH160); //OPCODE = Hash public key
            this.Add(OPCODE.PUBKEY_HASH);
            this.AddRange(addressHash); //Hashed publicKey
            this.Add(OPCODE.EQ_VERIFY); //OPCODE = Check if equal: address and hashed public key
            this.Add(OPCODE.CHECKSIG); //Validate signature with the provided public key
        }

        /// <summary>
        /// This will initialize the script as an P2SH-locking script (Pay To Script Hash)
        /// </summary>
        /// <param name="script">The script that will be hashed</param>
        public LockingScript(Script script) : base(SCRIPTTYPE.LOCK_P2SH)
        {
            using(var hash160 = RIPEMD160.Create())
            {
                byte[] scriptHash = hash160.ComputeHash(script.ToArray());
                if (scriptHash.Length > 255) throw new ArgumentException("Error: hash is too big.");

                this.Add(OPCODE.DUP); //Duplicate unlocking script data
                this.Add(OPCODE.RIPEMD160); //Hash given script
                this.PushSizeAndCode(scriptHash.Length);
                this.AddRange(scriptHash); //Script hash
                this.Add(OPCODE.EQ_VERIFY); //Verify if script hash matches created hash
                this.Add(OPCODE.EVAL_SCRIPT); //Execute (duplicated in begin) script
            }
        }

        /// <summary>
        /// Get public key from script
        /// </summary>
        /// <returns>byte[84]</returns>
        public byte[] GetPublicKey()
        {
            if (this.Type != SCRIPTTYPE.LOCK_P2PK)
                throw new ArgumentException($"This type of script ({this.Type.ToString()}) does commonly not contain a public key");

            byte size = Script.PubKeySize;
            return this.Skip(2).Take(size).ToArray();
        }

        /// <summary>
        /// Get public key hash (or address)
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetPublicKeyHash()
        {
            if (this.Type != SCRIPTTYPE.LOCK_P2PKH)
                throw new ArgumentException($"This type of script ({this.Type.ToString()}) does commonly not contain a public key hash");

            byte size = this.Skip(3).Take(1).First();
            return this.Skip(4).Take(size).ToArray();
        }

    }
}
