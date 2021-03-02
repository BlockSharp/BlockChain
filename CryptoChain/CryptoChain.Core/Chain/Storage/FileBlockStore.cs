using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain.Storage.Indexes;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Chain.Storage
{
    public class FileBlockStore : IBlockStore
    {
        private readonly string _path;
        private BlockIndexes _blockIndexes;
        private List<BlockFile> _blockFiles;

        private readonly bool _memoryBlockIndex;
        private Dictionary<string, BlockIndexMeta> memoryIndexes;
        

        public FileBlockStore(string path, BlockIndexes indexes, bool memoryBlockIndex = true)
        {
            _path = path;
            _blockIndexes = indexes;
            _blockFiles = new List<BlockFile>();
            _memoryBlockIndex = memoryBlockIndex;
            
            Index();
            
            if (memoryBlockIndex)
                memoryIndexes = _blockIndexes.ToDictionary();
        }

        private void Index()
        {
            for (int i = 0; i <= 999999; i++)
            {
                var bf = new BlockFile(_path, i);
                if(!bf.Exists)
                    break;
                Console.WriteLine("Indexed ["+bf.Number+"] "+bf.From+"-"+bf.To);
                _blockFiles.Add(bf);
            }
        }

        public async Task CreateIndexes()
        {
            //1. delete file
            await Task.Run(() =>
            {
                foreach (var f in _blockFiles)
                    _blockIndexes.Write(f.CreateIndexes().ToArray());
            });
        }

        private string GetCompressed(byte[] hash)
            => Convert.ToBase64String(Hash.SHA_1(hash));

        public uint BlockHeight => _blockFiles.Any() ? _blockFiles.Max(x => x.To) : 0;

        private IEnumerable<BlockIndex> Indexes()
            => _memoryBlockIndex ? memoryIndexes.Select(x => new BlockIndex(x.Key, x.Value)) : _blockIndexes.Read(); 
        
        private BlockIndexMeta? GetMeta(byte[] hash)
        {
            var compHash = Hash.SHA_1(hash);
            var compr = GetCompressed(hash);
            return _memoryBlockIndex
                ? (memoryIndexes.ContainsKey(compr) ? memoryIndexes[GetCompressed(hash)] : null)
                : Indexes().FirstOrDefault(x => x.CompressedHash.SequenceEqual(compHash))?.Meta;
        }

        private BlockIndexMeta? GetMeta(uint blockHeight)
        {
            return _memoryBlockIndex
                ? memoryIndexes.Values.FirstOrDefault(x => x.BlockHeight == blockHeight)
                : Indexes().FirstOrDefault(x => x.Meta.BlockHeight == blockHeight)?.Meta;
        }

        public async Task<Block?> GetBlock(byte[] hash)
        {
            var meta = GetMeta(hash);
            if (meta == null) return null;
            var file = _blockFiles.First(x => x.Number == meta.File);
            return await file.ReadBlockAsync(meta.Position);
        }

        public uint GetHeight(byte[] hash)
            => GetMeta(hash)?.BlockHeight ?? 0;

        public async Task<Block?> GetBlock(uint blockHeight)
        {
            var meta = GetMeta(blockHeight);
            if (meta == null) return null;
            var file = _blockFiles.First(x => x.Number == meta.File);
            return await file.ReadBlockAsync(meta.Position);
        }

        public IEnumerable<BlockHeader> GetHeaders()
            => _blockFiles.SelectMany(x => x.ReadHeaders());

        public IEnumerable<Block> All()
            => _blockFiles.SelectMany(x => x.Read());

        public IEnumerable<Block> Range(uint from, uint to)
        {
            int at = 0;
            int cnt = (int)(to - from) + 1;
            var files = _blockFiles.Where(x => x.To >= from && x.From <= to).OrderBy(x => x.From).ToList();
            var firstIdx = GetMeta(from) ?? throw new ArgumentException("Failed to get meta information from first block");
            for (int i = 0; i < files.Count; i++)
            {
                foreach (var b in files[i].Read(i == 0 ? firstIdx.Position : 0))
                {
                    yield return b;
                    if(++at >= cnt)
                        break;
                }
            }
        }

        public async Task AddBlocks(params Block[] blocks)
        {
            if(!_blockFiles.Any())
                _blockFiles.Add(new BlockFile(_path, 0, BlockHeight));
            int idx = _blockFiles.Count - 1;
            
            int number = _blockFiles[idx].Number;
            foreach (var block in blocks)
            {
                if (_blockFiles[idx].Size + block.Length > Constants.MaxBlockFileSize)
                {
                    _blockFiles.Add(new BlockFile(_path, ++number, BlockHeight));
                    idx++;
                }

                var res = await _blockFiles[idx].Write(block);
                _blockIndexes.Write(res);
                
                if(_memoryBlockIndex)
                    memoryIndexes.Add(GetCompressed(block.Hash), res[0].Meta);
            }
        }

        public Task RemoveBlock(byte[] hash, bool cascade = true)
        {
            throw new System.NotImplementedException();
        }
    }
}