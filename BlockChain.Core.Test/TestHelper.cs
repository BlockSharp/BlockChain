namespace BlockChain.Core.Test
{
    public static class TestHelper
    {
        /// <summary>
        /// Determines if the two passed byte arrays are equal.
        /// </summary>
        /// <returns>true if arrays are equal</returns>
        public static bool ArrayEquals(byte[] b1, byte[] b2)
        {
            if(b1 == null || b2 == null) return false;
            int length = b1.Length;
            if(b2.Length != length) return false;
            while(length-- >0) 
                if(b2[length] != b1[length]) return false;
            return true; 
        }
    }
}