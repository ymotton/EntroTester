using System.Collections.Generic;
using EntroTester.ObjectDumper;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests.EntroTester
{
    [TestClass]
    public class DumperTests
    {
        class Parent
        {
            public List<Child> Children { get; set; }
            public string Foo { get; set; } 
        }

        class Child
        {
            public GrandChild1 GrandChild1 { get; set; }
            public List<GrandChild2> GrandChildren { get; set; }
        }

        class GrandChild1
        {
            public string String { get; set; }
            public List<string> Strings { get; set; }
        }

        class GrandChild2
        {
            public int Integer { get; set; }
            public List<int> Ints { get; set; }
        }
        [TestMethod]
        public void Foo()
        {
            var parents = Builder.Create<List<Parent>>().Take(3);
            var dumpToString = parents.DumpToString("parents");
            Assert.AreEqual("", dumpToString);
        }
    }
}
