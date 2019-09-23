using System;
using System.Linq;
using EntroBuilder.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests
{
    [TestClass]
    public class BagGeneratorTests
    {
        Random _rnd = new Random(123456);

        [TestMethod]
        public void Next_Collection_ProducesAllValuesOnce()
        {
            var count = 100;
            var collection = Enumerable.Range(0, count).ToList();
            var generator = new BagGenerator<int>(collection);

            var indexes = new int[count];
            for (int i=0; i<count; i++)
            {
                indexes[generator.Next(_rnd)]++;
            }

            for (int i = 0; i < count; i++)
                Assert.AreEqual(1, indexes[i], $"Failed at index {i}");
        }

        [TestMethod]
        public void Next_Collection_ProducesAllValuesOnceThenWraps()
        {
            var count = 100;
            var collection = Enumerable.Range(0, count).ToList();
            var generator = new BagGenerator<int>(collection);

            var iterations = 10;
            var indexes = new int[count];
            for (int i=0; i<count*iterations; i++)
            {
                indexes[generator.Next(_rnd)]++;
            }

            for (int i = 0; i < count; i++)
                Assert.AreEqual(iterations, indexes[i], $"Failed at index {i}");
        }
    }
}
