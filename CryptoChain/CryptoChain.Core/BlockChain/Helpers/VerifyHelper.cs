using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Block.Helpers;

namespace CryptoChain.Core.BlockChain.Helpers
{
    public static class VerifyHelper
    {
        /// <summary>
        /// Determines if all the blocks in the blockchain are valid
        /// </summary>
        /// <param name="blockChain">BlockChain to check</param>
        /// <param name="token">CancellationToken to stop checking. Use (CancellationTokenSource).Token</param>
        /// <returns>True is valid blockChain</returns>
        public static async Task<bool> Verify<T>(this BlockChain<T> blockChain, CancellationToken token = new CancellationToken()) where T : IBlockData, new()
            => await Task.Run(()=>_Verify(blockChain),token);

        private static bool _Verify<T>(BlockChain<T> blockChain) where T : IBlockData, new()
        {
            var sha256 = blockChain.GetHashingAlgorithm();
            var block = blockChain.First();
            foreach (var prevBlock in blockChain.Skip(1))
            {
                if (!block.IsValid(prevBlock.Hash(sha256), sha256)) return false;
                block = prevBlock;
            }

            return true;
        }
    }
}