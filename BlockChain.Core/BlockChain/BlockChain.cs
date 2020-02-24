using System;
using System.IO;
using System.Linq;
using System.Collections;
using BlockChain.Core.Block;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;

namespace BlockChain.Core
{
    public class BlockChain<T> : IEnumerable<Block<T>> where T : IBlockData, new()
    {
        /// <summary>
        /// Path to blockchain file
        /// </summary>
        private readonly string _file;
        
        /// <summary>
        /// Hashing algorithm instance used by blockchain and the helper methods of blockchain
        /// </summary>
        private readonly SHA256 _sha256;
        public SHA256 GetHashingAlgorithm() => _sha256;

        /// <summary>
        /// Create new instance of blockchain,
        /// if file does not exists file will be created
        /// </summary>
        /// <param name="file">path to blockchain file</param>
        /// <param name="sha256">sha256 instance to use</param>
        public BlockChain(string file, SHA256 sha256 = null)
        {
            _file = file;
            _sha256 = sha256 ?? SHA256.Create();

            if (!File.Exists(_file)) File.WriteAllBytes(_file, Constants.Genesis);
        }

        /// <summary>
        /// Implement IEnumerable methods
        /// ! First item is the last added item !
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Block<T>> GetEnumerator()
        {
            using var stream = new FileStream(_file, FileMode.Open);
            stream.Seek(0, SeekOrigin.End);                                       //Set position to end of file

            int blockSize = 0;
            do
            {
                byte[] size = new byte[4];
                stream.Seek(-(blockSize + 4), SeekOrigin.Current);                //Set position to start of block length
                stream.Read(size, 0, 4);                                  //Read block length
                blockSize = BitConverter.ToInt32(size);                                    //Convert block length to int
                
                stream.Seek(-blockSize, SeekOrigin.Current);                               //Set position to start of block
                byte[] blockData = new byte[blockSize];
                stream.Read(blockData, 0, blockSize);                     //Read block
                yield return new Block<T>(blockData);
            } while (stream.Position != blockSize);
        }

        /// <summary>
        /// Add new block to the blockchain
        /// </summary>
        /// <param name="block">Block to add to the blockchain</param>
        public void Add(Block<T> block)
        {
            if (!block.IsValid(this.First().Hash(_sha256), _sha256))
                throw new InvalidDataException("Could not add block to blockchain, block is invalid.");

            byte[] data = block.ToArray();
            using var stream = new FileStream(_file, FileMode.Append);
            stream.Write(data, 0, data.Length);
        }
    }
}