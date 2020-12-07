using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions.Data;
using CryptoChain.Core.Transactions.Scripting.Interpreter.Operations;

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
        
        public uint LockTime { get; }

        public Transaction(uint lockTime = 0, int version = Constants.Version)
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

        /// <summary>
        /// Generate the Transaction Hash
        /// </summary>
        /// <returns>A byte[] hash (2x SHA-256)</returns>
        public byte[] Hash()
            => Cryptography.Hashing.Hash.HASH_256(Serialize());
        
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var x = (Transaction) obj;
            return x.Length == Length && x.Version == Version && x.LockTime == LockTime
                   && x.Inputs.SequenceEqual(Inputs) && x.Outputs.SequenceEqual(Outputs);
        }
    }
}