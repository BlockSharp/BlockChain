using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    /// <summary>
    /// 34 MB for 1 million items. So this index can be stored in RAM
    /// This is stored in an index file. Every item is exactly 36 bytes
    /// </summary>
    public class BlockIndex : ISerializable
    {
        public byte[] CompressedHash { get; set; }
        public BlockIndexMeta Meta { get; set; }
        
        public int Length => 20 + Meta.Length;
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            CompressedHash.CopyTo(buffer, 0);
            Meta.Serialize().CopyTo(buffer, 20);
            return buffer;
        }
        
        public override string ToString()
        {
            return $"Compressed Hash: {Convert.ToHexString(CompressedHash)}\nHeight: {Meta.BlockHeight}\nFile: {Meta.File}\nPosition : {Meta.Position}";
        }

        public BlockIndex(byte[] hash, uint height, int file, long position)
        {
            CompressedHash = Hash.SHA_1(hash);
            Meta = new BlockIndexMeta
            {
                BlockHeight = height,
                File = file,
                Position = position
            };
        }

        public BlockIndex(byte[] serialized)
        {
            CompressedHash = serialized[..20];
            Meta = new BlockIndexMeta(serialized[20..]);
        }
    }

    public class BlockIndexMeta : ISerializable
    {
        public uint BlockHeight { get; set; }
        public int File { get; set; }
        public long Position { get; set; }
        
        public int Length => 4 + 4 + 8;
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            BitConverter.GetBytes(BlockHeight).CopyTo(buffer, 0);
            BitConverter.GetBytes(File).CopyTo(buffer, 4);
            BitConverter.GetBytes(Position).CopyTo(buffer, 8);
            return buffer;
        }
        
        public BlockIndexMeta(){}
        public BlockIndexMeta(byte[] serialized)
        {
            BlockHeight = BitConverter.ToUInt32(serialized, 0);
            File = BitConverter.ToInt32(serialized, 4);
            Position = BitConverter.ToInt64(serialized, 8);
        }
    }
}