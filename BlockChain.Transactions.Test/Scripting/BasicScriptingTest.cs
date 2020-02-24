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
            script.Enqueue(OPCODE.OP_1);
            script.Enqueue(OPCODE.OP_2);
            script.Enqueue(OPCODE.OP_3);
            script.Enqueue(OPCODE.OP_4);
            script.Enqueue(OPCODE.OP_5);
            Interpreter.Initialize();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++) script.Run();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}