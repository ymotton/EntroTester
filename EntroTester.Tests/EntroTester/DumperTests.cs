using System.Collections.Generic;
using System.Linq;
using EntroTester.ObjectDumper;
using EntroBuilder;
using EntroBuilder.Generators;
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
            public GrandChild1 GrandChild { get; set; }
            public List<GrandChild2> GrandChildren { get; set; }

            public class GrandChild1
            {
                public string String { get; set; }
                public List<string> Strings { get; set; }
            }
            public class GrandChild2
            {
                public int Integer { get; set; }
                public List<int> Ints { get; set; }
            }
        }

        [TestMethod]
        public void VerifyDumpToString()
        {
            var parents = Builder.Create<List<Parent>>()
                .Configure(new ListGenerator.Configuration { MaxItems = 5 })
                .Take(3).ToList();
            var dumpToString = parents.DumpToString("parents");
            Assert.IsNotNull(dumpToString);
        }
    }
}
