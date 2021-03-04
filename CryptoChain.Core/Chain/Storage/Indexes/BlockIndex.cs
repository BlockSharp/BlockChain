using LiteDB;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    public class BlockIndex
    {
        [BsonId]
        public byte[] Hash { get; set; }
        public int File { get; set; }
        public long Position { get; set; }
        public uint Height { get; set; }
    }
}