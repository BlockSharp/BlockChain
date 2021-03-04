using LiteDB;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    public class TransactionIndex
    {
        [BsonId]
        public byte[] TxId { get; set; }
        public uint BlockHeight { get; set; }
        public ushort Index { get; set; }
    }
}