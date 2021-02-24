using System;
using System.IO;
using System.Linq;

namespace CryptoChain.Core.Blocks
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
            => bits.Length == 4 && bits[0] >= 3 && bits[0] <= ValueSize;

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
        /// Create a new target. How lower, how more difficult. For readability, please enter your numbers in hex.
        /// The exponent is the 'index' of the target including the coefficients. 
        /// Example: (32, 0xFF, 0x00, 0x00) equals (31, 0x00, 0xFF, 0x00) and (30, 0x00, 0x00, 0xFF)
        /// The target will look like: 0000000000000000000000000000000000000000000000000000000000FF0000
        /// 29 zero pairs (00), 2 from the right
        /// The most difficult (read: nearly impossible) target will be: [3, 0x01, 0x00, 0x00]
        /// The easiest target will be: [32, any, any, 0xFF]
        /// </summary>
        /// <param name="exponent">The exponent (size of the target in bytes, amount of zeroes - 3)</param>
        /// <param name="c1">First byte of target</param>
        /// <param name="c2">Second byte of target</param>
        /// <param name="c3">Third byte of target</param>
        public Target(byte exponent = ValueSize, byte c1 = 0xFF, byte c2 = 0xFF, byte c3 = 0xFF) : this(new byte[]{exponent, c1,c2,c3}){}

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

        public override string ToString()
            => new(Convert.ToHexString(Value).Reverse().ToArray());
    }
}