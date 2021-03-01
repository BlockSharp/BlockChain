using System.Collections.Generic;
using System.IO;

namespace CryptoChain.Core.Abstractions
{
    public abstract class IndexFile<T> where T : ISerializable
    {
        protected readonly string File;

        public IndexFile(string file)
            => File = file;

        public abstract IEnumerable<T> Read();
        
        public void Write(params T[] indexes)
        {
            using var fs = new FileStream(File, FileMode.Append, FileAccess.Write);
            foreach (var idx in indexes)
                fs.Write(idx.Serialize());
            fs.Close();
        }
    }
}