using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Hashing;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Transactions
{
    public class TransactionList : List<Transaction>, ISerializable
    {
        public int TransactionCount { get; private set; }
        public int Length => this.Sum(x => x.Length + 4) + 4;

        private byte[]? _merkleRoot;

        /// <summary>
        /// Get the merkle root from all txIds of all transactions in the list
        /// </summary>
        public byte[] MerkleRoot
        {
            get
            {
                if (_merkleRoot == null)
                {
                    Queue<byte[]> txIds = new(this.Select(x => x.TxId));
                    _merkleRoot =  GenerateMerkleRoot(txIds);
                }
                
                return _merkleRoot;
            }
        }

        public TransactionList() {}

        /// <summary>
        /// Deserialize transactionList from bytes
        /// </summary>
        /// <param name="serialized">The serialized TransactionList</param>
        public TransactionList(byte[] serialized)
        {
            TransactionCount = BitConverter.ToInt32(serialized);
            var items = Serialization.MultipleFromBuffer(serialized, 4).ToList();
            foreach (var i in items)
                Add(new Transaction(i));
        }

        public byte[] Serialize()
        {
            TransactionCount = Count;
            byte[] buffer = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(TransactionCount), 0, buffer, 0, 4);
            var items = new List<ISerializable>();
            foreach (var i in this)
                items.Add(i);
            
            buffer.AddSerializableRange(items, 4);
            return buffer;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var x = (TransactionList) obj;
            return x.Length == Length && x.SequenceEqual(this);
        }
        
        /// <summary>
        /// Generate the merkle root from all transactions in this list
        /// </summary>
        /// <param name="txIds">The TxIds to be hashed into a merkle root</param>
        /// <returns>A merkle root (recursive)</returns>
        private byte[] GenerateMerkleRoot(Queue<byte[]> txIds)
        {
            if (txIds.Count == 1)
                return txIds.Dequeue();

            var hashes = new Queue<byte[]>();
            
            while (txIds.Any())
            {
                //hash itself twice if no two pairs anymore
                var first = txIds.Count >= 2 ? txIds.Dequeue() : txIds.Peek();
                var second = txIds.Dequeue();
                var both = new byte[first.Length + second.Length];
                first.CopyTo(both, 0);
                second.CopyTo(both, first.Length);
                hashes.Enqueue(Hash.HASH_256(both));
            }
            
            return GenerateMerkleRoot(hashes);
        }
    }
}