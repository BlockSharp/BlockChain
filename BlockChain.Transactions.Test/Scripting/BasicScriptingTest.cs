using System;
using System.Diagnostics;
using BlockChain.Transactions.Scripting;
using BlockChain.Transactions.Scripting.Enums;
using BlockChain.Transactions.Scripting.Scripts;
using NUnit.Framework;

namespace BlockChain.Transactions.Test.Scripting
{
    public class BasicScriptingTest
    {
        [Test]
        public void ScriptingTest1()
        {
            var script = new Script();
            script.Add(OPCODE.OP_1);
            script.Add(OPCODE.OP_1);
            script.Add(OPCODE.OP_1);
            script.Add(OPCODE.OP_1);
            Interpreter.Initialize();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++) script.Run();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}