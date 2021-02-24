using System.Collections;
using System.Collections.Generic;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Abstractions
{
    public interface IScript : ISerializable
    {
        /// <summary>
        /// Get next instruction from script
        /// </summary>
        /// <returns>OPCODE</returns>
        Opcode Next();
        
        /// <summary>
        /// Get next byte from script
        /// </summary>
        /// <returns>byte</returns>
        byte NextByte();
        
        /// <summary>
        /// Get range of bytes from script
        /// </summary>
        /// <returns>IEnumerable</returns>
        IEnumerable<byte> NextRange();
        
        /// <summary>
        /// Get range of bytes from script
        /// </summary>
        /// <param name="count">Amount of bytes to dequeue</param>
        /// <returns>byte[]</returns>
        byte[] NextRange(int count);
        
        /// <summary>
        /// Add a byte/instruction to the script
        /// </summary>
        /// <param name="b">The byte to be added</param>
        void Add(byte b);
        
        /// <summary>
        /// Add OPCODE(s) to the script
        /// </summary>
        /// <param name="code">The instruction(s)</param>
        void Add(params Opcode[] code);
        
        /// <summary>
        /// Add an other script to this script
        /// </summary>
        /// <param name="s">The script to be added</param>
        void Add(IScript s);
        
        /// <summary>
        /// Add byteRange/data to the script
        /// </summary>
        /// <param name="bytes">byte[]</param>
        void AddRange(byte[] bytes);
        
        /// <summary>
        /// Clone the script
        /// </summary>
        /// <returns></returns>
        IScript Clone();

        /// <summary>
        /// Get the hash from the script
        /// </summary>
        /// <returns>The hash</returns>
        byte[] Hash();
    }
}