using BlockChain.Transactions.Scripting.Enums;
using System;

namespace BlockChain.Transactions.Scripting.Scripts
{
    public class UnlockingScript : Script
    {
        /// <summary>
        /// This will initialize the script as an P2PK-unlocking script (Pay To Public Key)
        /// </summary>
        /// <param name="signature">The signature of the previous owner, byte[64]</param>
        public UnlockingScript(byte[] signature) : base(SCRIPTTYPE.UNLOCK_P2PK)
        {
            if (signature.Length != SignatureSize) throw new ArgumentException("Signature size is invalid");

            this.Add(OPCODE.SIGNATURE);
            this.AddRange(signature);
        }

        /// <summary>
        /// This will initialize the script as an P2PKH unlocking script (Pay To Public Key Hash)
        /// </summary>
        /// <param name="signature">Your signature byte[64]</param>
        /// <param name="publicKey">Your public key byte[84]</param>
        public UnlockingScript(byte[] signature, byte[] publicKey) : base(SCRIPTTYPE.UNLOCK_P2PKH)
        {
            if (publicKey.Length != PubKeySize) throw new ArgumentException("Public key size is invalid");
            if (signature.Length != SignatureSize) throw new ArgumentException("Signature size is invalid");

            this.Add(OPCODE.SIGNATURE);
            this.AddRange(signature);
            this.Add(OPCODE.PUBKEY);
            this.AddRange(publicKey);
        }

        /// <summary>
        /// This will initialize the script as an P2SH unlocking script (Pay To Script Hash)
        /// </summary>
        /// <param name="script">The script that will be verified</param>
        public UnlockingScript(Script script, Script unlockScript = null) : base(SCRIPTTYPE.UNLOCK_P2SH)
        {
            if (unlockScript == null)
                unlockScript = new CustomScript(OPCODE.DO_NOTHING); //Used because empty script will not be accepted

            byte[] ls = script.ToArray();
            byte[] us = unlockScript.ToArray();

            this.PushSizeAndCode(us.Length);
            this.AddRange(us);
            this.PushSizeAndCode(ls.Length);
            this.AddRange(ls);
        }
    }
}
