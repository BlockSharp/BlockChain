using CryptoChain.Core.Cryptography;
using NUnit.Framework;

namespace CryptoChain.Core.Tests.Cryptography
{
    public class RandomGeneratorTest
    {
        //There is a very small change this test fails
        [Test]
        public void RandomIsReallyRandom()
        {
            RandomGenerator g = new RandomGenerator();
            RandomGenerator g2 = new RandomGenerator();

            byte[] a = g.GetBytes(16);
            byte[] b = g2.GetBytes(16);
            Assert.AreNotEqual(a,b);
        }

        [Test]
        public void RandomCanBeSeeded()
        {
            byte[] seed = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            RandomGenerator g = new RandomGenerator(seed);
            RandomGenerator g2 = new RandomGenerator(seed);
            
            //3 rounds
            Assert.AreEqual(g.GetBytes(16), g2.GetBytes(16));
            Assert.AreEqual(g.GetBytes(32), g2.GetBytes(32));
            Assert.AreEqual(g.GetBytes(8), g2.GetBytes(8));
        }

        [Test]
        public void RandomCanSkipIterations()
        {
            byte[] seed = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            RandomGenerator g = new RandomGenerator(seed);
            RandomGenerator g2 = new RandomGenerator(seed);
            
            g.Skip(10);
            g2.Skip(5);
            
            Assert.AreNotEqual(g.GetBytes(10), g2.GetBytes(10));
            g2.Skip(6);
            Assert.AreEqual(g.GetBytes(10), g2.GetBytes(10));
            Assert.AreEqual(g.Iterations, g2.Iterations);
        }
    }
}