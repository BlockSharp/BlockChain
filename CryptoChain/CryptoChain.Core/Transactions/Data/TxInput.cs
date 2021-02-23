using System;
using System.IO;
using System.Text;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Helpers;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Transactions.Data
{
    /// <summary>
    /// The transaction input. Points at a transaction (with the txId TxId) and marks its output index (VOut)
    /// </summary>
    public class TxInput : ISerializable
    {
        public int Length => Constants.TransactionHashLength + 6 + UnlockingScript.Length;
        
        //The transaction hash/id this input refers to.
        public byte[] TxId { get; set; }
        
        //The index of the selected output (in the txOut list) of the transaction this input points to
        //This is limited to uint to decrease block size
        public uint VOut { get; set; }
        public int ScriptLength { get; private set; }
        public IScript UnlockingScript { get; set; }

        /// <summary>
        /// Create a TxInput from a selected TxOutput
        /// </summary>
        /// <param name="transactionHash">The txID of the selected transaction</param>
        /// <param name="selectedOutput">The index of the output of the selected transaction</param>
        /// <param name="unlockScript">The script that unlocks the output</param>
        public TxInput(byte[] transactionHash, uint selectedOutput, IScript unlockScript)
        {
            if (transactionHash.Length != Constants.TransactionHashLength)
                throw new InvalidDataException("Transaction hash length must equal " + Constants.TransactionHashLength);
            
            TxId = transactionHash;
            VOut = selectedOutput;
            UnlockingScript = unlockScript;
            ScriptLength = unlockScript.Length;
        }
        
        /// <summary>
        /// Deserialize a TxInput
        /// </summary>
        /// <param name="serialized">The serialized TxInput</param>
        public TxInput(byte[] serialized)
        {
            TxId = Serialization.FromBuffer(serialized, 0, false, Constants.TransactionHashLength);
            int idx = Constants.TransactionHashLength;
            VOut = BitConverter.ToUInt16(serialized, idx);
            idx += 2;
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
            Buffer.BlockCopy(BitConverter.GetBytes(VOut), 0, buffer, idx, 2);
            idx += 2;
            Buffer.BlockCopy(BitConverter.GetBytes(ScriptLength), 0, buffer, idx, 4);
            idx += 4;
            Buffer.BlockCopy(UnlockingScript.Serialize(), 0, buffer, idx, ScriptLength);
            return buffer;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var x = (TxInput) obj;
            return x.Length == Length && x.VOut == VOut && x.UnlockingScript.Equals(UnlockingScript);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("============= Transaction Input =============");
            sb.AppendLine("Selected Tx: " + TxId.ToHexString());
            sb.AppendLine("Selected output (VOut): " + VOut);
            sb.AppendLine($"UnlockingScript ({ScriptLength}): " + Convert.ToHexString(UnlockingScript.Serialize()));
            return sb.ToString();
        }
    }
}