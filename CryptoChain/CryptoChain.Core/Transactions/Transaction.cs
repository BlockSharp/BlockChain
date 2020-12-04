using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions.Data;

namespace CryptoChain.Core.Transactions
{
    public class Transaction : ISerializable
    {
        public int Length => 
            6 + Inputs.Sum(x => x.Length) + Outputs.Sum(x => x.Length);
        
        //Version, 4 bytes. Used as a hexadecimal value
        public int Version { get; set; }
        
        //Indicates the amount of transaction inputs and outputs
        public byte TxInCount { get; }
        public byte TxOutCount { get; }
        
        public ICollection<TxInput> Inputs { get; set; }
        public ICollection<TxOutput> Outputs { get; set; }

        public Transaction(byte[] serialized)
        {
            Version = BitConverter.ToInt32(serialized, 0);
            TxInCount = serialized[4];
            TxOutCount = serialized[5];
            int idx = 6;

            Inputs = Serialization.MultipleFromBuffer(serialized, idx)
                .Take(TxInCount).Select(x => new TxInput(x)).ToList();

            idx += Inputs.Sum(x => x.Length);

            Outputs = Serialization.MultipleFromBuffer(serialized, idx)
                .Take(TxOutCount).Select(x => new TxOutput(x)).ToList();
        }
        
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(Version), 0, buffer, 0, 4);
            buffer[4] = TxInCount;
            buffer[5] = TxOutCount;
            int idx = 6;
            
            var txs = new List<ISerializable>();
            txs.AddRange(Inputs);
            txs.AddRange(Outputs);

            buffer.AddSerializableRange(txs, idx);
            return buffer;
        }
    }
}