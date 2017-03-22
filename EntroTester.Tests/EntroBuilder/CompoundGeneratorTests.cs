using System;
using System.Linq;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests.EntroBuilder
{
    [TestClass]
    public class CompoundGeneratorTests
    {
        [TestMethod]
        public void CompoundGenerator_FollowsDefaultRatioProbabilities()
        {
            string value1 = nameof(value1);
            string value2 = nameof(value2);
            string value3 = nameof(value3);
            string value4 = nameof(value4);

            var n = 10000;
            var values = Builder.Create<string>()
                .For(Is.Value(value1).Or(Is.Value(value2)).Or(Is.Value(value3)).Or(Is.Value(value4)))
                .Take(n);

            var counts = values.GroupBy(x => x).Select(g => g.Count()).ToList();
            var mean = counts.Average();
            var stdDev = Math.Sqrt(counts.Select(i => Math.Pow(i - mean, 2)).Average()) / n;
            Assert.IsTrue(stdDev < 0.02);
        }

        [TestMethod]
        public void CompoundGenerator_FollowsCustomRatioProbabilities()
        {
            string value1 = nameof(value1);
            string value2 = nameof(value2);
            string value3 = nameof(value3);
            string value4 = nameof(value4);

            var n = 10000;
            var values = Builder.Create<string>()
                .For(Is.Value(value1).Or(Is.Value(value2), 2).Or(Is.Value(value3), 4).Or(Is.Value(value4), 8))
                .Take(n);

            var countMap = values.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var ratio12 = (double)countMap[value1]/countMap[value2];
            var ratio23 = (double)countMap[value2]/countMap[value3];
            var ratio34 = (double)countMap[value3]/countMap[value4];
            Assert.IsTrue(Math.Abs(ratio12 - 0.5) < 0.02);
            Assert.IsTrue(Math.Abs(ratio23 - 0.5) < 0.02);
            Assert.IsTrue(Math.Abs(ratio34 - 0.5) < 0.02);
        }
    }
}
