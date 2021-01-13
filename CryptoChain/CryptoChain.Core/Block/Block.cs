using System;
using System.IO;
using System.Linq;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Cryptography.Hashing;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions;

namespace CryptoChain.Core.Block
{
    public class Block : ISerializable
    {
        public int Length => Header.Length + Data.Length + 1 + 4;
        
        public BlockDataIdentifier Type { get; set; }
        public BlockHeader Header { get; set; }
        public int DataLength { get; set; }
        public byte[] Data { get; set; }


        private TransactionList? _transactions;
        public TransactionList Transactions
        {
            get
            {
                if (Type != BlockDataIdentifier.TRANSACTIONS)
                    throw new ArgumentException("Data does not contain transactions");
                
                if (_transactions == null)
                    _transactions = new TransactionList(Data);
                return _transactions;
            }
        }

        /// <summary>
        /// Deserialize a block
        /// </summary>
        /// <param name="serialized">The serialized block</param>
        public Block(byte[] serialized)
        {
            Type = (BlockDataIdentifier)serialized[0];
            int idx = 1;
            DataLength = BitConverter.ToInt32(serialized, idx);
            idx += 4;
            Header = new BlockHeader(Serialization.FromBuffer(serialized, idx, false, serialized.Length - DataLength));
            idx += (serialized.Length - DataLength) - idx;
            Data = Serialization.FromBuffer(serialized, idx, false);
        }
        
        /// <summary>
        /// Create a new block
        /// </summary>
        /// <param name="transactions">The transactions to use in the block</param>
        /// <param name="prevBlockHash">The previous block hash</param>
        /// <param name="timestamp">The time the block is created</param>
        /// <param name="target">The target when mining this block</param>
        /// <param name="nonce">The mining nonce</param>
        /// <param name="version">The block version</param>
        public Block(TransactionList transactions, byte[] prevBlockHash, DateTime timestamp, Target target, uint nonce, int version = Constants.BlockVersion)
        {
            Data = transactions.Serialize();
            Type = BlockDataIdentifier.TRANSACTIONS;
            Header = new BlockHeader(prevBlockHash, transactions.MerkleRoot, timestamp, target, nonce, version);
        }

        //Other constructors for people who want to use the blockchain to store something else then transactions
        public Block(ISerializable data, byte[] prevBlockHash, DateTime timestamp, Target target, uint nonce,
            int version = Constants.BlockVersion) : this(data.Serialize(), prevBlockHash, timestamp, target, nonce, version){}
        public Block(byte[] data, byte[] prevBlockHash, DateTime timestamp, Target target, uint nonce,
            int version = Constants.BlockVersion)
        {
            Data = data;
            Type = BlockDataIdentifier.RAW_DATA;
            Header = new BlockHeader(prevBlockHash, Hash.HASH_256(Data), timestamp, target, nonce, version);
        }
        
        /// <summary>
        /// Serialize a block to a byte[]
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] Serialize()
        {
            DataLength = Data.Length;
            byte[] buffer = new byte[Length];
            buffer[0] = (byte)Type;
            int idx = 1;
            Buffer.BlockCopy(BitConverter.GetBytes(Data.Length), 0, buffer, idx, 4);
            idx += 4;
            buffer.AddSerializable(Header, idx, false);
            idx += Header.Length;
            Buffer.BlockCopy(Data, 0, buffer, idx, DataLength);
            return buffer;
        }
    }
}