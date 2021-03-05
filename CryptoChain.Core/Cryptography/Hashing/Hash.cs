using System.Security.Cryptography;

namespace CryptoChain.Core.Cryptography.Hashing
{
    public class Hash
    {
        public static byte[] SHA_256(byte[] input)
        {
            using var sha = SHA256.Create();
            return sha.ComputeHash(input);
        }

        public static byte[] SHA_512(byte[] input)
        {
            using var sha = SHA512.Create();
            return sha.ComputeHash(input);
        }

        public static byte[] SHA_1(byte[] input)
        {
            using var sha = SHA1.Create();
            return sha.ComputeHash(input);
        }

        public static byte[] RIPEMD_160(byte[] input)
        {
            using var sha = RIPEMD160.Create();
            return sha.ComputeHash(input);
        }

        public static byte[] HASH_256(byte[] input) => SHA_256(SHA_256(input));
        public static byte[] HASH_160(byte[] input) => RIPEMD_160(SHA_256(input));
    }
}