using System.Text;
using BlockChain.Core.Block;

namespace BlockChain.Core.Test.Block
{
    public class TestBlockData :IBlockData
    {
        private string _data;

        public TestBlockData() { }
        public TestBlockData(string data)
            =>_data = data;

        public byte[] ToArray()=>Encoding.UTF8.GetBytes(_data);
        public void FromArray(byte[] data) => this._data = Encoding.UTF8.GetString(data);
    }
}