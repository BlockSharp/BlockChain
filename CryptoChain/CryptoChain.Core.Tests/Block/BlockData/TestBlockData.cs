using System.Text;
using CryptoChain.Core.Abstractions;

namespace CryptoChain.Core.Tests.Block.BlockData
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