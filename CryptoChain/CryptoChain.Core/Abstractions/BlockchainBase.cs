using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Chain;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Scripting;
using CryptoChain.Core.Transactions.Scripting.Interpreter;

namespace CryptoChain.Core.Abstractions
{
    public abstract class BlockchainBase
    {
        public IBlockStore Blocks { get; }
        public ITransactionStore Transactions { get; }
        protected readonly ScriptInterpreter Interpreter;
        
        public BlockchainBase(IBlockStore store, ITransactionStore transactions)
        {
            Blocks = store;
            Transactions = transactions;
            Interpreter = new ScriptInterpreter();
        }

        /// <summary>
        /// Validate all block headers (check if the prevHash equals the hash of the previous block)
        /// </summary>
        /// <returns>True if blockChain is valid</returns>
        public bool ValidateHeaders()
        {
            var headers = Blocks.GetHeaders().ToList();
            for (int i = 1; i < headers.Count; i++)
                if (!headers[i].PrevHash.SequenceEqual(headers[i - 1].Hash))
                    return false;
            return true;
        }

        /// <summary>
        /// Check if transaction is valid
        /// </summary>
        /// <param name="transaction">The transaction to be validated</param>
        /// <returns>ValidationResult</returns>
        public async Task<ValidationResult> Validate(Transaction transaction)
        {
            //Get desired information
            transaction.BlockHeight = Transactions.GetBlockHeight(transaction.TxId);
            var blockHeight = Blocks.BlockHeight;
            
            // Check if transaction is coinbase
            if (transaction.IsCoinbase)
                return ValidationResult.TX_IS_COINBASE;

            if (!transaction.Inputs.Any())
                return ValidationResult.TX_NO_INPUTS;
            
            if (!transaction.Outputs.Any())
                return ValidationResult.TX_NO_OUTPUTS;

            if (transaction.TxInCount != transaction.Inputs.Count ||
                transaction.TxOutCount != transaction.Outputs.Count)
                return ValidationResult.TX_WRONG_DATA;
            
            // Validate transaction inputs and outputs
            ulong inSum = 0;
            ulong outSum = 0;
                
            foreach (var output in transaction.Outputs)
                outSum += output.Amount;

            foreach (var input in transaction.Inputs)
            {
                // Check reference transaction
                var inputTx = await Transactions.GetTransaction(input.TxId);
                if (inputTx == null)
                    return ValidationResult.TX_WRONG_REFERENCE;
                
                var selOutput = inputTx.Outputs[input.VOut];
                inSum += selOutput.Amount;

                // Verify lockTime of referenced transaction
                if (!inputTx.VerifyLockTime(blockHeight))
                    return ValidationResult.TX_LOCK_TIME_ERROR;
                    
                // Check if input's selected output isn't spent on another place (if UTXO)
                //... HEAVY (if not indexed!)
                    
                // Check if inputs can be unlocked with the unlockingScript
                if (Interpreter.Execute(ref inputTx, input.UnlockingScript, selOutput.LockingScript) !=
                    ExecutionResult.SUCCESS)
                    return ValidationResult.TX_SCRIPT_FAILURE;
            }

            // Check if balance is correct (dont forget to check for coinbase!)
            if (outSum - inSum != 0)
                return ValidationResult.TX_UNBALANCED;

            return ValidationResult.SUCCESS;
        }
    }
}