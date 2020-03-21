using BlockChain.Transactions.Scripting.Scripts;
using System;
using System.Linq;

namespace BlockChain.Transactions
{
    public class TxHeader
    {
        public uint Version { get; private set; } //4 bytes
        public uint TxInCount { get; private set; } //4 bytes
        public uint TxOutCount { get; private set; } //4 bytes

        /// <summary>
        /// Create new TxHeader
        /// </summary>
        /// <param name="txInCount">The count of TxIns</param>
        /// <param name="txOutCount">The count of TxOuts</param>
        /// <param name="version">The version. Usually 0x000000</param>
        public TxHeader(int txInCount, int txOutCount, uint version = 0)
        {
            if (txInCount < 0) throw new ArgumentException("TxInCount must be greater than zero");
            if (txOutCount < 0) throw new ArgumentException("TxOutCount must be greater than zero");

            this.TxInCount = (uint)txInCount;
            this.TxOutCount = (uint)txOutCount;
            this.Version = version;
        }

        /// <summary>
        /// Deserialize header
        /// </summary>
        /// <param name="serialized">byte[12]</param>
        public TxHeader(byte[] serialized)
        {
            this.Version = BitConverter.ToUInt32(serialized, 0);
            this.TxInCount = BitConverter.ToUInt32(serialized, 4);
            this.TxOutCount = BitConverter.ToUInt32(serialized, 8);
        }

        /// <summary>
        /// Serialize header
        /// </summary>
        /// <returns>byte[12]</returns>
        public byte[] ToArray()
        {
            byte[] buffer = new byte[12];
            Buffer.BlockCopy(BitConverter.GetBytes(Version), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(TxInCount), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(TxOutCount), 0, buffer, 8, 4);
            return buffer;
        }
    }

    /// <summary>
    /// TxIn (Transaction Input) = 40+ B
    /// </summary>
    public class TxIn
    {
        public byte[] PrevHash { get; private set; } //32 bytes <-- points to previous transaction
        public uint TxOutIdx { get; private set; } //4 bytes <-- points to selected TxOut index from previous transaction
        public uint scriptLength { get; private set; } //4 bytes, indicates the length of the script
        public Script script { get; set; } //unknown size <-- unlocks transaction

        public int Size { get { return (40 + script.Count); } private set { } }

        /// <summary>
        /// Create empty TxIn object without script
        /// </summary>
        /// <param name="prevHash">The hash of the previous transaction it points to. byte[32]</param>
        /// <param name="TxOutIndex">The TxOut index in the previous transaction it points to</param>
        public TxIn(byte[] prevHash, uint TxOutIndex) : this(prevHash, TxOutIndex, new Script()) { }

        /// <summary>
        /// Create a new TxIn object with an unlocking script
        /// </summary>
        /// <param name="prevHash">The hash of the previous transaction it points to. byte[32]</param>
        /// <param name="TxOutIndex">The TxOut index in the previous transaction it points to</param>
        /// <param name="unlockingScript">The unlocking script for the TxOut it points to</param>
        public TxIn(byte[] prevHash, uint TxOutIndex, byte[] unlockingScript) : this(prevHash, TxOutIndex, new Script(unlockingScript)) { }

        /// <summary>
        /// Create new TxIn object
        /// </summary>
        /// <param name="prevHash">The hash of the previous transaction it points to. byte[32]</param>
        /// <param name="TxOutIndex">The TxOut index in the previous transaction it points to</param>
        /// <param name="unlockingScript">The unlocking script for the TxOut it points to</param>
        public TxIn(byte[] prevHash, uint TxOutIndex, Script unlockingScript)
        {
            if (prevHash.Length != 32) throw new ArgumentException("The prevHash is invalid. Length must be 32");

            this.PrevHash = prevHash;
            this.TxOutIdx = TxOutIndex;
            this.scriptLength = (uint)unlockingScript.Count;
            this.script = unlockingScript;
        }

        /// <summary>
        /// Deserialize TxIn object
        /// </summary>
        /// <param name="serialized">Serialized TxIn object (byte[40+])</param>
        public TxIn(byte[] serialized)
        {
            this.PrevHash = serialized.Take(32).ToArray();
            this.TxOutIdx = BitConverter.ToUInt32(serialized, 32);
            this.scriptLength = BitConverter.ToUInt32(serialized, 36);
            this.script = new Script(serialized.Skip(40).ToArray());
        }

        /// <summary>
        /// Get TxIn object size
        /// </summary>
        /// <param name="serialized">The referenced binary data</param>
        /// <returns>The size of the referenced serialized Txin object</returns>
        public static int GetSize(ref byte[] serialized)
            => (40 + (int)BitConverter.ToUInt32(serialized, 36));

        /// <summary>
        /// Get serialized TxIn object
        /// </summary>
        /// <returns>byte[40+]</returns>
        public byte[] ToArray()
        {
            this.scriptLength = (uint)this.script.Count;

            byte[] buffer = new byte[40 + scriptLength];
            Buffer.BlockCopy(PrevHash, 0, buffer, 0, 32);
            Buffer.BlockCopy(BitConverter.GetBytes(TxOutIdx), 0, buffer, 32, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(scriptLength), 0, buffer, 36, 4);
            Buffer.BlockCopy(script.ToArray(), 0, buffer, 40, (int)scriptLength);
            return buffer;
        }
    }

    /// <summary>
    /// TxOut (Transaction Output) = 12+ bytes
    /// </summary>
    public class TxOut
    {
        public ulong Amount { get; private set; } //8 bytes
        public uint scriptLength { get; private set; } //4 bytes
        public Script script { get; set; } //unknown size <-- locks transaction

        public int Size { get { return (12 + script.Count); } private set { } }

        /// <summary>
        /// Create empty TxOut without script
        /// </summary>
        /// <param name="amount">The amount</param>
        public TxOut(ulong amount) : this(amount, new Script()) { }

        /// <summary>
        /// Create new TxOut object
        /// </summary>
        /// <param name="amount">The amount</param>
        /// <param name="lockingScript">The script that locks the transaction</param>
        public TxOut(ulong amount, Script lockingScript)
        {
            this.Amount = amount;
            this.script = lockingScript;
            this.scriptLength = (uint)lockingScript.Count;
        }

        /// <summary>
        /// Deserialize TxOut object
        /// </summary>
        /// <param name="serialized">Serialized object byte[12+]</param>
        public TxOut(byte[] serialized)
        {
            this.Amount = BitConverter.ToUInt64(serialized, 0);
            this.scriptLength = BitConverter.ToUInt32(serialized, 8);
            this.script = new Script(serialized.Skip(12).ToArray());
        }

        /// <summary>
        /// Get TxOut object size
        /// </summary>
        /// <param name="serialized">The referenced binary data</param>
        /// <returns>The size of serialized TxOut object</returns>
        public static int GetSize(ref byte[] serialized)
            => (12 + (int)BitConverter.ToUInt32(serialized, 8));

        /// <summary>
        /// Get serialized TxOut object
        /// </summary>
        /// <returns>byte[12+]</returns>
        public byte[] ToArray()
        {
            this.scriptLength = (uint)this.script.Count;

            byte[] buffer = new byte[12 + scriptLength];
            Buffer.BlockCopy(BitConverter.GetBytes(Amount), 0, buffer, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(scriptLength), 0, buffer, 8, 4);
            Buffer.BlockCopy(script.ToArray(), 0, buffer, 12, (int)scriptLength);
            return buffer;
        }
    }
}