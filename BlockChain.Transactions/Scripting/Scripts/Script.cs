using System.Collections.Generic;
using System.Linq;
using BlockChain.Transactions.Scripting.Enums;

namespace BlockChain.Transactions.Scripting.Scripts
{
    /// <summary>
    /// Basic script object,
    /// Extend for custom scripts
    /// </summary>
    public class Script : Queue<byte>
    {
        public Script()
        {
        }

        public void Add(OPCODE code)
            =>Enqueue((byte)code);

        public Script(byte[] script) : base(script.Select(x => x))
        {
        }
        public new byte[] ToArray() => this.Select(x => x).ToArray();
    }
}