using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    public class UTXOIndex : ISerializable
    {
        public ushort OutputCount { get; set; }
        public byte[] CompressedTxId { get; set; }
        public List<uint> UnspentOutputs { get; set; }
        public int Length => 20 + 2 + UnspentOutputs.Count * 4;

        public UTXOIndex(byte[] txId, uint[] unspentOutputs)
        {
            UnspentOutputs = unspentOutputs.ToList();
            CompressedTxId = Hash.SHA_1(txId);
        }

        public UTXOIndex(byte[] serialized)
        {
            UnspentOutputs = new List<uint>();
            ushort count = BitConverter.ToUInt16(serialized, 0);
            OutputCount = count;
            CompressedTxId = serialized[2..22];
            int idx = 22;
            for (int i = 0; i < count; i++)
            {
                UnspentOutputs.Add(BitConverter.ToUInt32(serialized, idx));
                idx += 4;
            }
        }
        
        public byte[] Serialize()
        {
            OutputCount = (ushort)UnspentOutputs.Count;
            byte[] buffer = new byte[Length];
            BitConverter.GetBytes(OutputCount).CopyTo(buffer, 0);
            CompressedTxId.CopyTo(buffer, 2);
            int idx = 22;
            foreach (var uo in UnspentOutputs)
            {
                BitConverter.GetBytes(uo).CopyTo(buffer, idx);
                idx += 4;
            }

            return buffer;
        }
    }
}