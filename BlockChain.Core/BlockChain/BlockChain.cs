using System;
using System.IO;
using System.Linq;
using System.Collections;
using BlockChain.Core.Block;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BlockChain.Core
{
    /// <summary>
    /// BlockChain, saves blocks to disk
    /// Behaves as a stack, last added item is the first returned item
    /// </summary>
    public class BlockChain<T> : IEnumerable<Block<T>> where T : IBlockData, new()
    {
        /// <summary>
        /// Location of the blockchain file
        /// </summary>
        private readonly string _file;
        
        /// <summary>
        /// Location of the header file
        /// </summary>
        private readonly string _headerFile;
        
        /// <summary>
        /// Buffersize (size = integers for blockheader, block for blockchain)
        /// Amount of lengths (BlockHeader) and blocks to load into memory
        /// </summary>
        private readonly int _bufferSize;
        
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
        /// <param name="bufferSize"></param>
        public BlockChain(string file, SHA256 sha256 = null, int bufferSize = 100)
        {
            _file = file;
            _bufferSize = bufferSize;
            _headerFile = $"{file}.h";
            
            _sha256 = sha256 ?? SHA256.Create();
            if (!File.Exists(_file))
            {
                File.WriteAllBytes(_file, Constants.Genesis);
                new BlockChainHeader(_headerFile, _bufferSize).Add(Constants.Genesis.Length);
            }
        }

        /// <summary>
        /// Get total amount of blocks that are in the blockchain
        /// </summary>
        /// <returns></returns>
        public long Size() => new FileInfo(_headerFile).Length / 4;

        /// <summary>
        /// Add new block to the blockChain
        /// </summary>
        /// <param name="block">Block to add to the blockChain</param>
        public void Add(Block<T> block)
        {
            //if (!block.IsValid(this.First().Hash(_sha256), _sha256))
            //        throw new InvalidDataException("Could not add block to blockchain, block is invalid.");
            
            byte[] data = block.ToArray();
            using(var stream = new FileStream(_file, FileMode.Append))
                stream.Write(data);
            new BlockChainHeader(_headerFile, _bufferSize).Add(data.Length);
        }
        
        /// <summary>
        /// Implement IEnumerable methods
        /// ! First returned item is the last added item !
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Block<T>> GetEnumerator()
        {
            using var stream = new FileStream(_file, FileMode.Open);
            long startPos = stream.Length;
            using var header = new BlockChainHeader(_headerFile, _bufferSize).GetEnumerator();

            do
            {
                header.MoveNext();
                int[] sizes = header.Current;
                int bytesToRead = sizes.Sum();
                startPos -= bytesToRead;

                var read = new byte[bytesToRead];
                stream.Position = Math.Max(0,startPos);
                stream.Read(read);

                int position = read.Length, endPosition = position;
                for(var i = 0;i < sizes.Length;i++)
                {
                    position -= sizes[i];
                    yield return new Block<T>(read[position..endPosition]);
                    endPosition = position;
                }
            } while (startPos > 0);
        }
    }
}