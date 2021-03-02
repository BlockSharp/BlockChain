using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain.Storage.Indexes;

namespace CryptoChain.Core.Chain.Storage
{
    /// <summary>
    /// This class provides an interface to store blocks in, a blkXXXXXX.dat file
    /// It also stores the blockHeight (from and to)
    /// </summary>
    public class BlockFile
    {
        private readonly string _folder;
        
        /// <summary>
        /// The number of the file. For instance, blk0000x where x = Number
        /// </summary>
        public int Number { get; set; }
        
        /// <summary>
        /// The blockHeight this file starts with (if it contains the genesis block, it will start at 1)
        /// </summary>
        public uint From { get; set; }
        
        /// <summary>
        /// The end of the block range. This is inclusive, this means:
        /// From = 1, To = 3 contains blocks at height 1,2 and 3
        /// </summary>
        public uint To { get; set; }
        
        /// <summary>
        /// The amount of blocks in the file
        /// </summary>
        public uint Count => To == 0 ? 0 : To - From + 1;
        
        /// <summary>
        /// Get byte size of file
        /// </summary>
        public long Size { get; private set; }

        private readonly int _headerSize = 8;

        /// <summary>
        /// Indicate if file contains block at specific height
        /// </summary>
        /// <param name="blockHeight">The block's height</param>
        /// <returns>True if the file contains the block at specific height</returns>
        public bool Contains(uint blockHeight)
            => From >= blockHeight && To <= blockHeight;

        /// <summary>
        /// The location the block is stored
        /// </summary>
        public string Location
            => Path.Combine(_folder, "blk" + Number.ToString().PadLeft(6, '0') + ".dat");

        /// <summary>
        /// Create a new BlockFile or open an existing one
        /// </summary>
        /// <param name="folder">The folder, like /home/user/.CryptoChain/blocks</param>
        /// <param name="number">The number of the file</param>
        /// <param name="currentHeight">The current block height</param>
        /// <exception cref="ArgumentException">Exception is thrown when file exists and is empty/does not contain header</exception>
        public BlockFile(string folder, int number, uint currentHeight = 0)
        {
            _folder = folder;
            Number = number;
            if (Exists)
            {
                byte[] header = new byte[_headerSize];
                using var fs = new FileStream(Location, FileMode.Open, FileAccess.Read);
                Size = fs.Length;
                if (fs.Read(header) != _headerSize)
                    throw new ArgumentException("Failed to read header");
                fs.Close();
                From = BitConverter.ToUInt32(header, 0);
                To = BitConverter.ToUInt32(header, 4);
            }
            else
            {
                From = currentHeight + 1;
                To = 0;
                Size = 0;
            }
        }

        /// <summary>
        /// Indicates if the file exists
        /// </summary>
        public bool Exists => File.Exists(Location);

        /// <summary>
        /// Write one or more blocks to the file. This increases the blockHeight (To parameter)
        /// </summary>
        /// <param name="blocks">The blocks to be written</param>
        /// <returns>Array of BlockIndexes</returns>
        public async Task<BlockIndex[]> Write(params Block[] blocks)
        {
            if (To == 0)
                To = From - 1;
            var nr = To;
            To += (uint)blocks.Length;

            var indexes = new List<BlockIndex>();
            await using var fs = new FileStream(Location, FileMode.OpenOrCreate, FileAccess.Write);
            await fs.WriteAsync(BitConverter.GetBytes(From));
            await fs.WriteAsync(BitConverter.GetBytes(To));
            fs.Seek(0, SeekOrigin.End);
            foreach (var b in blocks)
            {
                indexes.Add(new BlockIndex(b.Hash, ++nr, Number, fs.Position));
                await fs.WriteAsync(b.Serialize());
            }

            Size = fs.Length;
            fs.Close();
            return indexes.ToArray();
        }

        /// <summary>
        /// Read blocks from the file
        /// </summary>
        /// <param name="startAt">Position to start stream at</param>
        /// <returns>IEnumerable with blocks</returns>
        /// <exception cref="ArgumentException">Exception is thrown when the file is invalid</exception>
        public IEnumerable<Block> Read(long startAt = 0)
        {
            using var fs = new FileStream(Location, FileMode.Open, FileAccess.Read);
            if (startAt != 0)
                fs.Position = startAt;
            else
                fs.Position += _headerSize;

            byte[] sizeBuffer = new byte[4];
            while (fs.Position < fs.Length)
            {
                fs.Position++;
                if (fs.Read(sizeBuffer) != 4)
                    throw new ArgumentException("Failed to read block data size");
                
                fs.Position -= 5;
                int blockSize = BitConverter.ToInt32(sizeBuffer) + 5 + Constants.BlockHeaderLength;
                byte[] buffer = new byte[blockSize];
                if (fs.Read(buffer) != blockSize)
                    throw new ArgumentException("Failed to read desired block length");
                yield return new Block(buffer);
            }
            fs.Close();
        }

        /// <summary>
        /// Create indexes from block file
        /// </summary>
        /// <returns>BlockIndex IEnumerable</returns>
        public IEnumerable<BlockIndex> CreateIndexes()
        {
            using var fs = new FileStream(Location, FileMode.Open, FileAccess.Read);
            fs.Position += _headerSize;

            uint height = From;

            byte[] sizeBuffer = new byte[4];
            while (fs.Position < fs.Length)
            {
                var pos = fs.Position;
                fs.Position++;
                if (fs.Read(sizeBuffer) != 4)
                    throw new ArgumentException("Failed to read block data size");

                var buffer = new byte[Constants.BlockHeaderLength];
                if (fs.Read(buffer) != buffer.Length)
                    throw new ArgumentException("Failed to read block header");

                var header = new BlockHeader(buffer);
                fs.Position += BitConverter.ToInt32(sizeBuffer);

                yield return new BlockIndex(header.Hash, height++, Number, pos);
            }
            fs.Close();
        }
        
        /// <summary>
        /// Read headers from the file
        /// </summary>
        /// <returns>IEnumerable with BlockHeaders</returns>
        /// <exception cref="ArgumentException">Thrown when data is invalid</exception>
        public IEnumerable<BlockHeader> ReadHeaders()
        {
            using var fs = new FileStream(Location, FileMode.Open, FileAccess.Read);
            fs.Position += _headerSize;

            var buffer = new byte[Constants.BlockHeaderLength];
            var sizeBuffer = new byte[4];
            while (fs.Position < fs.Length)
            {
                fs.Position++;
                if (fs.Read(sizeBuffer) != 4)
                    throw new ArgumentException("Failed to read data size");

                if (fs.Read(buffer) != buffer.Length)
                    throw new ArgumentException("Failed to read desired block length");
                yield return new BlockHeader(buffer);

                fs.Position += BitConverter.ToInt32(sizeBuffer);
            }
            fs.Close();
        }
        
        /// <summary>
        /// Read single block from file with given byte position
        /// </summary>
        /// <param name="position">The position</param>
        /// <param name="header">The header, optional. If provided it won't be read from the file</param>
        /// <returns>Block, if position does not exist NULL</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Block?> ReadBlockAsync(long position, BlockHeader? header = null)
        {
            await using var fs = new FileStream(Location, FileMode.Open, FileAccess.Read);
            if (fs.Length <= position + 85)
                return null;
                
            fs.Position = position;
            var type = (BlockDataIdentifier) fs.ReadByte();
            byte[] lbr = new byte[4];
            if (await fs.ReadAsync(lbr) != 4)
                throw new ArgumentException("Failed to read data length");
            
            int dataLength = BitConverter.ToInt32(lbr);
            if (header == null)
            {
                byte[] hbr = new byte[Constants.BlockHeaderLength];
                if (await fs.ReadAsync(hbr) != hbr.Length)
                    throw new ArgumentException("Failed to read header");
                header = new BlockHeader(hbr);
            }
            else
            {
                fs.Position += Constants.BlockHeaderLength;
            }

            byte[] blockData = new byte[dataLength];
            if (await fs.ReadAsync(blockData) != dataLength)
                throw new ArgumentException("Failed to read block data");

            fs.Close();
            return new Block(blockData, header, type);
        }
    }
}