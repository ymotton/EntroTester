using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests
{
    [TestClass]
    public class SequenceGeneratorTests
    {
        Random _rnd = new Random(123456);

        [TestMethod]
        public void Next_Collection_ProducesUniformlyDistributedAllPossibleValues()
        {
            var collection = Enumerable.Range(0, 10).ToList();
            var generator = new SequenceGenerator<int>(collection);

            int length = 1000000;
            int[] frequency = new int[10];
            for (int i=0; i<length; i++)
            {
                var index = generator.Next(_rnd);
                frequency[index]++;
            }

            // If uniformly random this should be true
            for (int i = 0; i < 10; i++)
                Assert.IsTrue(frequency[i] > (length * .95 / 10));
        }

        [TestMethod]
        public void Next_FiniteSequence_ProducesUniformlyDistributedAllPossibleValues()
        {
            int upperBound = 20;
            var finiteSequence = Enumerable.Range(0, upperBound);
            var generator = new SequenceGenerator<int>(finiteSequence);

            int length = 1000000;
            int[] frequency = new int[upperBound];
            for (int i = 0; i < length; i++)
            {
                var index = generator.Next(_rnd);
                frequency[index]++;
            }

            // If uniformly random this should be true
            for (int i = 0; i < upperBound; i++)
                Assert.IsTrue(frequency[i] > (length * .95 / upperBound));
        }

        static IEnumerable<int> InfiniteSequence()
        {
            int i = 0;
            while(true)
            {
                yield return i;
                i++;
            }
        }
        [TestMethod]
        public void Next_InfiniteSequence_ProducesDifferentValues()
        {
            var infiniteSequence = InfiniteSequence();
            var generator = new SequenceGenerator<int>(infiniteSequence, 100);

            var results = new List<int>();
            int length = 1000000;
            for (int i = 0; i < length; i++)
            {
                var index = generator.Next(_rnd);
                results.Add(index);
            }
            
            foreach(var i in results)
                Assert.IsTrue(i < 100);
        }
    }
}
