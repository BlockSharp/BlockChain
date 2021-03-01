using System.Collections.Generic;
using System.IO;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    public class TransactionIndexes : IndexFile<TransactionIndex>
    {
        public TransactionIndexes(string file) : base(file) {}
        
        public override IEnumerable<TransactionIndex> Read()
        {
            using var fs = new FileStream(File, FileMode.Open, FileAccess.Read);
            if (fs.Length % 28 != 0)
                throw new InvalidDataException("The index file seems to be corrupt.");

            byte[] buffer = new byte[28];
            while (fs.Position < fs.Length)
            {
                int read = fs.Read(buffer);
                if (read == buffer.Length)
                    yield return new TransactionIndex(buffer);
                else
                    break;    
            }
            
            fs.Close();
        }
    }
}