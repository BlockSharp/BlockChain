using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace BlockChain.Transactions
{
    public class Transaction
    {
        public TxHeader txHeader { get; protected set; }
        public List<TxIn> txIncoming { get; protected set; }
        public List<TxOut> txOutgoing { get; protected set; }
        public uint lockTime { get; protected set; }


        /// <summary>
        /// Create new transaction
        /// </summary>
        /// <param name="incoming">The incoming transaction (1)</param>
        /// <param name="outgoing">The outgoing transaction (1)</param>
        /// <param name="lockTime">Locktime. Default 0x0</param>
        /// <param name="version">Version. Usually 0x0</param>
        public Transaction(TxIn incoming, TxOut outgoing, uint lockTime = 0x0, uint version = 0x0) : this(new List<TxIn>() { incoming }, new List<TxOut> { outgoing }, lockTime, version) { }

        /// <summary>
        /// Create new transaction
        /// </summary>
        /// <param name="inputs">The TxIns. At least one</param>
        /// <param name="outputs">The TxOuts. At least one</param>
        /// <param name="lockTime">Locktime. Default 0x0</param>
        /// <param name="version">Version. Usually 0x0</param>
        public Transaction(List<TxIn> inputs, List<TxOut> outputs, uint lockTime = 0x0, uint version = 0x0)
        {
            this.txIncoming = inputs;
            this.txOutgoing = outputs;
            this.txHeader = new TxHeader(inputs.Count(), outputs.Count());
            this.lockTime = 0x0; //Not in use yet
            this.txHeader = new TxHeader(inputs.Count(), outputs.Count(), version);
        }

        /// <summary>
        /// Deserialize Transaction object
        /// </summary>
        /// <param name="serialized">Serialized Transaction (byte[12 + 4 + (40+) + (12+)])</param>
        public Transaction(byte[] serialized)
        {
            this.txIncoming = new List<TxIn>();
            this.txOutgoing = new List<TxOut>();
            this.txHeader = new TxHeader(serialized.Take(12).ToArray());

            if (txHeader.TxInCount == 0 || txHeader.TxOutCount == 0)
                throw new ArgumentException("This transaction is invalid.");

            int idx = 12;
            for (int i = 0; i < txHeader.TxInCount; i++)
            {
                byte[] next = serialized.Skip(idx).Take(40).ToArray();
                int size = TxIn.GetSize(ref next);
                this.txIncoming.Add(new TxIn(serialized.Skip(idx).Take(size).ToArray()));
                idx += size;
            }

            for (int i = 0; i < txHeader.TxOutCount; i++)
            {
                byte[] next = serialized.Skip(idx).Take(12).ToArray();
                int size = TxOut.GetSize(ref next);
                this.txOutgoing.Add(new TxOut(serialized.Skip(idx).Take(size).ToArray()));
                idx += size;
            }

            this.lockTime = BitConverter.ToUInt32(serialized, idx);
        }

        /// <summary>
        /// Generate hash of the transaction
        /// </summary>
        /// <returns>Hash of this object: byte[32]</returns>
        public byte[] Hash()
            => Hash(this);


        /// <summary>
        /// Hash transaction
        /// </summary>
        /// <param name="transaction">The transaction to be hashed</param>
        /// <returns>Hash (byte[32]])</returns>
        public static byte[] Hash(Transaction transaction)
        {
            using (var sha = SHA256.Create())
                return sha.ComputeHash(transaction.ToArray());
        }

        /// <summary>
        /// Serialize transaction to byte[]
        /// </summary>
        /// <returns>byte[12 + 4 + (40+) + (12+)]</returns>
        public byte[] ToArray()
        {
            int size = txIncoming.Sum(f => f.Size) + txOutgoing.Sum(f => f.Size);

            byte[] buffer = new byte[12 + 4 + size];
            Buffer.BlockCopy(txHeader.ToArray(), 0, buffer, 0, 12);

            int idx = 12;
            foreach (var item in txIncoming)
            {
                Buffer.BlockCopy(item.ToArray(), 0, buffer, idx, item.Size);
                idx += item.Size;
            }

            foreach (var item in txOutgoing)
            {
                Buffer.BlockCopy(item.ToArray(), 0, buffer, idx, item.Size);
                idx += item.Size;
            }

            Buffer.BlockCopy(BitConverter.GetBytes(lockTime), 0, buffer, idx, 4);

            return buffer;
        }
    }
}