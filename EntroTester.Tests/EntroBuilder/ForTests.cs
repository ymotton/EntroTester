using System.Collections.Generic;
using System.Linq;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests.EntroBuilder
{
    [TestClass]
    public class ForTests
    {
        [TestMethod]
        public void For_InterfaceAndImplementation_ConfiguredImplementation()
        {
            var instance = Builder.Create<ClassWithInterfaceMember>()
                .For<IInterfaceToBeImplemented, InterfaceImplementation>()
                .Build();

            Assert.IsNotNull(instance.InterfaceProperty);
            Assert.IsNotNull(instance.InterfaceProperty.Id);
        }

        public class ClassWithInterfaceMember
        {
            public IInterfaceToBeImplemented InterfaceProperty { get; set; }
        }
        public interface IInterfaceToBeImplemented
        {
            string Id { get; set; }
        }
        public class InterfaceImplementation : IInterfaceToBeImplemented
        {
            public string Id { get; set; }
        }

    }
}
