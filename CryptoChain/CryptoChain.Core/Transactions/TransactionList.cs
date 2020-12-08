using System;
using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Transactions
{
    public class TransactionList : List<Transaction>, ISerializable, IBlockData
    {
        public int TransactionCount { get; private set; }
        public int Length => this.Sum(x => x.Length + 4) + 4;
        public TransactionList() {}

        public TransactionList(byte[] serialized)
            => FromArray(serialized);
        
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(TransactionCount), 0, buffer, 0, 4);
            var items = new List<ISerializable>();
            foreach (var i in this)
                items.Add(i);
            
            buffer.AddSerializableRange(items);
            return buffer;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var x = (TransactionList) obj;
            return x.Length == Length && x.SequenceEqual(this);
        }

        public new byte[] ToArray()
            => Serialize();

        public void FromArray(byte[] data)
        {
            TransactionCount = BitConverter.ToInt32(data);
            var items = Serialization.MultipleFromBuffer(data, 4).ToList();
            foreach (var i in items)
                Add(new Transaction(i));
        }
    }
}