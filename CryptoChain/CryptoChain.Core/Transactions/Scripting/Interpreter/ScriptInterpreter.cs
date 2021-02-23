using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Transactions.Scripting.Interpreter
{
    public class ScriptInterpreter : IScriptInterpreter
    {
        /// <summary>
        /// Dictionary with all the operations
        /// </summary>
        private readonly Dictionary<Opcode, Operation> _operations;

        /// <summary>
        /// Load all Operations into the Operations dictionary.
        /// </summary>
        public ScriptInterpreter()
        {
            _operations = Assembly.GetExecutingAssembly()
                .GetTypes().Where(x=>(x.Namespace??"").Contains("Interpreter.Operations"))
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes().OfType<OpCode>().Any() && m.IsStatic)
                .ToDictionary(k => (
                        (OpCode) k.GetCustomAttributes().First()).Opcode,
                    v => new Operation(v, (OpCode)v.GetCustomAttributes().First()));
        }

        public ExecutionResult Execute(ref Transaction t, params IScript[] scripts) =>
            Execute(ref t, out _, scripts);
        public ExecutionResult Execute(ref Transaction t, out byte[]? output, params IScript[] scripts)
        {
            var stack = new ExecutionStack { Transaction = t };

            for (int i = 0; i < scripts.Length; i++)
            {
                var s = scripts[i];
                stack.SetScript(ref s); //reference is needed to grab data from script
                
                while (s.Length > 0)
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
                    catch(Exception e)
                    {
                        output = stack.FirstOrDefault();
#if DEBUG
                        throw e;
#endif
                        if (e is KeyNotFoundException) return ExecutionResult.UNKNOWN_CODE;
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