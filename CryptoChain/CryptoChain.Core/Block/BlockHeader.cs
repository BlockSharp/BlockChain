/* https://en.bitcoin.it/wiki/Block_hashing_algorithm
 *
 * [Version]                4 bytes
 * [HashPrevBlock]          32 bytes
 * [HashMerkleRoot]         32 bytes
 * [Time]                   8 bytes
 * [Bits]                   4 bytes
 * [Nonce]                  4 bytes
 */

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Block
{
    public class BlockHeader
    {
        public const byte Size = 84;

        /// <summary> (84 bytes)
        /// Contains all the data of this block header
        /// </summary>
        private readonly byte[] _headerData;
        public byte[] ToArray() => _headerData;

        /// <summary> (4 bytes)
        /// Block version number.
        /// Version of application it is made with
        /// </summary>
        public byte[] Version => _headerData[..4];
        public ushort GetVersion() => BitConverter.ToUInt16(Version);

        /// <summary> (32 bytes)
        /// 256-bit hash of the previous block header
        /// </summary>
        public byte[] HashPrevBlock => _headerData[4..36];

        /// <summary> (32 bytes)
        /// 256-bit hash based on all of the data in the block
        /// </summary>
        public byte[] HashMerkleRoot => _headerData[36..68];

        /// <summary> (8 bytes)
        /// Time this block was created
        /// </summary>
        public byte[] Time => _headerData[68..76];
        public DateTime GetTime() => new DateTime(BitConverter.ToInt64(Time));
        public void SetTime(DateTime time)
        {
            byte[] data = BitConverter.GetBytes(time.Ticks);
            Buffer.BlockCopy(data, 0, _headerData, 68, 8);
        }

        /// <summary> (4 bytes) 
        /// Current target in compact format
        /// </summary>
        public byte[] Bits => _headerData[76..80];
        public Target GetTarget() => new Target(Bits);

        /// <summary> (4 bytes)
        /// 32-bit number (unsigned)
        /// Used to mine the block
        /// See miner for more detail
        /// </summary>
        public byte[] Nonce => _headerData[80..84];
        public uint GetNonce() => BitConverter.ToUInt32(Nonce);
        private void SetNonce(byte[] data)
            => Buffer.BlockCopy(data, 0, _headerData, 80, 4);
        public void SetNonce(int data) => SetNonce(BitConverter.GetBytes(data));

        /// <summary>
        /// Construct(Deserialize) BlockHeader from a byte[]
        /// </summary>
        /// <param name="headerData">byte[] of a BlockHeader</param>
        public BlockHeader(byte[] headerData)
        {
            if (headerData == null || headerData.Length != Size) throw new InvalidDataException("Invalid data size");
            _headerData = headerData;
        }

        /// <summary>
        /// Create a new blockHeader
        /// </summary>
        /// <param name="hashPrevBlock">Hash from previous block</param>
        /// <param name="blockData">Data of a block, used to calculate hashMerkleRoot</param>
        /// <param name="target">Target of this block header, will be saved as bits</param>
        /// <param name="sha256">sha256 instance to calculate hash of blockData</param>
        /// <returns>New instance of a BlockHeader</returns>
        public static BlockHeader Create(byte[] hashPrevBlock, IBlockData blockData, Target target, SHA256 sha256)
            => new BlockHeader(hashPrevBlock, blockData.GetHashMerkleRoot(sha256), target.ToBits());

        /// <summary>
        /// Create a new BlockHeader
        /// </summary>
        /// <param name="hashPrevBlock">Hash of previous block</param>
        /// <param name="hashMerkleRoot">Hash of data in the block</param>
        /// <param name="bits">Current target in compact format</param>
        public BlockHeader(byte[] hashPrevBlock, byte[] hashMerkleRoot, byte[] bits)
        {
            if (hashPrevBlock == null || hashPrevBlock.Length != 32) throw new InvalidDataException("hashPrevBlock has an invalid length");
            if (hashMerkleRoot == null || hashMerkleRoot.Length != 32) throw new InvalidDataException("hashMerkleRoot has an invalid length");
            if (!Target.IsValidTarget(bits)) throw new InvalidDataException("bits is invalid");
            
            _headerData = new byte[Size];
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            Buffer.BlockCopy(new[] {(byte) v.Major, (byte) v.Minor, (byte) v.Build, (byte) v.Revision},
                0, _headerData, 0, 4); // Write version into headerData
            Buffer.BlockCopy(hashPrevBlock, 0, _headerData, 4, 32); //Write hashPrev block int headerData
            Buffer.BlockCopy(hashMerkleRoot, 0, _headerData, 36, 32);//Write hashMerkleRoot into headerData
            Buffer.BlockCopy(new byte[8],
                0, _headerData, 68, 8);//Write time into headerData
            Buffer.BlockCopy(bits, 0, _headerData, 76, 4);//Write bits into headerData
            Buffer.BlockCopy(new byte[] {0, 0, 0, 0}, 0, _headerData, 80, 4);//Write nonce into headerData
        }
    }
}