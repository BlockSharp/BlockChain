using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BlockChain.Core
{
    /// <summary>
    /// BlockChainHeader, saves length of blocks to disk
    /// Behaves as a stack, last added item is the first returned item
    /// </summary>
    public class BlockChainHeader : IEnumerable<int[]>
    {
        /// <summary>
        /// bufferSize (size = integers)
        /// Amount of ingerers read at once
        /// </summary>
        private readonly int _bufferSize;
        /// <summary>
        /// Location of the header file
        /// </summary>
        private readonly string _file;

        /// <summary>
        /// Create new instance of blockHeader
        /// </summary>
        /// <param name="file">Location of the header file</param>
        /// <param name="bufferSize">BufferSize  (size = integers)integers</param>
        public BlockChainHeader(string file, int bufferSize)
        {
            _file = file;
            _bufferSize = bufferSize;
        }

        /// <summary>
        /// Add new integer to header file
        /// </summary>
        /// <param name="length">Length of data that is added to the blockchain</param>
        public void Add(int length)
        {
            using var stream = new FileStream(_file,FileMode.Append);
            stream.Write(BitConverter.GetBytes(length));
        }

        /// <summary>
        /// Return int[buffersize] every iteration
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<int[]> GetEnumerator()
        {
            using var stream = new FileStream(_file, FileMode.Open);
            long startPos = stream.Length;
            
            do
            {
                long endPos = startPos;
                startPos -= _bufferSize * 4;
                long bytesToRead = startPos < 0 ? endPos : _bufferSize * 4;

                var read = new byte[bytesToRead];
                stream.Position = Math.Max(0,startPos);
                stream.Read(read);

                int[] buffer = new int[bytesToRead / 4];
                for (int i = read.Length - 4, x = 0; i >= 0; i-=4, x++) buffer[x] = BitConverter.ToInt32(read,i);
                yield return buffer;
            } while (startPos > 0);
        }
    }
}