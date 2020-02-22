using System.Text;
using BlockChain.Core.Block;

namespace BlockChain.Cli
{
    public class ExampleBlockData : IBlockData
    {
        private string testData;
        public byte[] TestData => Encoding.UTF8.GetBytes(testData);

        public ExampleBlockData()
        {
        }

        public ExampleBlockData(string data)
        {
            testData = data;
        }

        public override string ToString() => testData;
        public byte[] ToArray() => Encoding.UTF8.GetBytes(testData);
        public void FromArray(byte[] data) => testData = Encoding.UTF8.GetString(data);
        public int Size() => Encoding.UTF8.GetByteCount(testData);
    }
}