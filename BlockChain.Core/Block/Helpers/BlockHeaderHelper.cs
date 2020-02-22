using System.Linq;
using System.Security.Cryptography;

namespace BlockChain.Core.Block
{
    public static class BlockHeaderHelper
    {
        /// <summary>
        /// Calculate hash of passed block header
        /// </summary>
        /// <param name="blockHeader"></param>
        /// <param name="sha256">sha256 instance to create hash of blockData</param>
        /// <returns>Hash of passed header</returns>
        public static byte[] Hash(this BlockHeader blockHeader, SHA256 sha256)
            => sha256.ComputeHash(blockHeader.ToArray());

        /// <summary>
        /// Create new instance of passed block header (deep clone)
        /// </summary>
        /// <param name="header">header object to clone</param>
        /// <returns>New instance of current object</returns>
        public static BlockHeader Clone(this BlockHeader header)
            => new BlockHeader(header.ToArray().ToArray());
    }
}