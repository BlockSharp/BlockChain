using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Transactions;

namespace CryptoChain.Core.Chain.Storage
{
    public class TransactionStore : ITransactionStore
    {
        private IBlockStore _blocks;
        
        //These grow very large so they can't be stored into RAM. Consider looping over index files
        //However, that will decrease performance.
        private Dictionary<string, (string block, int index)> _indexes;
        private Dictionary<string, List<ushort>> _txUnspent;

        public TransactionStore(ref IBlockStore blockStore, bool autoIndex = true)
        {
            _blocks = blockStore;
            _indexes = new Dictionary<string, (string, int)>();
            _txUnspent = new Dictionary<string, List<ushort>>();
            if (autoIndex)
                Index().Wait();
        }

        //HEAVY!
        public async Task Index()
        {
            await Task.Run(() =>
            {
                #if DEBUG
                Stopwatch sw = Stopwatch.StartNew();                
                Console.WriteLine("Indexing transactions...");
                #endif
                
                foreach (var block in _blocks.All())
                {
                    if (block.Type != BlockDataIdentifier.TRANSACTIONS)
                        continue;

                    foreach (var t in block.Transactions)
                    {
                        _indexes.Add(Convert.ToHexString(t.TxId), (Convert.ToHexString(block.Hash), block.Transactions.IndexOf(t)));
                        
                        if(t.IsCoinbase)
                            continue;
                        
                        _txUnspent.Add(Convert.ToHexString(t.TxId), Enumerable.Range(0, t.TxOutCount).Select(x => (ushort)x).ToList());
                        
                        foreach (var input in t.Inputs)
                        {
                            if (_txUnspent.ContainsKey(Convert.ToHexString(input.TxId)))
                                _txUnspent[Convert.ToHexString(input.TxId)].Remove(input.VOut);
                        }
                    }
                }
                #if DEBUG
                sw.Stop();
                Console.WriteLine($"Indexed {_indexes.Count} transactions in {sw.Elapsed}");
                #endif
            });
        }

        public async Task<Transaction> GetTransaction(byte[] txId)
        {
            var idx = _indexes[Convert.ToHexString(txId)];
            return await GetTransaction(Convert.FromHexString(idx.block), idx.index);
        }

        public async Task<Transaction> GetTransaction(byte[] blockId, int index)
        {
            var blk = await _blocks.GetBlock(blockId);
            if (blk == null)
                throw new ArgumentException("Block not found");
            return blk.Transactions[index];
        }

        public void Add(Transaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public bool IsUnspent(byte[] txId, ushort vOut)
        {
            return GetUnspent(txId).Contains(vOut);
        }

        public Dictionary<byte[], ushort[]> ListUnspent()
        {
            return _txUnspent.ToDictionary(x => Convert.FromHexString(x.Key), x => x.Value.ToArray());
        }

        public ushort[] GetUnspent(byte[] txId)
        {
            if (_txUnspent.TryGetValue(Convert.ToBase64String(txId), out List<ushort>? vouts))
                return vouts.ToArray();
            return new ushort[0];
        }
    }
}