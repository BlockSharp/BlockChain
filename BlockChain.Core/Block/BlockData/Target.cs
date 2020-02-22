/* https://learnmeabitcoin.com/guide/bits
 * Format: [length of target] [last byte 3] [last byte 2] [last byte of target]
 */

using System;
using System.IO;

namespace BlockChain.Core.Block
{
    public class Target
    {
        public const int ValueSize = 32;

        /// <summary>
        /// The Target (32 bytes)
        /// Hash of a block must be below target before the block is valid.
        /// </summary>
        public readonly byte[] Value;

        /// <summary>
        /// Determines whether the passed byte array contains a valid target.
        /// </summary>
        /// <param name="bits">Target in compact format</param>
        /// <returns>true = valid bits</returns>
        public static bool IsValidTarget(byte[] bits)
            => bits != null && bits.Length == 4 && bits[0] >= 3 && bits[0] <= ValueSize;

        /// <summary>
        /// Construct the target from bits,
        /// Format bits: [length of target] [last byte 3] [last byte 2] [last byte of target]
        /// </summary>
        public Target(byte[] bits)
        {
            if (!IsValidTarget(bits)) throw new InvalidDataException("Invalid bits");
            Value = new byte[ValueSize];
            Buffer.BlockCopy(bits, 1, Value, bits[0] - 3, 3);
        }

        /// <summary>
        /// Convert the Target to bits
        /// Format bits: [length of target] [last byte 3] [last byte 2] [last byte of target]
        /// </summary>
        /// <returns>Target in compact format</returns>
        public byte[] ToBits()
        {
            byte[] bits = new byte[4];
            bits[0] = (byte) GetBitsLength();
            Buffer.BlockCopy(Value, bits[0] - 3, bits, 1, 3);
            return bits;
        }

        /// <summary>
        /// Determines if the passed hash is valid for this target.
        /// Hash is valid if smaller than target.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns>true = valid</returns>
        public bool IsValid(byte[] hash)
        {
            for (int i = Value.Length - 1; i > 0; i--)
            {
                if (hash[i] > Value[i]) return false;
                if (hash[i] < Value[i]) return true;
            }

            return true;
        }

        /// <summary>
        /// Get index of first byte in target with a value.
        /// </summary>
        /// <returns>index of first byte with value</returns>
        private int GetBitsLength()
        {
            int index = ValueSize - 4;
            for (int i = Value.Length - 1; i > 0; i--)
                if (Value[i] != 0)
                    return Math.Max(i + 1, 3);
            return index;
        }
    }
}