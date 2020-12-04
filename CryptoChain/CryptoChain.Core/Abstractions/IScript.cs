using System.Collections;
using System.Collections.Generic;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Abstractions
{
    public interface IScript : ISerializable
    {
        Opcode Next();
        byte NextByte();
        IEnumerable<byte> NextRange();
        byte[] NextRange(int count);
        
        void Add(byte b);
        void Add(Opcode code);
        void AddRange(byte[] bytes);
    }
}