using System;
using System.IO;
using System.Linq;
using System.Text;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions;

namespace CryptoChain.Core.Block
{
    /// <summary>
    /// The Block. Note that a block won't be generated using a constructor but can just be created
    /// using a Miner
    /// </summary>
    public class Block : ISerializable
    {
        public int Length => Header.Length + Data.Length + 1 + 4;
        
        public BlockDataIdentifier Type { get; }
        public BlockHeader Header { get; }
        public int DataLength { get; }
        public byte[] Data { get; }
        public byte[] Hash => Header.Hash;

        private TransactionList? _transactions;
        
        /// <summary>
        /// Get list of transactions from block
        /// </summary>
        /// <exception cref="ArgumentException">Throws if the block does not contain transaction data</exception>
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
        /// Create a new instance of a Block. This function is intended to be used by the Miner
        /// </summary>
        /// <param name="data">The block data</param>
        /// <param name="header">The block header</param>
        /// <param name="type">The block data type</param>
        public Block(byte[] data, BlockHeader header, BlockDataIdentifier type)
        {
            Data = data;
            DataLength = data.Length;
            Header = header;
            Type = type;
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
        /// Serialize a block to a byte[]
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] Serialize()
        {
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("============================= Block ============================");
            sb.AppendLine("Type: " + Type);
            sb.AppendLine("DataLength: " + DataLength);
            sb.AppendLine("Hash/block ID: " + Hash.ToHexString());
            sb.AppendLine(Header.ToString());
            if (Type == BlockDataIdentifier.TRANSACTIONS)
            {
                sb.AppendLine("Data (TransactionList): ");
                sb.AppendLine(Transactions.ToString());
            }
            else
                sb.AppendLine("Data (RAW): " + Convert.ToHexString(Data));
            return sb.ToString();
        }
    }
}