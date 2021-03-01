using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Hashing;

namespace CryptoChain.Core.Chain.Storage.Indexes
{
    /// <summary>
    /// 28 GB for 1 billion transactions. So this won't be stored in RAM
    /// Every time you want to find a transaction you have to loop the index file
    /// This is stored into an index file. Every item is exactly 28 bytes
    /// </summary>
    public class TransactionIndex : ISerializable
    {
        public byte[] CompressedHash { get; set; }
        public uint BlockHeight { get; set; }
        public int Index { get; set; }
        public int Length => 20 + 4 + 4;
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            CompressedHash.CopyTo(buffer, 0);
            BitConverter.GetBytes(BlockHeight).CopyTo(buffer, 20);
            BitConverter.GetBytes(Index).CopyTo(buffer, 24);
            return buffer;
        }

        public override string ToString()
        {
            return $"Compressed Hash: {Convert.ToHexString(CompressedHash)}\nHeight: {BlockHeight}\nIndex: {Index}";
        }

        public TransactionIndex(byte[] hash, uint blockHeight, int index)
        {
            CompressedHash = Hash.SHA_1(hash);
            BlockHeight = blockHeight;
            Index = index;
        }

        public TransactionIndex(byte[] serialized)
        {
            CompressedHash = serialized[..20];
            BlockHeight = BitConverter.ToUInt32(serialized, 20);
            Index = BitConverter.ToInt32(serialized, 24);
        }
    }
}