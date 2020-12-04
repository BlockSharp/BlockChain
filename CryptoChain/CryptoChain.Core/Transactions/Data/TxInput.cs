using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Transactions.Data
{
    public class TxInput : ISerializable
    {
        public int Length => Constants.TransactionHashLength + 5 + UnlockingScript.Length;
        
        //The transaction hash/id this input refers to.
        public byte[] TxId { get; set; }
        //The index of the selected output (in the txOut list) of the transaction this input points to
        public byte VOut { get; set; }
        public int ScriptLength { get; private set; }
        public IScript UnlockingScript { get; set; }

        public TxInput(byte[] serialized)
        {
            TxId = Serialization.FromBuffer(serialized, 0, false, Constants.TransactionHashLength);
            int idx = Constants.TransactionHashLength;
            VOut = serialized[idx];
            idx += 1;
            ScriptLength = BitConverter.ToInt32(serialized, idx);
            idx += 4;
            //if doesnt work: provide ScriptLength
            UnlockingScript = new Script(Serialization.FromBuffer(serialized, idx, false));
        }
        
        public byte[] Serialize()
        {
            ScriptLength = UnlockingScript.Length;
            byte[] buffer = new byte[Length];
            int idx = 0;
            Buffer.BlockCopy(TxId, 0, buffer, 0, Constants.TransactionHashLength);
            idx += Constants.TransactionHashLength;
            buffer[idx] = VOut;
            idx += 1;
            Buffer.BlockCopy(BitConverter.GetBytes(ScriptLength), 0, buffer, idx, 4);
            idx += 4;
            Buffer.BlockCopy(UnlockingScript.Serialize(), 0, buffer, idx, ScriptLength);
            return buffer;
        }
    }
}