using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Transactions;

namespace CryptoChain.Core.Abstractions
{
    public interface ITransactionStore
    {
        /// <summary>
        /// Reference to a blockStore instance
        /// </summary>
        public IBlockStore BlockStore { get; set; }
        
        /// <summary>
        /// Get transaction by its TxID
        /// </summary>
        /// <param name="txId">The transaction hash/id</param>
        /// <returns>The desired transaction</returns>
        public Task<Transaction?> GetTransaction(byte[] txId);
        
        /// <summary>
        /// Get transaction by the block hash and the index in the block
        /// </summary>
        /// <param name="blockId">The block's id</param>
        /// <param name="index">The index of the transaction in the block</param>
        /// <returns>The desired transaction</returns>
        public Task<Transaction> GetTransaction(byte[] blockId, int index);
        
        /// <summary>
        /// Add a new transaction to the storage system/indexing file
        /// </summary>
        /// <param name="transaction">The transaction to be added</param>
        /// <param name="blockId">The block containing the transaction</param>
        /// <param name="index">The index of the transaction in the block</param>
        public void Add(Transaction transaction, byte[] blockId, int index);

        /// <summary>
        /// Check if an output from a transaction is spent or not
        /// </summary>
        /// <param name="txId">The transaction</param>
        /// <param name="vOut">The referenced output index</param>
        /// <returns>True if the output is already spent</returns>
        public bool IsUnspent(byte[] txId, ushort vOut)
            => GetUnspent(txId).Contains(vOut);

        /// <summary>
        /// Get all unspent transaction outputs in a dictionary
        /// </summary>
        /// <returns>Dictionary [TxID (byte[]), Array with unspent vOut's (indexes of outputs))</returns>
        public Dictionary<byte[], ushort[]> ListUnspent();

        /// <summary>
        /// Get unspent output indexes (vOut) from a transaction
        /// </summary>
        /// <param name="txId">The transaction</param>
        /// <returns>Indexes of unspent transaction outputs</returns>
        public ushort[] GetUnspent(byte[] txId);
    }
}