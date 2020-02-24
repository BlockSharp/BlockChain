using System.Collections.Generic;
using System.Linq;
using BlockChain.Transactions.Scripting.Enums;

namespace BlockChain.Transactions.Scripting.Scripts
{
    /// <summary>
    /// Basic script object,
    /// Extend for custom scripts
    /// </summary>
    public class Script : Stack<OPCODE>
    {
        public Script()
        {
        }

        public Script(byte[] script) : base(script.Select(x => (OPCODE) x))
        {
        }

        public new byte[] ToArray() => this.Select(x => (byte) x).Reverse().ToArray();
    }
}