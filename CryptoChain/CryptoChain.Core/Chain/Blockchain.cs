using System;
using System.Linq;
using System.Threading.Tasks;
using CryptoChain.Core.Abstractions;
using CryptoChain.Core.Blocks;
using CryptoChain.Core.Chain.Storage;
using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Scripting;
using CryptoChain.Core.Transactions.Scripting.Interpreter;

namespace CryptoChain.Core.Chain
{
    public class Blockchain
    {
        private readonly IBlockStore _blocks;
        private readonly ITransactionStore _transactions;
        private readonly ScriptInterpreter _interpreter;

        public Blockchain(IBlockStore store)
        {
            _blocks = store;
            _transactions = new TransactionStore(ref store);
            _interpreter = new ScriptInterpreter();
            Update();
        }

        public void Update()
        {
            _interpreter.CurrentBlockHeight = _blocks.BlockHeight;
        }

        public bool ValidateHeaders()
        {
            var headers = _blocks.GetHeaders().ToList();
            for (int i = 1; i < headers.Count; i++)
                if (!headers[i].PrevHash.SequenceEqual(headers[i - 1].Hash))
                    return false;
            return true;
        }

        public Task<Transaction> GetTransaction(byte[] txId)
            => _transactions.GetTransaction(txId);
        
        //Replace bool with enum?
        public async Task<ValidationResult> Validate(Transaction transaction)
        {
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
                var inputTx = await GetTransaction(input.TxId);
                var selOutput = inputTx.Outputs[input.VOut];
                inSum += selOutput.Amount;
                
                // Verify lockTime of referenced transaction
                if (!inputTx.VerifyLockTime(_blocks.BlockHeight))
                    return ValidationResult.TX_LOCK_TIME_ERROR;
                    
                // Check if input's selected output isn't spent on another place (if UTXO)
                //... HEAVY (if not indexed!)
                    
                // Check if inputs can be unlocked with the unlockingScript
                if (_interpreter.Execute(ref inputTx, input.UnlockingScript, selOutput.LockingScript) !=
                    ExecutionResult.SUCCESS)
                    return ValidationResult.TX_SCRIPT_FAILURE;
            }

            // Check if balance is correct (dont forget to check for coinbase!)
            if (outSum - inSum != 0)
                return ValidationResult.TX_UNBALANCED;

            return ValidationResult.SUCCESS;
        }

        public Task<Block> GetBlock(byte[] blockId)
        {
            throw new System.NotImplementedException();
        }
    }
}