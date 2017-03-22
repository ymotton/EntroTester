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
    }
}
