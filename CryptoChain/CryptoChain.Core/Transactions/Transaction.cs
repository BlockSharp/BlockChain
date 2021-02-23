using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions.Data;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Transactions
{
    public class Transaction : ISerializable
    {
        public int Length => 
            6 + Inputs.Sum(x => x.Length + 4) + Outputs.Sum(x => x.Length + 4) + 4;
        
        //Version, 4 bytes. Used as a hexadecimal value
        public int Version { get; }
        
        //Indicates the amount of transaction inputs and outputs
        public byte TxInCount { get; private set; }
        public byte TxOutCount { get; private set; }
        
        public ICollection<TxInput> Inputs { get; set; } = new List<TxInput>();
        public ICollection<TxOutput> Outputs { get; set; } = new List<TxOutput>();

        private byte[]? _txId;

        /// <summary>
        /// Generate the Transaction Hash (or TxID)
        /// </summary>
        /// <returns>A byte[] hash (2x SHA-256), 32 bytes</returns>
        public byte[] TxId
        {
            get
            {
                if (_txId == null)
                    _txId = Cryptography.Hashing.Hash.HASH_256(Serialize());
                return _txId;
            }
        }
        
        /// <summary>
        /// Indicates when the transaction can be mined into a block.
        /// Can indicate a Unix timestamp (when it is above 1_000_000_000, twice of bitcoin) or a block height (when below that number)
        /// </summary>
        public uint LockTime { get; }

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="lockTime">The locking time or block height</param>
        /// <param name="version">The blockchain version</param>
        public Transaction(uint lockTime = 0, int version = Constants.TransactionVersion)
        {
            Version = version;
            LockTime = lockTime;
        }

        /// <summary>
        /// Deserialize a serialized Transaction object
        /// </summary>
        /// <param name="serialized">The serialized transaction</param>
        public Transaction(byte[] serialized)
        {
            Version = BitConverter.ToInt32(serialized, 0);
            TxInCount = serialized[4];
            TxOutCount = serialized[5];
            int idx = 6;
            
            Inputs = Serialization.MultipleFromBuffer(serialized, idx)
                .Take(TxInCount).Select(x => new TxInput(x)).ToList();
            
            idx += Inputs.Sum(x => x.Length + 4);

            Outputs = Serialization.MultipleFromBuffer(serialized, idx)
                .Take(TxOutCount).Select(x => new TxOutput(x)).ToList();

            idx += Outputs.Sum(x => x.Length + 4);
            LockTime = BitConverter.ToUInt32(serialized, idx);
        }

        /// <summary>
        /// Create a new coinbase transaction
        /// </summary>
        /// <param name="outputScript"></param>
        /// <param name="blockHeight">The current blockHeight to avoid duplicate coinbase TxIds</param>
        /// <param name="amount">The amount of money (block reward + transaction fees)</param>
        /// <returns>Coinbase transaction</returns>
        public static Transaction CoinBase(IScript outputScript, uint blockHeight, ulong amount)
        {
            var unlockingScript = new ScriptBuilder();
            unlockingScript.PushData(BitConverter.GetBytes(blockHeight));
            var input = new TxInput(new byte[Constants.TransactionHashLength], byte.MaxValue, unlockingScript);
            var output = new TxOutput(amount, outputScript);
            Transaction t = new (Constants.MinimumCoinBaseLockTime);
            t.Inputs.Add(input);
            t.Outputs.Add(output);
            return t;
        }
        
        /// <summary>
        /// Serialize the transaction
        /// </summary>
        /// <returns>byte[Length]</returns>
        public byte[] Serialize()
        {
            TxInCount = (byte)Inputs.Count;
            TxOutCount = (byte)Outputs.Count;
            
            byte[] buffer = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(Version), 0, buffer, 0, 4);
            buffer[4] = TxInCount;
            buffer[5] = TxOutCount;
            int idx = 6;
            
            var txs = new List<ISerializable>();
            txs.AddRange(Inputs);
            txs.AddRange(Outputs);

            idx = buffer.AddSerializableRange(txs, idx);
            Buffer.BlockCopy(BitConverter.GetBytes(LockTime), 0, buffer, idx, 4);
            return buffer;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var x = (Transaction) obj;
            return x.TxId.SequenceEqual(TxId);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("====================== Transaction =====================");
            sb.AppendLine("Version: " + Version.ToString("X"));
            sb.AppendLine("TxID/Hash: " + TxId.ToHexString());
            sb.AppendLine($"Inputs #: {TxInCount}, Outputs #: {TxOutCount}");
            sb.AppendLine("[Inputs]");
            foreach (var i in Inputs)
                sb.AppendLine(i.ToString());
            sb.AppendLine("[Outputs]");
            foreach (var o in Outputs)
                sb.AppendLine(o.ToString());
            return sb.ToString();
        }
    }
}