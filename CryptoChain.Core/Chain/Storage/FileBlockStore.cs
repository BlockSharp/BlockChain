using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain.Storage.Indexes;
using CryptoChain.Core.Helpers;
using LiteDB;

namespace CryptoChain.Core.Chain.Storage
{
    public class FileBlockStore : IBlockStore, IDisposable
    {
        private readonly string _path;
        private readonly LiteDatabase _indexDb;
        public readonly ILiteCollection<BlockIndex> _blockIndexes;
        private readonly List<BlockFile> _blockFiles;

        public FileBlockStore(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            _path = path;
            _indexDb = new LiteDatabase(Path.Combine(path, "block_idx.db"));
            _blockIndexes = _indexDb.GetCollection<BlockIndex>("block_idx");
            _blockIndexes.EnsureIndex(x => x.Height);
            _blockFiles = new List<BlockFile>();
            Index();
        }

        private void Index()
        {
            Stopwatch sw = Stopwatch.StartNew();
            int i;
            for (i = 0; i <= 999999; i++)
            {
                var bf = new BlockFile(_path, i);
                if(!bf.Exists)
                    break;
                _blockFiles.Add(bf);
            }
            DebugUtils.Info($"Indexed {i} blockFiles finished in {sw.Elapsed}");
        }

        public async Task CreateIndexes()
        {
            await Task.Run(() =>
            {
                _indexDb.DropCollection("block_idx");
                Stopwatch sw = Stopwatch.StartNew();
                
                var indexes = new List<BlockIndex>(100);
                int j = 0;
                for (int i = 0; i < _blockFiles.Count; i++)
                {
                    var f = _blockFiles[i];
                    DebugUtils.Log("Creating indexes for blockFile #"+f.Number);
                    indexes.AddRange(f.CreateIndexes());
                    if (j++ >= 100)
                    {
                        j = 0;
                        _blockIndexes.InsertBulk(indexes);
                        indexes.Clear();
                    }
                }

                if (indexes.Any())
                    _blockIndexes.InsertBulk(indexes);
                
                DebugUtils.Info("Creating indexes finished in "+sw.Elapsed);
            });
        }
        
        public uint BlockHeight => _blockFiles.Any() ? _blockFiles.Max(x => x.To) : 0;

        private BlockIndex? GetMeta(byte[] hash)
            => _blockIndexes.FindById(hash);

        private BlockIndex? GetMeta(uint blockHeight)
            => _blockIndexes.FindOne(x => x.Height == blockHeight);

        public async Task<Block?> GetBlock(byte[] hash)
        {
            var meta = GetMeta(hash);
            if (meta == null) return null;
            var file = _blockFiles.First(x => x.Number == meta.File);
            return await file.ReadBlockAsync(meta.Position);
        }

        public uint GetHeight(byte[] hash)
            => GetMeta(hash)?.Height ?? 0;

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
            => _blockFiles.OrderBy(x => x.To).SelectMany(x => x.Read());

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
                _blockIndexes.InsertBulk(res);
            }
        }

        public Task RemoveBlock(byte[] hash, bool cascade = true)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            _indexDb.Dispose();
        }
    }
}