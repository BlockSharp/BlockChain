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
            script.Push(OPCODE.OP_1);
            script.Push(OPCODE.OP_2);
            script.Push(OPCODE.OP_3);
            script.Push(OPCODE.OP_4);
            script.Push(OPCODE.OP_5);
            Interpreter.Initialize();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000000; i++) script.Run();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}