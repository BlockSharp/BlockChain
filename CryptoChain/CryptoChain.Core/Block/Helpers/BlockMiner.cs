using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Block.Helpers
{
    public static class BlockMiner
    {
        private static int _nonce;

        /// <summary>
        /// Mine a block
        /// </summary>
        /// <param name="block">Block to mine</param>
        /// <param name="token">CancellationToken to stop mining. Use (CancellationTokenSource).Token</param>
        /// <param name="threadCount">Amount of worker threads to span, if smaller then 1 or not set all cores will be used</param>
        /// <returns>Mined block, or null if canceled</returns>
        public static async Task<Block<T>> Mine<T>(this Block<T> block, CancellationToken token = new CancellationToken(),
            int threadCount = 0) where T : IBlockData, new()
        {
            var result = await block.GetBlockHeader().Mine(token,threadCount);
            if (result == null) return null;
            return new Block<T>(result.ToArray(), block.Data);
        }
        /// <summary>
        /// Mine a block header
        /// </summary>
        /// <param name="header">Block header to be mined</param>
        /// <param name="token">CancellationToken to stop mining. Use (CancellationTokenSource).Token</param>
        /// <param name="threadCount">Amount of worker threads to span, if smaller then 1 or not set all cores will be used</param>
        /// <returns>Mined block header, or null if canceled</returns>
        public static async Task<BlockHeader> Mine(this BlockHeader header, CancellationToken token = new CancellationToken(),
            int threadCount = 0)
            => await Task.Run(() => StartMiner(header, token, threadCount), token);

        /// <summary>
        /// Start and handle the worker threads
        /// </summary>
        /// <param name="header">Block header to be mined</param>
        /// <param name="token">CancellationToken to stop mining</param>
        /// <param name="threadCount">Amount of worker threads to span, if smaller then 1 or not set all cores will be used</param>
        /// <returns>Mined block header, or null if canceled</returns>
        private static BlockHeader StartMiner(BlockHeader header, CancellationToken token, int threadCount = 0)
        {
            if (threadCount <= 0) threadCount = Environment.ProcessorCount;
            
            var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            using var done = new ManualResetEvent(false);
            var running = threadCount;

            BlockHeader foundHeader = null;
            header.SetTime(DateTime.UtcNow);
            _nonce = 0;

            for (int i = 0; i < threadCount; i++)
            {
                new Thread(() =>
                {
                    var returnValue = WorkerThread(header.Clone(), tokenSource.Token);

                    if (returnValue != null)
                    {
                        tokenSource.Cancel();
                        foundHeader = returnValue;
                    }

                    if (0 == Interlocked.Decrement(ref running)) done.Set();
                }).Start();
            }

#if DEBUG
            var sw = Stopwatch.StartNew();
            do
            {
                if(done.WaitOne(1000)) break;
                Console.Write($"\r[{header.GetTime():HH:mm} {_nonce}] {_nonce/sw.ElapsedMilliseconds/1000}M H/S");
            } while (true);
#else
            done.WaitOne();
#endif
            
            //Header is null if cancellation is requested or if all values for nonce are tried
            //Miner will run again if cancellation is not requested and header is null
            if (token.IsCancellationRequested) return null;
            return foundHeader ?? StartMiner(header, token, threadCount);
        }

        /// <summary>
        /// Worker thread, multiple are spanned
        /// ! Make deep clone of header. This function is not thread safe !
        /// </summary>
        /// <param name="header">Block header to mine</param>
        /// <param name="token">CancellationToken to stop mining.</param>
        /// <returns>null or mined block</returns>
        private static BlockHeader WorkerThread(BlockHeader header, CancellationToken token)
        {
            var target = header.GetTarget();
            using var sha256 = SHA256.Create();
            
            for (int n = 0; _nonce != -1 && !token.IsCancellationRequested; n = Interlocked.Increment(ref _nonce))
            {
                header.SetNonce(n);
                var hash = header.Hash(sha256);
                if (target.IsValid(hash)) return header;
            }
            return null;
        }
    }
}