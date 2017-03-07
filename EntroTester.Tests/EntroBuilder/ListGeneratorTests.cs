using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EntroBuilder;
using EntroBuilder.Generators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests
{
    [TestClass]
    public class ListGeneratorTests
    {
        Random _rnd = new Random(123456);

        [TestMethod]
        public void IEnumerableOfT_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof (IEnumerable<int>),
                t => 1);

            IEnumerable<int> instance = generator.Next(_rnd) as IEnumerable<int>;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ICollectionOfT_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof (ICollection<int>),
                t => 1);

            ICollection<int> instance = generator.Next(_rnd) as ICollection<int>;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void IListOfT_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof (IList<int>),
                t => 1);

            IList<int> instance = generator.Next(_rnd) as IList<int>;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void IEnumerable_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof(IEnumerable),
                t => 1);

            IEnumerable instance = generator.Next(_rnd);

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ICollection_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof(ICollection),
                t => 1);

            ICollection instance = generator.Next(_rnd) as ICollection;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void IList_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof (IList),
                t => 1);

            IList instance = generator.Next(_rnd) as IList;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ListOfT_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof(List<int>),
                t => 1);

            List<int> instance = generator.Next(_rnd) as List<int>;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ConcurrentBagOfT_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof(ConcurrentBag<int>),
                t => 1);

            ConcurrentBag<int> instance = generator.Next(_rnd) as ConcurrentBag<int>;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void StackOfT_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof(Stack<int>),
                t => 1);

            Stack<int> instance = generator.Next(_rnd) as Stack<int>;

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void HashSetOfT_IsSupported()
        {
            var generator = new ListGenerator(
                new ListGenerator.Configuration(),
                typeof(HashSet<int>),
                t => 1);

            HashSet<int> instance = generator.Next(_rnd) as HashSet<int>;

            Assert.IsNotNull(instance);
        }
    }
}
