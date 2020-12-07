using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Transactions.Scripting
{
    public class Script : Queue<byte>, IScript
    {
        public int Length => Count;
        
        /// <summary>
        /// Deserialize script
        /// </summary>
        /// <param name="data">The serialized script</param>
        public Script(byte[] data) : base(data) {}
        
        public Script() {}

        public Script(params IScript[] scripts)
        {
            foreach (var s in scripts)
                Add(s);
        }
        
        public byte[] Serialize() => ToArray();
        public Opcode Next() => (Opcode)Dequeue();
        public byte NextByte() => Dequeue();

        public IEnumerable<byte> NextRange()
        {
            while (Count > 0) 
                yield return Dequeue();
        }

        public byte[] NextRange(int count)
            => NextRange().Take(count).ToArray();

        public void Add(byte b)
            => Enqueue(b);

        public void Add(params Opcode[] codes)
        {
            foreach (var c in codes)
                Enqueue((byte)c);
        }

        public void Add(IScript s)
        {
            AddRange(s.Serialize());
        }

        public void AddRange(byte[] bytes)
        {
            foreach (var b in bytes)
                Add(b);
        }

        public byte[] Hash()
            => Cryptography.Hashing.Hash.HASH_160(this.Serialize());

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            var s = (Script)obj;
            return s.Length == Length && this.SequenceEqual(s);
        }

    }
}