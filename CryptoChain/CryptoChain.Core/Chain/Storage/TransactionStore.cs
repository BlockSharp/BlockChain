using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain.Storage.Indexes;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions;
using LiteDB;

namespace CryptoChain.Core.Chain.Storage
{
    public class TransactionStore : ITransactionStore, IDisposable
    {
        private readonly LiteDatabase _indexDb;
        private readonly ILiteCollection<TransactionIndex> _txIndexes;
        private readonly ILiteCollection<UtxoIndex> _utxoIndexes;
        public IBlockStore BlockStore { get; set; }

        public TransactionStore(IBlockStore blockStore, string path)
        {
            BlockStore = blockStore;
            _indexDb = new LiteDatabase(Path.Combine(path, "tx_idx.db"));
            _txIndexes = _indexDb.GetCollection<TransactionIndex>("tx_idx");
            _utxoIndexes = _indexDb.GetCollection<UtxoIndex>("utxo_idx");
        }
        
        public async Task CreateTxIndexes()
        {
            await Task.Run(() =>
            {
                _indexDb.DropCollection("tx_idx");
                Stopwatch sw = Stopwatch.StartNew();
                uint height = 1;
                var indexes = new List<TransactionIndex>(500);
                foreach (var b in BlockStore.All())
                {
                    if (b.Type == BlockDataIdentifier.TRANSACTIONS)
                    {
                        DebugUtils.Log("Creating tx indexes for block #"+height);
                        var height1 = height;
                        indexes.AddRange(b.Transactions.Select(x => new TransactionIndex{TxId = x.TxId, Index = (ushort)b.Transactions.IndexOf(x), BlockHeight = height1}));
                    }

                    if (indexes.Count >= 500)
                    {
                        _txIndexes.InsertBulk(indexes);
                        indexes.Clear();
                    }
                    
                    height++;
                }

                if (indexes.Any())
                    _txIndexes.InsertBulk(indexes);
                
                DebugUtils.Info("Creating indexes finished in "+sw.Elapsed);
            });
        }

        public async Task CreateUtxoIndexes()
        {
            await Task.Run(() =>
            {
                _indexDb.DropCollection("utxo_idx");
                Stopwatch sw = Stopwatch.StartNew();
                uint height = 1;
                foreach (var b in BlockStore.All())
                {
                    if (b.Type == BlockDataIdentifier.TRANSACTIONS)
                    {
                        DebugUtils.Log("Creating UTXO indexes for block #"+height++);
                        foreach (var t in b.Transactions)
                        {
                            _utxoIndexes.Insert(new UtxoIndex{TxId = t.TxId, UnspentOutputs = Enumerable.Range(0, t.TxOutCount).Select(x => (ushort)x).ToList()});
                            foreach (var input in t.Inputs)
                            {
                                var refTx = _utxoIndexes.FindOne(x => x.TxId == input.TxId);
                                if(refTx != null)
                                {
                                    refTx.UnspentOutputs.Remove(input.VOut);
                                    if (!refTx.UnspentOutputs.Any())
                                        _utxoIndexes.Delete(input.TxId);
                                    else
                                        _utxoIndexes.Update(refTx);
                                }
                            }
                        }
                    }
                }
                DebugUtils.Info("Creating indexes finished in "+sw.Elapsed);
            });
        }

        private TransactionIndex? GetMeta(byte[] hash)
            => _txIndexes.FindById(hash);
        
        public async Task<Transaction?> GetTransaction(byte[] txId)
        {
            var meta = GetMeta(txId);
            if (meta == null) return null;
            var block = await BlockStore.GetBlock(meta.BlockHeight);
            return block?.Transactions[meta.Index];
        }

        public async Task<Transaction?> GetTransaction(byte[] blockId, int index)
        {
            var block = await BlockStore.GetBlock(blockId);
            return block?.Transactions[index];
        }

        public void Add(Transaction transaction, byte[] blockId, int index)
        {
            uint blockHeight = BlockStore.GetHeight(blockId);
            _txIndexes.Insert(new TransactionIndex{TxId = transaction.TxId, Index = (ushort)index, BlockHeight = blockHeight});
            
            DebugUtils.Log("Updating UTXO database...");
            _utxoIndexes.Insert(new UtxoIndex
            {
                TxId = transaction.TxId,
                UnspentOutputs = Enumerable.Range(0, transaction.Outputs.Count()).Select(x => (ushort) x).ToList()
            });
            
            foreach (var i in transaction.Inputs)
            {
                var refTx = _utxoIndexes.FindById(i.TxId);
                if (refTx != null)
                {
                    refTx.UnspentOutputs.Remove(i.VOut);
                    if (!refTx.UnspentOutputs.Any())
                        _utxoIndexes.Delete(i.TxId);
                    else
                        _utxoIndexes.Update(refTx);
                }
            }
            DebugUtils.Info("Updated UTXO database");
        }

        public uint GetBlockHeight(byte[] txId)
            => GetMeta(txId)?.BlockHeight ?? 0;

        public async Task<Block?> GetContainingBlock(byte[] txId)
        {
            var meta = GetMeta(txId);
            if (meta == null) return null;
            return await BlockStore.GetBlock(meta.BlockHeight);
        }

        public Dictionary<string, List<ushort>> ListUnspent()
            => _utxoIndexes.FindAll().ToDictionary(x => Convert.ToBase64String(x.TxId),
                x => x.UnspentOutputs);

        public IEnumerable<UtxoIndex> ListUtxo()
            => _utxoIndexes.FindAll();

        public ushort[] GetUnspent(byte[] txId)
            => _utxoIndexes.FindById(txId)?.UnspentOutputs.ToArray() ??
               new ushort[0];

        public void Dispose()
        {
            _indexDb.Dispose();
        }
    }
}