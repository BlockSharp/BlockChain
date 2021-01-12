using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Block
{
    public class BlockHeader : ISerializable
    {
        public int Version { get; set; }
        public byte[] PrevHash { get; set; }
        public byte[] MerkleRoot { get; set; }
        public DateTime Timestamp { get; set; }
        public byte[] Bits { get; set; }
        public uint Nonce { get; set; }

        public Target Target => new Target(Bits);

        private byte[]? _blockHash;

        /// <summary>
        /// Get the blockHash or blockId
        /// </summary>
        public byte[] Hash
        {
            get
            {
                if (_blockHash == null)
                    _blockHash = Cryptography.Hashing.Hash.HASH_256(Serialize());
                return _blockHash;
            }
        }
        
        public int Length => 4 + Constants.BlockHashLength + Constants.TransactionHashLength + 8 + 4 + 4;
        

        /// <summary>
        /// Deserialize blockHeader from bytes
        /// </summary>
        /// <param name="serialized">The serialized blockHeader</param>
        public BlockHeader(byte[] serialized)
        {
            int idx = 0;
            Version = BitConverter.ToInt32(serialized, idx);
            idx += 4;
            PrevHash = Serialization.FromBuffer(serialized, idx, false, Constants.BlockHashLength);
            idx += Constants.BlockHashLength;
            MerkleRoot = Serialization.FromBuffer(serialized, idx, false, Constants.TransactionHashLength);
            idx += Constants.TransactionHashLength;
            Timestamp = new DateTime(BitConverter.ToInt64(serialized, idx));
            idx += 8;
            Bits = Serialization.FromBuffer(serialized, idx, false, 4);
            idx += 4;
            Nonce = BitConverter.ToUInt32(serialized, idx);
        }
        
        public byte[] Serialize()
        {
            byte[] buffer = new byte[Length];
            int idx = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(Version), 0, buffer, idx, 4);
            idx += 4;
            Buffer.BlockCopy(PrevHash, 0, buffer, idx, Constants.BlockHashLength);
            idx += Constants.BlockHashLength;
            Buffer.BlockCopy(MerkleRoot, 0, buffer, idx, Constants.TransactionHashLength);
            idx += Constants.TransactionHashLength;
            Buffer.BlockCopy(BitConverter.GetBytes(Timestamp.Ticks), 0, buffer, idx, 8);
            idx += 8;
            Buffer.BlockCopy(Bits, 0, buffer, idx, 4);
            idx += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Nonce), 0, buffer, idx, 4);
            return buffer;
        }
    }
}