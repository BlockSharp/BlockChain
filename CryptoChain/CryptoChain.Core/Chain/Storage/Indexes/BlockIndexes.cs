using System;
using System.Collections.Generic;
using System.IO;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    public class BlockIndexes : IndexFile<BlockIndex>
    {
        public override IEnumerable<BlockIndex> Read()
        {
            using var fs = new FileStream(File, FileMode.Open, FileAccess.Read);
            if (fs.Length % 36 != 0)
                throw new InvalidDataException("The index file seems to be corrupt.");
            
            byte[] buffer = new byte[36];
            while (fs.Position < fs.Length)
            {
                int read = fs.Read(buffer);
                if (read == buffer.Length)
                    yield return new BlockIndex(buffer);
                else
                    break;
            }
            
            fs.Close();
        }

        public Dictionary<string, BlockIndexMeta> ToDictionary()
        {
            var indexes = new Dictionary<string, BlockIndexMeta>();
            foreach (var idx in Read())
                indexes.Add(Convert.ToBase64String(idx.CompressedHash), idx.Meta);
            return indexes;
        }

        public BlockIndexes(string file) : base(file)
        { }
    }
}