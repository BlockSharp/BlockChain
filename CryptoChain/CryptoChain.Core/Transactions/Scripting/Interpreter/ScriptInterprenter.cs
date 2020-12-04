using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter
{
    public class ScriptInterpreter
    {
        /// <summary>
        /// Dictionary with all the operations
        /// </summary>
        private readonly Dictionary<Opcode, Operation> _operations;
        public ExecutionResult Result { get; private set; }

        /// <summary>
        /// Load all Operations into the Operations dictionary.
        /// </summary>
        public ScriptInterpreter()
        {
            _operations = Assembly.GetExecutingAssembly()
                .GetTypes().Where(x=>(x.Namespace??"").StartsWith("Operations"))
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes().OfType<OpCode>().Any() && m.IsStatic)
                .ToDictionary(k => (
                        (OpCode) k.GetCustomAttributes().First()).Opcode,
                    v => new Operation(v,v.GetCustomAttributes().First() as OpCode));
        }

        public ExecutionResult Execute(ref Transaction t, params Script[] scripts) =>
            Execute(ref t, out _, scripts);
        public ExecutionResult Execute(ref Transaction t, out byte[] output, params Script[] scripts)
        {
            var stack = new ExecutionStack();
            stack.Transaction = t;

            for (int i = 0; i < scripts.Length; i++)
            {
                var s = scripts[i];
                stack.SetScript(ref s); //reference is needed to grab data from script
                
                while (s.Count > 0)
                {
                    try
                    {
                        Opcode op = s.Next();
#if DEBUG
                        Console.WriteLine("Executing: " + op);
#endif
                        var res = _operations[op].Execute(ref stack);
#if DEBUG
                        Console.Write(stack.ToString());
#endif
                        if (res != null)
                        {
                            output = stack.FirstOrDefault();
                            return res.Value;
                        }
                    }
                    catch
                    {
                        output = stack.FirstOrDefault();
                        return ExecutionResult.UNKNOWN_ERROR;
                    }
                }
            }
            
            output = stack.FirstOrDefault();

            int count = stack.Count;
            if (count == 0) return ExecutionResult.INVALID_STACK;
            if (count > 1) return ExecutionResult.MORE_ITEMS_ON_BOTTOM;
            return stack.PeekBool() ? ExecutionResult.SUCCESS : ExecutionResult.FAILURE;
        }
    }
}