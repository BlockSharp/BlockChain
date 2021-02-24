using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Chain.Storage
{
    /// <summary>
    /// This code is no clean code, but is just used as an example how we can store blocks and implement the
    /// IBlockStore interface
    /// </summary>
    public class FileBlockStore : IBlockStore
    {
        private readonly string _path;
        private List<BlockFileHelper> _blockFiles;
        private Dictionary<string, (int file, long position)> _blockLocations;
        private readonly bool _autoIndex;

        public FileBlockStore(string path, bool autoIndex = true)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            _blockFiles = new List<BlockFileHelper>();
            _path = path;
            _blockLocations = new Dictionary<string, (int, long)>();
            _autoIndex = autoIndex;
            if (autoIndex)
                Index().Wait();
        }

        public async Task Index()
        {
            _blockFiles.Clear();
            _blockLocations.Clear();
            
            #if DEBUG
            Stopwatch sw = Stopwatch.StartNew();
            #endif
            int blkNr = 0;
            while (true)
            {
                var file = await BlockFileHelper.ReadAsync(_path, blkNr++);
                if(file == null)
                    break;
                
                foreach (var x in file.Headers)
                    _blockLocations.Add(Convert.ToHexString(x.Value.Hash), (file.Number, x.Key));
                
                _blockFiles.Add(file);
                #if DEBUG
                Console.WriteLine($"Indexed file #{file.Number} ({file.Headers.Count} headers, ^{file.From} - {file.To})");
                #endif
            }

            BlockHeight = _blockFiles.Any() ? _blockFiles.Max(x => x.To) : 0;
            
            #if DEBUG
            sw.Stop();
            Console.WriteLine($"Indexed {_blockFiles.Count} files in {sw.Elapsed}, total of {_blockFiles.Sum(x => x.Headers.Count)} headers");
            #endif
        }

        private BlockFileHelper GetFile(int number)
            => _blockFiles.First(x => x.Number == number);

        public uint BlockHeight { get; private set; }
        
        public async Task<Block?> GetBlock(byte[] hash)
        {
            var location = _blockLocations[Convert.ToHexString(hash)];
            var file = GetFile(location.file);
            var block = await file.ReadBlockAsync(_path, location.position, file.Headers[location.position]);
            return block;
        }

        public uint GetHeight(byte[] hash)
        {
            var location = _blockLocations[Convert.ToHexString(hash)];
            var file = GetFile(location.file);
            var header = file.Headers[location.position];
            return (uint)file.Headers.Values.ToList().IndexOf(header) + file.From;
        }
        
        public async Task<Block> GetBlock(uint blockHeight)
        {
            var file = _blockFiles.First(x => blockHeight >= x.From && blockHeight <= x.To);
            var desired = file.AtHeight(blockHeight);
            var block = await file.ReadBlockAsync(_path, desired.Key, desired.Value);
            if (block == null)
                throw new ArgumentException("Block not found");
            return block;
        }

        public BlockHeader GetHeader(uint blockHeight)
        {
            var file = _blockFiles.First(x => blockHeight >= x.From && blockHeight <= x.To);
            var desired = file.AtHeight(blockHeight);
            return desired.Value;
        }

        public IEnumerable<BlockHeader> GetHeaders()
            => _blockFiles.SelectMany(x => x.Headers.Values);

        public IEnumerable<BlockHeader> HeaderRange(uint from, uint to)
        {
            var files = _blockFiles.Where(x => x.To >= from && x.From <= to);
            foreach (var f in files)
            {
                for (uint i = Math.Max(from, f.From); i < Math.Min(to, f.To) + 1; i++)
                    yield return f.AtHeight(i).Value;
            }
        }

        public IEnumerable<Block> All()
        {
            foreach (var file in _blockFiles)
                foreach (var block in file.ReadBlocks(_path))
                    yield return block;
        }

        public IEnumerable<Block> Range(uint from, uint to)
        {
            var files = _blockFiles.Where(x => x.To >= from && x.From <= to);
            foreach (var f in files)
            {
                for (uint i = Math.Max(from, f.From); i < Math.Min(to, f.To) + 1; i++)
                {
                    var canidate = f.AtHeight(i);
                    var block = f.ReadBlockAsync(_path, canidate.Key, canidate.Value).Result;
                    yield return block ?? throw new ArgumentException("Block not found");
                }
            }
        }

        public async Task<uint> AddBlock(Block block)
        {
            var latest = _blockFiles.FirstOrDefault(x => x.To == _blockFiles.Max(y => y.To));
            if (latest != null && latest.Size + block.Length <= Constants.MaxBlockFileSize)
            {
                long pos = await latest.WriteBlock(block, _path);
                var idx = _blockFiles.IndexOf(latest);
                latest.Headers.Add(pos, block.Header);
                _blockFiles[idx] = latest;
                _blockLocations.Add(block.Hash.ToHexString(), (latest.Number, pos));
                BlockHeight++;
            }
            else
            {
                var nr = latest?.Number + 1 ?? 0;
                await BlockFileHelper.WriteAsync(new List<Block> {block}, BlockHeight ==0 ? 0 : ++BlockHeight, _path, nr);
                latest = await BlockFileHelper.ReadAsync(_path, nr);
                Debug.Assert(latest != null, nameof(latest) + " != null");
                _blockFiles.Add(latest);
                _blockLocations.Add(block.Hash.ToHexString(), (nr, BlockFileHelper.HeaderLength));
            }

            if(_autoIndex)
                await Index();
            
            return BlockHeight;
        }

        public void RemoveBlock(byte[] hash, bool cascade = true)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            if (_blockFiles.Count == 0)
                return "Store contains no blocks";
            
            var sb = new StringBuilder();
            sb.AppendLine("================= FileBlockStore stats =================");
            sb.AppendLine("Current blockHeight: " + BlockHeight);
            sb.AppendLine("Amount of blockFiles: " + _blockFiles.Count);
            sb.AppendLine("Total byte size: " + _blockFiles.Sum(x => x.Size));
            sb.AppendLine("Amount of blocks: " + _blockFiles.Sum(x => x.Count));
            sb.AppendLine($"Range of blocks: {_blockFiles.Min(x => x.From)} - {_blockFiles.Max(x => x.To)}");
            sb.AppendLine($"Oldest block: {GetHeader(0).Hash.ToHexString()}");
            sb.AppendLine($"Newest block: {GetHeader(BlockHeight).Hash.ToHexString()}");

            return sb.ToString();
        }
    }
    
    public class BlockFileHelper
    {
        public static readonly int HeaderLength = 8;
        
        public int Count => (int)(To - From) + 1;
        public uint From { get; set; }
        public uint To { get; set; }
        public Dictionary<long, BlockHeader> Headers { get; set; }
        public int Number { get; set; }
        public long Size { get; set; }
        
        public static string Name(int number)
            => "blk" + number.ToString().PadLeft(6, '0') + ".dat";

        public BlockFileHelper()
        {
            Headers = new Dictionary<long, BlockHeader>();
        }
        
        public KeyValuePair<long, BlockHeader> AtHeight(uint height)
            => Headers.ElementAt((int)height - (int)From);

        public static async Task WriteAsync(List<Block> blocks, uint height, string folder, int number)
        {
            string path = Path.Combine(folder, Name(number));
            await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            await fs.WriteAsync(BitConverter.GetBytes(height));
            await fs.WriteAsync(BitConverter.GetBytes((uint)(height + blocks.Count - 1)));
            foreach (var block in blocks)
            {
                await fs.WriteAsync(block.Serialize());
            }
            fs.Close();
        }

        public async Task<long> WriteBlock(Block block, string folder)
        {
            string path = Path.Combine(folder, Name(Number));
            await using var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            byte[] header = new byte[HeaderLength];
            if (await fs.ReadAsync(header) != HeaderLength)
                throw new ArgumentException("Failed to read header");
            uint to = BitConverter.ToUInt32(header[4..]) + 1;
            fs.Position -= 4;
            await fs.WriteAsync(BitConverter.GetBytes(to));
            fs.Seek(0, SeekOrigin.End);
            long loc = fs.Position;
            await fs.WriteAsync(block.Serialize());
            To++;
            Size = fs.Length;
            fs.Close();
            return loc;
        }

        public IEnumerable<Block> ReadBlocks(string folder)
        {
            var path = Path.Combine(folder, Name(Number));
            if (!File.Exists(path))
                throw new ArgumentException("Block file not found");
            
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Position += HeaderLength; //skip blockFile header
            
            byte[] bdl = new byte[4];
            while (fs.Position < fs.Length)
            {
                var pos = fs.Position;
                fs.Position++;
                if (fs.Read(bdl) != 4)
                    throw new ArgumentException("Failed to read data length");

                fs.Position = pos;
                int blockSize = BitConverter.ToInt32(bdl) + 5 + Constants.BlockHeaderLength;
                byte[] buffer = new byte[blockSize];
                if (fs.Read(buffer) != blockSize)
                    throw new ArgumentException("Failed to read desired block length");
                yield return new Block(buffer);
            }
            
            fs.Close();
        }

        public async Task<Block?> ReadBlockAsync(string folder, long position, BlockHeader? header)
        {
            var path = Path.Combine(folder, Name(Number));
            if (!File.Exists(path))
                return null;
            
            await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
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

        public static async Task<BlockFileHelper?> ReadAsync(string folder, int number)
        {
            var blockFile = new BlockFileHelper {Number = number};
            string path = Path.Combine(folder, Name(number));
            if (!File.Exists(path))
                return null;
            
            byte[] hbuffer = new byte[HeaderLength];
            await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            var read = await fs.ReadAsync(hbuffer, 0, hbuffer.Length);
            if (read != HeaderLength)
                throw new ArgumentException("Failed to read the file");

            blockFile.From = BitConverter.ToUInt32(hbuffer, 0);
            blockFile.To = BitConverter.ToUInt32(hbuffer, 4);
            blockFile.Size = fs.Length;

            byte[] buffer = new byte[4 + Constants.BlockHeaderLength];
            while (fs.Position < fs.Length)
            {
                var pos = fs.Position;
                fs.Position++;
                read = await fs.ReadAsync(buffer, 0, buffer.Length);
                if (read != buffer.Length)
                    throw new ArgumentException("Failed to read block header");
                
                int dataLength = BitConverter.ToInt32(buffer, 0);
                var header = new BlockHeader(buffer[4..]);
                blockFile.Headers.Add(pos, header);
                fs.Position += dataLength;
            }
            
            fs.Close();

            if (blockFile.Count != blockFile.Headers.Count)
                throw new ArgumentException($"The amount of blocks in the file ({blockFile.Headers.Count}) doesnt match the blockHeight range ({blockFile.Count})");
            
            return blockFile;
        }
    }
}