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
        void Add(params Opcode[] code);
        void Add(IScript s);
        void AddRange(byte[] bytes);
        IScript Clone();

        byte[] Hash();
    }
}