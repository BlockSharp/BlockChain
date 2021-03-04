using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Scripting;
using CryptoChain.Core.Transactions.Scripting.Interpreter;

namespace CryptoChain.Core.Chain
{
    public class Blockchain : BlockchainBase
    {
        public Blockchain(IBlockStore store, ITransactionStore transactionStore) 
            : base(store, transactionStore)
        {
            Update();
        }

        public void Update()
        {
            Interpreter.CurrentBlockHeight = Blocks.BlockHeight;
        }
    }
}