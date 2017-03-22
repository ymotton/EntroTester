using System;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests.EntroBuilder
{
    [TestClass]
    public class CustomGeneratorTests
    {
        [TestMethod]
        public void CustomGenerator_Considers_RandomInstance()
        {
            var sut = new CustomGenerator<int>(random => random.Next(0, 1000000));

            int expectedValue = sut.Next(new Random(0));

            Assert.AreEqual(expectedValue, 726243);
        }
    }
}
