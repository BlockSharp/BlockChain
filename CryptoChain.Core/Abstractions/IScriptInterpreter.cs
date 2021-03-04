using CryptoChain.Core.Transactions;
using CryptoChain.Core.Transactions.Scripting;

namespace CryptoChain.Core.Abstractions
{
    public interface IScriptInterpreter
    {
        ExecutionResult Execute(ref Transaction t, params IScript[] scripts);
        ExecutionResult Execute(params IScript[] scripts);
        ExecutionResult Execute(ref Transaction t, out byte[]? output, params IScript[] scripts);
    }
}