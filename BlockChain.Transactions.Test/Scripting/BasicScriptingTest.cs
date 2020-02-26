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
            CustomScript custom = new CustomScript();
            custom.AddInstructions(OPCODE.OP_1, OPCODE.OP_2, OPCODE.ADD, OPCODE.OP_3, OPCODE.EQ_NUM);
            Interpreter.Initialize();

            Assert.AreEqual(EXECUTION_RESULT.SUCCESS, custom.Run());
        }
    }
}