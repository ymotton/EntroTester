using System.Collections.Generic;
using System.Linq;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests.EntroBuilder
{
    [TestClass]
    public class PropertySelectorTests
    {
        class Parent
        {
            public List<Child> Children { get; set; }
        }

        class Child
        {
            public List<string> Values { get; set; }
            public int[] Ints { get; set; }
        }

        [TestMethod]
        public void Property_WithSelectManySelector_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.SelectMany(c => c.Values), Is.Value("DUMMY"))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Values);
            Assert.AreEqual("DUMMY", instance.Children.First().Values.First());
        }

        [TestMethod]
        public void Property_WithSelectManySelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.SelectMany(c => c.Ints), Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        [TestMethod]
        public void Property_WithFirstSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.First().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        
        [TestMethod]
        public void Property_WithFirstOrDefaultSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.FirstOrDefault().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        [TestMethod]
        public void Property_WithSingleSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.Single().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        [TestMethod]
        public void Property_WithSingleOrDefaultSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.SingleOrDefault().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
    }
}
