using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Transactions
{
    public class TransactionList : List<Transaction>, ISerializable
    {
        public int Length => this.Sum(x => x.Length + 4);
        public TransactionList() {}

        public TransactionList(byte[] serialized)
        {
            var items = Serialization.MultipleFromBuffer(serialized).ToList();
            foreach (var i in items)
                Add(new Transaction(i));
        }
        
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
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
    }
}