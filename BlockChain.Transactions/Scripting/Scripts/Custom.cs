using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockChain.Transactions.Scripting.Enums;

namespace BlockChain.Transactions.Scripting.Scripts
{
    public class CustomScript : Script
    {
        public CustomScript() : base() { }
        public CustomScript(byte[] serialized) : base(serialized) { }
        public CustomScript(List<byte> instructions) : base() { this.AddInstructions(instructions); }
        public CustomScript(params OPCODE[] instructions) : base() { this.AddInstructions(instructions); }

        /// <summary>
        /// Clear all instructions. Take care: All instructions will be inverted!
        /// </summary>
        public void ClearInstructions() => this.Clear();

        public void AddInstruction(OPCODE opcode) => this.Add(opcode);
        public void AddInstruction(byte instruction) => this.Enqueue(instruction);

        /// <summary>
        /// WARNING: reverses instructions.
        /// </summary>
        /// <param name="instructions">The instructions</param>
        public void AddInstructions(List<byte> instructions)
            => this.AddRange(instructions.ToArray().Reverse().ToArray());

        /// <summary>
        /// WARNING: reverses instructions.
        /// </summary>
        /// <param name="opcodes">The opcodes or bytes you want to insert</param>
        public void AddInstructions(params byte[] opcodes)
            => this.AddRange(opcodes.Reverse().ToArray());

        /// <summary>
        /// WARNING: reverses instructions.
        /// </summary>
        /// <param name="opcodes">The OPCODES you want to add</param>
        public void AddInstructions(params OPCODE[] opcodes)
            => this.AddRange(opcodes.Reverse().Select(f => (byte)f).ToArray());

        /// <summary>
        /// Add data to instruction
        /// </summary>
        /// <param name="data"></param>
        public void AddData(byte[] data)
        {
            int size = data.Length;

            if (size <= byte.MaxValue)
            {
                this.Add(OPCODE.PUSHDATA_1);
                this.Enqueue((byte)size);
            }
            else if (size <= short.MaxValue)
            {
                this.Add(OPCODE.PUSHDATA_2);
                this.AddRange(BitConverter.GetBytes((short)size));
            }
            else
            {
                this.Add(OPCODE.PUSHDATA_4);
                this.AddRange(BitConverter.GetBytes(size));
            }

            this.AddRange(data);

        }

        public void AddData(string data) => AddData(Encoding.UTF8.GetBytes(data));

        public void AddData(int data)
        {
            this.Add(OPCODE.PUSH_INT);
            this.AddRange(BitConverter.GetBytes(data));
        }

        public void AddData(uint data)
        {
            this.Add(OPCODE.PUSH_UINT);
            this.AddRange(BitConverter.GetBytes(data));
        }

        public void AddData(short data)
        {
            this.Add(OPCODE.PUSH_SHORT);
            this.AddRange(BitConverter.GetBytes(data));
        }
    }
}