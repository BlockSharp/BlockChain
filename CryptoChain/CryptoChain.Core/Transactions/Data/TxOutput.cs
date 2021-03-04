using System;
using System.Text;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Transactions.Data
{
    public class TxOutput : ISerializable
    {
        public int Length => 8 + 4 + LockingScript.Length;
        
        //Amount of money of the transaction.
        public ulong Amount { get; }
        public int ScriptLength { get; private set; }
        public IScript LockingScript { get; set; }

        /// <summary>
        /// Create new TxOut
        /// </summary>
        /// <param name="amount">The amount of money</param>
        /// <param name="lockScript">The script that locks the output</param>
        public TxOutput(ulong amount, IScript lockScript)
        {
            Amount = amount;
            LockingScript = lockScript;
            ScriptLength = lockScript.Length;
        }
        
        /// <summary>
        /// Deserialize a TxOutput
        /// </summary>
        /// <param name="serialized">The serialized TxOutput</param>
        public TxOutput(byte[] serialized)
        {
            Amount = BitConverter.ToUInt64(serialized);
            ScriptLength = BitConverter.ToInt32(serialized, 8);
            //LockingScript = new Script(Serialization.FromBuffer(serialized, 12, false));
            LockingScript = new Script(serialized[12..]);
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
        
        public bool Equals(TxOutput x)
            => x.Length == Length && x.Amount == Amount && x.LockingScript.Equals(LockingScript);
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("============= Transaction Output =============");
            sb.AppendLine("Amount: " + Amount);
            sb.AppendLine($"LockingScript ({ScriptLength}): " + Convert.ToHexString(LockingScript.Serialize()));
            return sb.ToString();
        }
    }
}