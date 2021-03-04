using System.Globalization;
using System.Numerics;

namespace CryptoChain.Core.Cryptography.Algorithms
{
    public static class MathUtils
    {
        public static BigInteger FromHex(string hexString)
        {
            if (((hexString.Length % 2) == 1) || hexString[0] != '0') {
                hexString = "0" + hexString; // if the hex string doesnt start with 0, the parse will assume its negative
            }
            return BigInteger.Parse(hexString, NumberStyles.HexNumber);
        }
    }
}