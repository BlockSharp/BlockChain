using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Block
{
    public class Block : ISerializable
    {
        public int Length => Header.Length + Data.Length;
        
        public BlockHeader Header { get; set; }
        public byte[] Data { get; set; }
        
        public Block(byte[] serialized)
        {
            int idx = 0;
            int dataLength = BitConverter.ToInt32(serialized);
            idx += 4;
            Header = new BlockHeader(Serialization.FromBuffer(serialized, idx, false, serialized.Length - dataLength));
            idx += serialized.Length - dataLength;
            Data = Serialization.FromBuffer(serialized, idx, false);
        }

        //todo: make contructors and create functions for block and blockheader

        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            int idx = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(Data.Length), 0, buffer, idx, 4);
            idx += 4;
            buffer.AddSerializable(Header, idx, false);
            idx += Header.Length;
            Buffer.BlockCopy(Data, 0, buffer, idx, Data.Length);
            return buffer;
        }
    }
}