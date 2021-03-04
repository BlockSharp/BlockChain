using System.Collections.Generic;
using LiteDB;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    public class UtxoIndex
    {
        [BsonId]
        public byte[] TxId { get; set; }
        public List<ushort> UnspentOutputs { get; set; }
    }
}