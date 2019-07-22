using System;
using System.Collections.Generic;
using System.Linq;
using EntroBuilder;
using EntroBuilder.FallbackGenerators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests.EntroBuilder
{
    [TestClass]
    public class BuilderTests
    {
        interface IAmSupported { }
        interface IAmUnsupported { }
        class ImplementAmSupported : IAmSupported
        {
            ImplementAmSupported() { }
        }
        class A
        {
            public IAmSupported Supported { get; set; }
        }
        class B
        {
            public IAmUnsupported Unsupported { get; set; }
        }

        class TestFallbackGenerator : IFallbackGenerator
        {
            readonly List<Type> _types;
            public TestFallbackGenerator(List<Type> types)
            {
                _types = types;
            }

            public bool TryNext(Type type, Random random, out object instance)
            {
                _types.Add(type);
                instance = null;
                return true;
            }
        }

        [TestMethod]
        public void Configure_FallbackGenerator_ForSupportedTypes()
        {
            var types = new List<Type>();
            new Builder<A>()
                .Configure(new Builder.Configuration
                {
                    FallbackGenerator = new TestFallbackGenerator(types)
                })
                .Build();
            Assert.AreEqual(1, types.Count);
            Assert.AreEqual(typeof(IAmSupported), types.Single());
        }

        [TestMethod]
        public void Configure_DefaultInterfaceImplementationFallbackGenerator_FindsFirstConcreteImplementation()
        {
            var instance = new Builder<A>()
                .Configure(new Builder.Configuration
                {
                    FallbackGenerator = new DefaultInterfaceImplementationFallbackGenerator()
                })
                .Build();
            Assert.AreEqual(typeof(ImplementAmSupported), instance.Supported?.GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Configure_DefaultInterfaceImplementationFallbackGenerator_UnMatchedThrowsNotSupportedException()
        {
            new Builder<B>()
                .Configure(new Builder.Configuration
                {
                    FallbackGenerator = new DefaultInterfaceImplementationFallbackGenerator()
                })
                .Build();
        }
    }
}
