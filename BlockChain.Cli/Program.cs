using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BlockChain.Core.Block;
using BlockChain.Core;

namespace BlockChain.Cli
{
    class Program
    {
        const int count = 0;
        static void Main()
        {
            Console.WriteLine("Hi!");
            
            Block<TestBlockData> validBlock = new Block<TestBlockData>(Convert.FromBase64String(
                "AQAAAAmKCD7tIp5wWCHSYMEE9XHhXHA4AQsH0gMQiiqQkIjwHuOynzH++oBbPhLPtQl1PNwX9KnuriDiafYwR6WMlRz5B3jtcrfXCB4AAP/yBwEAMTIzNDY1NDIzNTE0M2UAAAA="));
            var x = validBlock.ToArray();
            
            TestBlockChain(validBlock);
        }

        static void TestBlockChain(Block<TestBlockData> validBlock)
        {
            const string file = "blochweakdb2.feeawbasfewa";
            Console.WriteLine($"--------------------- BlockChain2 --------------------------");
            using var sha256 = SHA256.Create();
            var blockChain = new BlockChain<TestBlockData>(file,sha256);

            var sw = Stopwatch.StartNew();
            for(int i = 0; i < count;i++) blockChain.Add(validBlock);
            Console.WriteLine($"Write({count}): {sw.ElapsedMilliseconds}");
            sw.Restart();

            var _ = blockChain.Count();
            Console.WriteLine($"Read({_}): {sw.ElapsedMilliseconds}");
            sw.Restart();

            byte[] timestamp = new byte[] {17, 104, 230, 94, 127, 183, 215, 8};
            var __ = blockChain.Count(x => ArrayEquals(x.GetBlockHeader().Time,timestamp));
            Console.WriteLine($"Search({_}): {sw.ElapsedMilliseconds}");
            sw.Restart();
            
            Console.WriteLine($"FileSize f1: {new FileInfo(file).Length}");
            Console.WriteLine($"FileSize f2: {new FileInfo(file+".h").Length}");
        }
        
        private static bool ArrayEquals(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            int length = b1.Length;
            if (b2.Length != length) return false;
            while (length-- > 0)
                if (b2[length] != b1[length])
                    return false;
            return true;
        }
    }
    
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