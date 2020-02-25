using System;
using System.Collections.Generic;
using System.Linq;
using BlockChain.Transactions.Scripting.Enums;

namespace BlockChain.Transactions.Scripting.Scripts
{
    /// <summary>
    /// Basic script object,
    /// Extend for custom scripts
    /// </summary>
    public class Script : Queue<byte>
    {
        //Constants
        public const int SignatureSize = 64; //Size of RSA[512] signature
        public const int PubKeySize = 84; //Size of RSA[512] public key
        public const int AddressSize = 20; //Size of HASH160

        public SCRIPTTYPE Type { get; private set; }

        public Script(SCRIPTTYPE type = SCRIPTTYPE.UNKNOWN)
            => this.Type = type;

        public Script(byte[] script) : base(script.Select(x => x))
            => this.Type = GetScriptType();

        /// <summary>
        /// Insert instructions of script above instructions of this script
        /// </summary>
        /// <param name="script">Another script. For example, an unlocking script</param>
        public void InsertScript(Script script)
            => this.AddRange(script.ToArray());

        public new byte[] ToArray() => this.Select(x => x).ToArray();

        /*
         * Find out script type
         */

        public bool IsLockP2PK() => (this.Count() == PubKeySize + 2 && this.First() == (byte)OPCODE.PUBKEY && this.Last() == (byte)OPCODE.CHECKSIG);
        public bool IsUnlockP2PK() => (this.Count() == SignatureSize + 1 && this.First() == (byte)OPCODE.SIGNATURE);
        public bool IsLockP2PKH()  => (this.Take(3).ToArray().SequenceEqual(new byte[3] { (byte)OPCODE.DUP, (byte)OPCODE.HASH160, (byte)OPCODE.PUBKEY_HASH }) && this.Last() == (byte)OPCODE.CHECKSIG);
        public bool IsUnlockP2PKH() => (this.Count() == PubKeySize + SignatureSize + 2 && this.First() == (byte)OPCODE.SIGNATURE && this.Skip(SignatureSize + 1).Take(1).First() == (byte)OPCODE.PUBKEY);

        /// <summary>
        /// Get script type using the helper functions
        /// </summary>
        /// <returns>SCRIPTTYPE</returns>
        public SCRIPTTYPE GetScriptType()
        {
            if (IsLockP2PK()) return SCRIPTTYPE.LOCK_P2PK;
            else if (IsUnlockP2PK()) return SCRIPTTYPE.UNLOCK_P2PK;
            else if (IsLockP2PKH()) return SCRIPTTYPE.LOCK_P2PKH;
            else if (IsUnlockP2PKH()) return SCRIPTTYPE.UNLOCK_P2PKH;
            else return SCRIPTTYPE.UNKNOWN;
        }

        /*
         * Helper methods
         */


        public void Add(OPCODE code) => Enqueue((byte)code);
        public void Add(byte data) => Enqueue(data);

        /// <summary>
        /// Push a range to the instructions
        /// </summary>
        /// <param name="data">The data you want</param>
        public void AddRange(byte[] data)
            => data.Reverse().ToList().ForEach(f => Add(f));

        /// <summary>
        /// Get range of data from the instructions
        /// </summary>
        /// <param name="count">The size of data you want</param>
        /// <returns>byte[]</returns>
        public byte[] DequeueRange(int count)
        {
            if (this.Count < count) throw new System.ArgumentException("Count out of range.");
            List<byte> buffer = new List<byte>();
            for (int i = 0; i < count; i++)
                buffer.Add(Dequeue());
            return buffer.ToArray().Reverse().ToArray();
        }

        /*
         *  Helpers
         */

        //Script helpers
        public void PushSizeAndCode(int size)
        {
            if (size <= byte.MaxValue)
            {
                this.Add((byte)size);
                this.Add(OPCODE.PUSHDATA_1);
            }
            else if (size <= short.MaxValue)
            {
                this.AddRange(System.BitConverter.GetBytes((short)size));
                this.Add(OPCODE.PUSHDATA_2);
            }
            else
            {
                this.AddRange(System.BitConverter.GetBytes(size));
                this.Add(OPCODE.PUSHDATA_4);
            }
        }
    }
}