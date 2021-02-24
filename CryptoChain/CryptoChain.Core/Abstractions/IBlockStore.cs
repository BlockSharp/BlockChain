using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoChain.Core.Blocks;

namespace CryptoChain.Core.Abstractions
{
    public interface IBlockStore
    {
        /// <summary>
        /// The current block height (amount of blocks)
        /// </summary>
        public uint BlockHeight { get; }
        
        /// <summary>
        /// Get a block by its hash
        /// </summary>
        /// <param name="hash">The blockHash</param>
        /// <returns>The desired block</returns>
        public Task<Block?> GetBlock(byte[] hash);

        /// <summary>
        /// Get a block height by its header
        /// </summary>
        /// <param name="hash">The hash of the block</param>
        /// <returns>The height of the desired block</returns>
        public uint GetHeight(byte[] hash);
        
        /// <summary>
        /// Get a block by its blockHeight. Note that this can differ per node
        /// </summary>
        /// <param name="blockHeight">The desired blockHeight</param>
        /// <returns>The desired block</returns>
        public Task<Block> GetBlock(uint blockHeight);

        /// <summary>
        /// Get header from specific block height
        /// </summary>
        /// <param name="blockHeight">The desired height</param>
        /// <returns>a blockHeader</returns>
        public BlockHeader GetHeader(uint blockHeight);

        /// <summary>
        /// Get all headers from the store
        /// </summary>
        /// <returns>List of all blockHeaders present</returns>
        public IEnumerable<BlockHeader> GetHeaders();

        /// <summary>
        /// Get range of headers
        /// </summary>
        /// <param name="from">Minimum block height</param>
        /// <param name="to">Maximum block height</param>
        /// <returns>IEnumerable of headers</returns>
        public IEnumerable<BlockHeader> HeaderRange(uint from, uint to);
        
        /// <summary>
        /// Enumerate over all blocks. Use with care!
        /// </summary>
        /// <returns>IEnumerable for all blocks</returns>
        public IEnumerable<Block> All();
        
        /// <summary>
        /// Enumerate over all blocks in a block height range
        /// </summary>
        /// <param name="from">The minimum height</param>
        /// <param name="to">The maximum height</param>
        /// <returns>Block IEnumerable</returns>
        public IEnumerable<Block> Range(uint from, uint to);
        
        /// <summary>
        /// Add a block to the storage system
        /// </summary>
        /// <param name="block">The block to be added</param>
        /// <returns>The height of the placed block</returns>
        public Task<uint> AddBlock(Block block);
        
        /// <summary>
        /// Remove block from the storage system
        /// </summary>
        /// <param name="hash">The hash of the block you want to delete</param>
        /// <param name="cascade">Indicates if you want to remove all dependents</param>
        public void RemoveBlock(byte[] hash, bool cascade = true);
    }
}