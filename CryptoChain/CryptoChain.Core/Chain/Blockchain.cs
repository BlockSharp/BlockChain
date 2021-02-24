using System.Collections;
using System.Collections.Generic;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;

namespace CryptoChain.Core.Chain
{
    public class Blockchain
    {
        private readonly IBlockStore _store;

        public Blockchain(IBlockStore store)
        {
            _store = store;
        }
    }
}