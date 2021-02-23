using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoChain.Core.Cryptography.Hashing;
using CryptoChain.Core.Transactions;

namespace CryptoChain.Core.Block
{
    public static class Miner
    {
        /// <summary>
        /// Mine a block from a list of transactions as block data
        /// </summary>
        /// <param name="transactions">The transactions</param>
        /// <param name="prevHash">The hash of the previous block in the chain</param>
        /// <param name="target">The desired target</param>
        /// <param name="token">The cancellation token</param>
        /// <param name="threadCount">The amount of threads you want to use</param>
        /// <returns>A block</returns>
        public static async Task<Block?> MineBlock(TransactionList transactions, byte[] prevHash, Target target,
            CancellationToken token = new(), int threadCount = 0)
            => await MineBlock(new BlockHeader(prevHash, transactions.MerkleRoot, target), transactions.Serialize(),
                BlockDataIdentifier.TRANSACTIONS, token, threadCount);
        
        /// <summary>
        /// Mine a new block using the miner
        /// </summary>
        /// <param name="header">The blockHeader with the required information (PrevHash, Target and MerkleRoot)</param>
        /// <param name="data">The data to be stored into the block</param>
        /// <param name="dataType">The type of the data stored into the block</param>
        /// <param name="token">The cancellation token</param>
        /// <param name="threadCount">The amount of threads you want to use</param>
        /// <returns>A Block if the mining succeeds and isn't cancelled</returns>
        public static async Task<Block?> MineBlock(BlockHeader header, byte[] data, BlockDataIdentifier dataType, CancellationToken token = new(),
            int threadCount = 0)
        {
            var worker = new Worker(header, threadCount);
            var res = await Task.Run(() => worker.Start(token), token);
            if (res == null)
                return null;
            return new Block(data, res, dataType);
        }
        
        
        /// <summary>
        /// The worker class provides the miner itself
        /// </summary>
        private class Worker
        {
            private readonly byte[] _data;
            private const int NonceIdx = 4 + Constants.BlockHashLength + Constants.TransactionHashLength + 4 + 4;
            private readonly int _threadCount;
            private readonly Target _target;
            private uint _nonce;
            private readonly DateTime _time;

            public Worker(BlockHeader header, int threadCount = 0)
            {
                header.Nonce = 0U;
                header.Timestamp = DateTime.Now;
                _time = header.Timestamp;
                _threadCount = threadCount <= 0 ? Environment.ProcessorCount : threadCount;
                _data = header.Serialize();
                _target = header.Target;
            }

            public BlockHeader? Start(CancellationToken token)
            {
                var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
                using var done = new ManualResetEvent(false);
                var running = _threadCount;

                BlockHeader? foundHeader = null;
                _nonce = 0;

                for (int i = 0; i < _threadCount; i++)
                {
                    new Thread(() =>
                    {
                        var returnValue = WorkerThread(tokenSource.Token);

                        if (returnValue != null)
                        {
                            tokenSource.Cancel();
                            foundHeader = new BlockHeader(returnValue);
                        }

                        if (0 == Interlocked.Decrement(ref running))
                            done.Set();
                    }).Start();
                }

#if DEBUG
                var sw = Stopwatch.StartNew();
                do
                {
                    if(done.WaitOne(1000)) break;
                    Console.Write($"\r[{_time:HH:mm} {_nonce}] {_nonce/sw.ElapsedMilliseconds/1000}M H/S");
                } while (true);
#else
                done.WaitOne();
#endif
            
                //Header is null if cancellation is requested or if all values for nonce are tried
                //Miner will run again if cancellation is not requested and header is null
                if (token.IsCancellationRequested) return null;
                return foundHeader ?? Start(token);
            }

            private void SetNonce(byte[] data, uint nonce)
                => BitConverter.GetBytes(nonce).CopyTo(data, NonceIdx);

            private byte[]? WorkerThread(CancellationToken token)
            {
                byte[] data = _data.ToArray();
                for (uint n = 0; _nonce < UInt32.MaxValue && !token.IsCancellationRequested; n = Interlocked.Increment(ref _nonce))
                {
                    SetNonce(data, n);
                    var hash = Hash.HASH_256(data);
                    if (_target.IsValid(hash))
                        return data;
                }

                return null;
            }
        }
    }
}