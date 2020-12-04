using System;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Transactions.Data
{
    public class TxOutput : ISerializable
    {
        public int Length => 8 + 4 + LockingScript.Length;
        
        //Amount of money of the transaction.
        public ulong Amount { get; set; }
        public int ScriptLength { get; private set; }
        public IScript LockingScript { get; set; }

        public TxOutput(byte[] serialized)
        {
            Amount = BitConverter.ToUInt64(serialized);
            ScriptLength = BitConverter.ToInt32(serialized, 8);
            //if doesnt work provide scriptlength
            LockingScript = new Script(Serialization.FromBuffer(serialized, 12, false));
        }
        
        public byte[] Serialize()
        {
            ScriptLength = LockingScript.Length;
            byte[] buffer = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(Amount), 0, buffer, 0, 8);
            int idx = 8;
            Buffer.BlockCopy(BitConverter.GetBytes(ScriptLength), 0, buffer, idx, 4);
            idx += 4;
            Buffer.BlockCopy(LockingScript.Serialize(), 0, buffer, idx, ScriptLength);
            return buffer;
        }
    }
}