using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockChain.Core.Block;

namespace BlockChain.Core
{
    public static class AddHelpers
    {
        /// <summary>
        /// Add new item to blockchain
        /// Block will be created and be mined
        /// </summary>
        /// <param name="blockChain">Instance of blockchain class</param>
        /// <param name="data">Data of block</param>
        /// <param name="target">Target of block, will be saved as bits in blockheader</param>
        /// <param name="token">CancellationToken to stop mining. Use (CancellationTokenSource).Token</param>
        public static async Task Add<T>(this BlockChain<T> blockChain, T data, Target target = null, CancellationToken token = new CancellationToken())
        where T : IBlockData, new()
        {
            var prevBlock = blockChain.First();
            target ??= prevBlock.GetBlockHeader().GetTarget();
            var block = Block<T>.Create(prevBlock.Hash(blockChain.GetHashingAlgorithm()),data,target, blockChain.GetHashingAlgorithm());
            
            block = await block.Mine(token);
            if(block != null) blockChain.Add(block);
        }
    }
}