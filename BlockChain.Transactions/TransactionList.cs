using System.Collections.Generic;
using BlockChain.Core.Block;

namespace BlockChain.Transactions
{
    public class TransactionList : List<Transaction>, IBlockData
    {
        public TransactionList(byte[] data) => FromArray(data);
        
        public new byte[] ToArray()
        {
            throw new System.NotImplementedException();
        }

        public void FromArray(byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public bool IsValid() => true;
    }
}