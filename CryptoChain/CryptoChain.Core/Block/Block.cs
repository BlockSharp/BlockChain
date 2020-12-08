/* https://en.bitcoin.it/wiki/Block
 *
 * [BlockHeader]            84 bytes
 * [BlockData]              * bytes
 * [BlockLength]            4 bytes
 */

using System;
using System.IO;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Block
{
    public class Block<T> where T : IBlockData, new()
    {
        /// <summary>
        /// Contains all the data of this block
        /// </summary>
        private readonly byte[] _blockData;
        public byte[] ToArray() => _blockData;

        /// <summary> (84 bytes)
        /// The blockHeader of this block
        /// See BlockHeader for more detail
        /// </summary>
        public byte[] BlockHeader => _blockData[..CryptoChain.Core.Block.BlockHeader.Size];
        public BlockHeader GetBlockHeader() => new BlockHeader(BlockHeader);

        /// <summary> (x bytes)
        /// The current data of this block
        /// </summary>
        public byte[] Data => _blockData[CryptoChain.Core.Block.BlockHeader.Size..^4];
        public T GetData()
        {
            var data = new T();
            data.FromArray(Data);
            return data;
        }

        /// <summary> (4 bytes)
        /// The current size of this block
        /// </summary>
        public byte[] Size => _blockData[^4..];
        public int GetSize() => BitConverter.ToInt32(Size);

        /// <summary>
        /// Construct(Deserialize) block from a byte[]
        /// </summary>
        /// <param name="blockData">byte[] of a block</param>
        public Block(byte[] blockData)
        {
            if (blockData == null || blockData.Length < CryptoChain.Core.Block.BlockHeader.Size + 4)
                throw new InvalidDataException("Invalid data size");
            _blockData = blockData;
        }

        /// <summary>
        /// Create a new block
        /// </summary>
        /// <param name="hashPrevBlock">Hash from previous block</param>
        /// <param name="data">Data of this block</param>
        /// <param name="target">Target of this block, will be saved as bits in blockheader</param>
        /// <param name="sha256">sha256 instance to create hash of blockData</param>
        /// <returns>New instance of a Block</returns>
        public static Block<TD> Create<TD>(byte[] hashPrevBlock, TD data, Target target, SHA256 sha256)
            where TD : IBlockData, new()
            => new Block<TD>(CryptoChain.Core.Block.BlockHeader.Create(hashPrevBlock, data, target, sha256), data);
        public Block(BlockHeader header, T data) : this(header.ToArray(), data.ToArray()) { }
        /// <summary>
        /// Create a new block
        /// </summary>
        /// <param name="blockHeader">blockHeader of this block</param>
        /// <param name="data">byte[] of data to be saved in block</param>
        public Block(byte[] blockHeader, byte[] data)
        {
            if(blockHeader == null || blockHeader.Length != CryptoChain.Core.Block.BlockHeader.Size)
                throw new InvalidDataException("blockHeader has an invalid size");
            if(data == null) throw new NullReferenceException("Data is null");
            
            _blockData = new byte[CryptoChain.Core.Block.BlockHeader.Size + data.Length + 4];
            Buffer.BlockCopy(blockHeader, 0, _blockData, 0, CryptoChain.Core.Block.BlockHeader.Size); //Write header into block
            Buffer.BlockCopy(data, 0, _blockData, CryptoChain.Core.Block.BlockHeader.Size, data.Length); //Write data into block
            Buffer.BlockCopy(BitConverter.GetBytes(_blockData.Length), 0, _blockData,
                CryptoChain.Core.Block.BlockHeader.Size + data.Length, 4); //Write size into block
        }
    }
}