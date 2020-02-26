using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BlockChain.Transactions.Scripting.Enums;
using BlockChain.Transactions.Scripting.Scripts;

namespace BlockChain.Transactions.Scripting
{
    public static class Interpreter
    {
        /// <summary>
        /// Dictionary with all the operations
        /// </summary>
        private static readonly Dictionary<OPCODE, Operation> Operations;

        /// <summary>
        /// Load all Operations into the Operations dictionary.
        /// </summary>
        public static void Initialize() { } //Useful empty function
        static Interpreter()
        {
            Operations = Assembly.GetExecutingAssembly()
                .GetTypes().Where(x=>(x.Namespace??"").StartsWith("Operations"))
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes().OfType<OpCode>().Any() && m.IsStatic)
                .ToDictionary(k => (
                        (OpCode) k.GetCustomAttributes().First()).OPCODE,
                    v => new Operation(v,v.GetCustomAttributes().First() as OpCode));
        }

        /// <summary>
        /// Run a script and return the result.
        /// </summary>
        /// <param name="script">Script to run</param>
        /// <param name="transaction">The transaction the script refers to</param>
        /// <returns>Result of script</returns>
        public static EXECUTION_RESULT Run(this Script script, Transaction transaction = null)
            => RunAndGetStack(script, out var output, transaction);

        /// <summary>
        /// Run a script and return the result.
        /// </summary>
        /// <param name="script">Script to run</param>
        /// <param name="transaction">The transaction the script refers to</param>
        /// <returns>Result of script</returns>
        public static EXECUTION_RESULT RunAndGetStack(this Script script, out byte[] output, Transaction transaction = null)
        {
            var executionStack = new ExecutionStack(ref script, ref transaction);
            try
            {
                while (script.Any())
                {
                    OPCODE current = (OPCODE)script.Dequeue();
#if DEBUG
                    System.Console.WriteLine("Executing: " + current.ToString());
#endif
                    var result = Operations[current].Execute(ref executionStack);

#if DEBUG
                    executionStack.PrintStack();
#endif

                    if (result != null) 
                    {
                        output = executionStack.FirstOrDefault();
                        return (EXECUTION_RESULT)result;
                    }
                }
            }
            catch(System.Exception e)
            {
#if DEBUG
                throw e;
#endif
                return EXECUTION_RESULT.UNKNOWN_ERROR; 
            }

            output = executionStack.FirstOrDefault();

            int count = executionStack.Count;
            if (count == 0) return EXECUTION_RESULT.INVALID_STACK;
            else if (count > 1) return EXECUTION_RESULT.MORE_ITEMS_ON_BOTTOM;
            else return executionStack.PeekByte() == 1 ? EXECUTION_RESULT.SUCCESS : EXECUTION_RESULT.FAILURE;
        }
    }
}