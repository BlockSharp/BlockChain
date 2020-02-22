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
        private readonly string _file;
        private readonly SHA256 _sha256;

        public BlockChain(string file, SHA256 sha256 = null)
        {
            _file = file;
            _sha256 = sha256 ?? SHA256.Create();

            if (!File.Exists(_file)) File.WriteAllBytes(_file, Constants.Genesis);
        }

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

        public void Add(Block<T> block)
        {
            if (block.IsValid(this.First().ToArray(), _sha256))
                throw new InvalidDataException("Could not add block to blockchain, block is invalid.");

            byte[] data = block.ToArray();
            using var stream = new FileStream(_file, FileMode.Append);
            stream.Write(data, 0, data.Length);
        }
        
        /* ToDo: Add test for this method.
        public async void Add(T data, Target target = null, CancellationToken token = new CancellationToken())
        {
            using var sha256 = SHA256.Create();
            var prevBlock = this.First();
            target ??= prevBlock.GetBlockHeader().GetTarget();
            var block = Block<T>.Create(prevBlock.Hash(sha256),data,target, sha256);
            
            await block.Mine(token);
            Add(block);
        }*/
    }
}