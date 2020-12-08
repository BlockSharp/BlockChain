using System.Security.Cryptography;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Block.Helpers
{
    public static class BlockHelper
    {
        /// <summary>
        /// Determines if a block is valid.
        /// </summary>
        /// <param name="block">Block to check</param>
        /// <param name="hashPrevBlock">Hash of previous block</param>
        /// <param name="sha256">sha256 instance to calculate hashMerkleRoot</param>
        /// <returns>True if block is valid</returns>
        public static bool IsValid<T>(this Block<T> block, byte[] hashPrevBlock, SHA256 sha256)
            where T : IBlockData, new()
        {
            var header = block.GetBlockHeader();
            var data = block.GetData();
            return (ArrayEquals(block.ToArray(), Constants.Genesis) || //Return true if block is genesis
                    ArrayEquals(header.HashPrevBlock, hashPrevBlock)) && //Check hash prev block
                   ArrayEquals(header.HashMerkleRoot, data.GetHashMerkleRoot(sha256)) && //Check hash data
                   data.IsValid() && //Check if data is valid (only used if overwritten)
                   block.GetSize() == block.ToArray().Length && // Check if block length is valid
                   header.GetTarget().IsValid(block.Hash(sha256)); //Check target
        }

        /// <summary>
        /// Create a hash of the block header.
        /// </summary>
        /// <param name="block">Block to check</param>
        /// <param name="sha256">sha256 instance to calculate hash of the blockheader</param>
        /// <returns>sha256 hash of the blockHeader</returns>
        public static byte[] Hash<T>(this Block<T> block, SHA256 sha256) where T : IBlockData, new()
            => sha256.ComputeHash(block.BlockHeader);

        /// <summary>
        /// Determines if the two passed byte arrays are equal.
        /// </summary>
        /// <returns>true if arrays are equal</returns>
        private static bool ArrayEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            int length = b1.Length;
            if (b2.Length != length) return false;
            while (length-- > 0)
                if (b2[length] != b1[length])
                    return false;
            return true;
        }
    }
}