using System;
using System.Collections.Generic;
using System.Text;

namespace BlockChain.Core.Cryptography
{
    public static class Hash
    {
        public static byte[] SHA256(byte[] input)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
                return sha.ComputeHash(input);
        }

        public static byte[] SHA1(byte[] input)
        {
            using (var sha = System.Security.Cryptography.SHA1.Create())
                return sha.ComputeHash(input);
        }

        public static byte[] RIPEMD160(byte[] input)
        {
            using (var sha = Cryptography.RIPEMD160.Create())
                return sha.ComputeHash(input);
        }

        public static byte[] HASH256(byte[] input) => SHA256(SHA256(input));
        public static byte[] HASH160(byte[] input) => RIPEMD160(SHA256(input));
    }
}
