using System.Collections.Generic;
using System.Linq;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Transactions.Scripting
{
    public class Script : Queue<byte>, IScript
    {
        public int Length => Count;
        public Script() {}
        public Script(byte[] data) : base(data) {}
        
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

        public void Add(Opcode code)
            => Enqueue((byte) code);

        public void AddRange(byte[] bytes)
        {
            foreach (var b in bytes)
                Add(b);
        }
    }
}