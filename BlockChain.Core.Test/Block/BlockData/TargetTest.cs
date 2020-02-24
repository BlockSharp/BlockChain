using System;
using System.Collections.Generic;
using BlockChain.Core.Block;
using NUnit.Framework;

namespace BlockChain.Core.Test
{
    public class TargetTest
    {
        [Test]
        public void IsValidTargetTest()
        {
            var invalidTargets = new[] {new byte[]{33,33,33,33}, new byte[5], new byte[0],  new byte[3],new byte[4], new byte[]{ 2, 0 ,0 ,0 }, null };
            var validTargets = new[] {new byte[] {32, 0, 0, 0}, new byte[] {12, 255, 75, 12}, new byte[]{ 3, 0 ,0 ,0 }};
            foreach (var i in invalidTargets)
                Assert.IsFalse(Target.IsValidTarget(i),"Invalid target was marked valid");
            foreach (var i in validTargets)
                Assert.IsTrue(Target.IsValidTarget(i),"Valid target was marked invalid");
        }
        
        [Test]
        public void TestConstructorAndToBits()
        {
            var testData = new Dictionary<byte[], byte[]>();
            byte[] target1 = new byte[Target.ValueSize];
            target1[0] = 1;
            target1[1] = 2;
            target1[2] = 3;
            testData.Add(new byte[]{3, 1, 2, 3},target1);
            
            byte[] target2 = new byte[Target.ValueSize];
            target2[29] = 255;
            target2[30] = 254;
            target2[31] = 253;
            testData.Add(new byte[] {32, 255, 254, 253},target2);
            
            byte[] target3 = new byte[Target.ValueSize];
            target3[0] = 0;
            target3[1] = 0;
            target3[2] = 1;
            testData.Add(new byte[] {3, 0, 0, 1},target3);

            foreach (var data in testData)
            {
                var target = new Target(data.Key);
                Assert.IsTrue(TestHelper.ArrayEquals(target.Value,data.Value),"Target is different then the expected target");
                var bits = target.ToBits();
                Assert.IsTrue(TestHelper.ArrayEquals(bits, data.Key),"Bits are different then the expected bits");
            }
        }

        [Test]
        public void TestIsValid()
        {
            var t = new Target(new byte[]{30,0,0,255});
            byte[] validHash = Convert.FromBase64String("QjKhmpgUjbHYbfaExjeAaiGWmSiFJ/1wGfq9IJLnAAA="),
                invalidHash = Convert.FromBase64String("4kO6jyfbfk4xwxb6pWDt6gGJgSPcRFgF1my82DnkYTw=");

            Assert.IsTrue(t.IsValid(validHash),"Valid hash was not marked valid");
            Assert.IsFalse(t.IsValid(invalidHash),"Invalid hash was not marked invalid");
        }
    }
}