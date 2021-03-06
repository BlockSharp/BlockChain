namespace BlockChain.Core
{
    public static class Constants
    {
        /// <summary>
        /// First block of the blockchain
        /// </summary>
        public static readonly byte[] Genesis =
        {
            1, 0, 0, 0, //Version
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, //Hash prev block
            227, 176, 196, 66, 152, 252, 28, 20, 154, 251, 244, 200, 153, 111, 185, 36, 39, 174, 65, 228, 100, 155, 147,
            76, 164, 149, 153, 27, 120, 82, 184, 85, //Hash data
            17, 104, 230, 94, 127, 183, 215, 8, //Timestamp
            31, 255, 0, 0, //Bits
            128, 168, 21, 0, //Nonce
            88, 0, 0, 0 //Length of block
        };
    }
}
