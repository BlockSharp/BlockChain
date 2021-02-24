using System;
using System.IO;
using System.Linq;
using System.Text;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;

namespace CryptoChain.Core.Blocks
{
    public class BlockHeader : ISerializable
    {
        public int Version { get; }
        public byte[] PrevHash { get; }
        public byte[] MerkleRoot { get; }
        public DateTime Timestamp { get; set; }
        public byte[] Bits { get; }
        public uint Nonce { get; set; }

        public Target Target => new (Bits);

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
        
        public int Length => Constants.BlockHeaderLength;
        
        /// <summary>
        /// Construct a new blockHeader
        /// </summary>
        /// <param name="prevHash">The hash of the previous block</param>
        /// <param name="merkleRoot">The merkle root or hash of the block data</param>
        /// <param name="bits">The shortened target</param>
        /// <param name="version">The block version (optional)</param>
        public BlockHeader(byte[] prevHash, byte[] merkleRoot, byte[] bits,
            int version = Constants.BlockVersion)
        {
            if (prevHash.Length != Constants.BlockHashLength)
                throw new ArgumentException("prevHash length must equal the BlockHashLength");

            if (merkleRoot.Length != Constants.TransactionHashLength)
                throw new ArgumentException("merkleRoot length must equal the TransactionHashLength");

            if (!Target.IsValidTarget(bits))
                throw new InvalidDataException("Target bits are not valid");
            
            Version = version;
            PrevHash = prevHash;
            MerkleRoot = merkleRoot;
            Bits = bits;
        }
        
        /// <summary>
        /// Construct a new blockHeader
        /// </summary>
        /// <param name="prevHash">The hash of the previous block</param>
        /// <param name="merkleRoot">The merkle root or hash of the block data</param>
        /// <param name="target">The target</param>
        /// <param name="version">The block version (optional)</param>
        public BlockHeader(byte[] prevHash, byte[] merkleRoot, Target target,
            int version = Constants.BlockVersion) : this(prevHash, merkleRoot, target.ToBits(), version){}
        

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
            var timestamp = BitConverter.ToUInt32(serialized, idx);
            Timestamp = DateTime.UnixEpoch.Add(TimeSpan.FromSeconds(timestamp)).ToUniversalTime();
            idx += 4;
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
            Buffer.BlockCopy(BitConverter.GetBytes((uint)(Timestamp.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds), 0, buffer, idx, 4);
            idx += 4;
            Buffer.BlockCopy(Bits, 0, buffer, idx, 4);
            idx += 4;
            Buffer.BlockCopy(BitConverter.GetBytes(Nonce), 0, buffer, idx, 4);
            return buffer;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("  ======================= Block Header =======================  ");
            sb.AppendLine($"  Version: {Version:X}");
            sb.AppendLine("  Prevhash: " + PrevHash.ToHexString());
            sb.AppendLine("  MerkeRoot: " + MerkleRoot.ToHexString());
            sb.AppendLine("  Timestamp: " + Timestamp.ToString("dd-MM-yyyy HH:mm:ss"));
            sb.AppendLine($"  Bits: [{string.Join(" ", Bits.Select(x => x.ToString("X"))).TrimEnd()}]");
            sb.AppendLine("  Nonce: " + Nonce);
            sb.AppendLine("  ============================================================  ");
            return sb.ToString();
        }
    }
}