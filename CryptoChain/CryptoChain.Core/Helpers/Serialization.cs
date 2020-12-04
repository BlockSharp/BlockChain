using System;
using System.Collections.Generic;
using System.Numerics;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Helpers
{
    public static class Serialization
    {
        /// <summary>
        /// Serializes ISerializable add adds it to a buffer
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="s">The serializable to be serialized</param>
        /// <param name="offset">The offset counter. Important when adding multiple</param>
        /// <param name="withLength">Include Length in serialization</param>
        /// <returns>The offset after the serialization</returns>
        public static int AddSerializable(this byte[] buffer, ISerializable s, int offset = 0, bool withLength = true)
        {
            int length = s.Length;
            if (withLength)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(length), 0, buffer, offset, 4);
                offset += 4;
            }
            
            Buffer.BlockCopy(s.Serialize(), 0, buffer, offset, length);
            offset += length;
            return offset;
        }

        /// <summary>
        /// Add range of serializable items. 
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="items">The items to be serialized</param>
        /// <param name="offset">The starting offset. Will be incremented</param>
        /// <returns>The offset after the serialization</returns>
        public static int AddSerializableRange(this byte[] buffer, ICollection<ISerializable> items, int offset = 0)
        {
            foreach (var s in items)
                offset = buffer.AddSerializable(s, offset);
            return offset;
        }

        /// <summary>
        /// Extracts specific part from byte array like Buffer.BlockCopy does, but with build-in length counter
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="offset">Offset</param>
        /// <param name="withLength">Indicates if length is stored in buffer. If not, count must be provided</param>
        /// <param name="count">The count. Do not provide when withLength is true. If not present it will be calculated from the buffer size and the offset</param>
        /// <returns>Specific byte[] part</returns>
        public static byte[] FromBuffer(byte[] buffer, int offset, bool withLength = true, int count = 0)
            => FromBuffer(buffer, offset, out _, withLength, count);
        
        public static byte[] FromBuffer(byte[] buffer, int offset, out int outOffset, bool withLength = true, int count = 0)
        {
            if (withLength)
            {
                count = BitConverter.ToInt32(buffer, offset);
                offset += 4;
            }
            else
            {
                if (count <= 0)
                    count = buffer.Length - offset;
            }
            
            byte[] result = new byte[count];
            Buffer.BlockCopy(buffer, offset, result, 0, count);
            outOffset = offset;
            return result;
        }

        /// <summary>
        /// Get as much as byte parts as you want using LINQ's .Take()
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="offset">The offset</param>
        /// <returns>LINQ iterator with byte[]</returns>
        public static IEnumerable<byte[]> MultipleFromBuffer(byte[] buffer, int offset = 0)
        {
            while (buffer.Length < offset + 4)
            {
                byte[] result = FromBuffer(buffer, offset, out int outOffset);
                offset += outOffset;
                yield return result;
            }
        }
        
        /// <summary>
        /// Converts BigInteger into inversed byte array
        /// </summary>
        /// <param name="i">A big number</param>
        /// <returns>Reversed byte content</returns>
        public static byte[] ToInvByteArray(this BigInteger i)
        {
            byte[] b = i.ToByteArray();
            Array.Reverse(b);
            return b;
        }
    }
}