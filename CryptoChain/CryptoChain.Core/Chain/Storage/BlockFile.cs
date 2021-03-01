using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain.Storage.Indexes;

namespace CryptoChain.Core.Chain.Storage
{
    public class BlockFile
    {
        private readonly string _folder;
        public int Number { get; set; }
        public uint From { get; set; }
        public uint To { get; set; }
        public uint Count => To - From;

        private readonly int _headerSize = 8;

        public string Location
            => Path.Combine(_folder, "blk" + Number.ToString().PadRight(6, '0') + ".dat");

        public BlockFile(string folder, int number)
        {
            _folder = folder;
            Number = number;
        }

        public async Task<BlockIndex[]> Write(params Block[] blocks)
        {
            uint current = To;
            To += (uint)blocks.Length;
            var indexes = new List<BlockIndex>();
            await using var fs = new FileStream(Location, FileMode.OpenOrCreate, FileAccess.Write);
            await fs.WriteAsync(BitConverter.GetBytes(From));
            await fs.WriteAsync(BitConverter.GetBytes(To));
            fs.Seek(0, SeekOrigin.End);
            foreach (var b in blocks)
            {
                indexes.Add(new BlockIndex(b.Hash, current++, Number, fs.Position));
                await fs.WriteAsync(b.Serialize());
            }
            fs.Close();
            return indexes.ToArray();
        }

        public IEnumerable<Block> Read()
        {
            using var fs = new FileStream(Location, FileMode.Open, FileAccess.Read);
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
        
        public async Task<Block?> ReadBlockAsync(long position, BlockHeader? header)
        {
            await using var fs = new FileStream(Location, FileMode.Open, FileAccess.Read);
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