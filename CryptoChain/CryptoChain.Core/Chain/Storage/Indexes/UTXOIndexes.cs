using System;
using System.Collections.Generic;
using System.IO;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    public class UTXOIndexes : IndexFile<UTXOIndex>
    {
        public override IEnumerable<UTXOIndex> Read()
        {
            using var fs = new FileStream(File, FileMode.Open, FileAccess.Read);
            
            byte[] buffer1 = new byte[2];
            while (fs.Position < fs.Length)
            {
                if (fs.Read(buffer1) != buffer1.Length)
                    throw new ArgumentException("Failed to read UTXOIndex");
                ushort len = BitConverter.ToUInt16(buffer1);
                fs.Position -= 2;
                byte[] buffer = new byte[len + 22];
                if(fs.Read(buffer) != buffer.Length)
                    break;
                yield return new UTXOIndex(buffer);
            }
            
            fs.Close();
        }

        public UTXOIndexes(string file) : base(file)
        { }
    }
}