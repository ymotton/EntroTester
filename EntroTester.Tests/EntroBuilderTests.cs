using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests
{
    [TestClass]
    public class EntroBuilderTests
    {
        static List<Root> _roots;
        readonly static string[] PossibleStrings = new[] { "a", "b", null };
        readonly static int[] PossibleIntegers = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        const int MinInteger = int.MinValue / 2;
        const int MaxInteger = int.MaxValue / 2;
        readonly static double[] PossibleDoubles = new[] { double.MinValue, -1.0002, 0, 1.0003, double.MaxValue };
        const double MinDouble = double.MinValue / 2;
        const double MaxDouble = double.MaxValue / 2;
        readonly static decimal[] PossibleDecimals = new[] { decimal.MinValue, -1.0002M, 0, 1.0003M, decimal.MaxValue };
        const decimal MinDecimal = decimal.MinValue / 2;
        const decimal MaxDecimal = decimal.MaxValue / 2;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var builder = EntroBuilder
                .Create<Root>()
                .ForProperty(a => a.String_ValueIn, Any.ValueIn(PossibleStrings))
                .ForProperty(a => a.Integer_ValueIn, Any.ValueIn(PossibleIntegers))
                .ForProperty(a => a.Integer_ValueBetween, Any.ValueBetween(MinInteger, MaxInteger))
                .ForProperty(a => a.Double_ValueIn, Any.ValueIn(PossibleDoubles))
                .ForProperty(a => a.Double_ValueBetween, Any.ValueBetween(MinDouble, MaxDouble))
                .ForProperty(a => a.Decimal_ValueIn, Any.ValueIn(PossibleDecimals))
                .ForProperty(a => a.Decimal_ValueBetween, Any.ValueBetween(MinDecimal, MaxDecimal))
                .ForProperty(a => a.RootChild.String_ValueIn, Any.ValueIn(PossibleStrings))
                .ForProperty(a => a.RootChild.Integer_ValueIn, Any.ValueIn(PossibleIntegers))
                .ForProperty(a => a.RootChild.Integer_ValueBetween, Any.ValueBetween(MinInteger, MaxInteger))
                .ForProperty(a => a.RootChild.Double_ValueIn, Any.ValueIn(PossibleDoubles))
                .ForProperty(a => a.RootChild.Double_ValueBetween, Any.ValueBetween(MinDouble, MaxDouble))
                .ForProperty(a => a.RootChild.Decimal_ValueIn, Any.ValueIn(PossibleDecimals))
                .ForProperty(a => a.RootChild.Decimal_ValueBetween, Any.ValueBetween(MinDecimal, MaxDecimal))
                .ForProperty(a => a.RootChild.NestedChild.String_ValueIn, Any.ValueIn(PossibleStrings));

            _roots = builder.Take(100000).ToList();
        }

        [TestMethod]
        public void Build_ProducesRoot_NonNull()
        {
            Assert.IsTrue(_roots.All(i => i != null));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithAllExpectedStrings()
        {
            Assert.IsFalse(_roots.Select(r => r.String_ValueIn).Except(PossibleStrings).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithAllExpectedIntegers()
        {
            Assert.IsFalse(_roots.Select(r => r.Integer_ValueIn).Except(PossibleIntegers).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithOnlyIntegersInRange()
        {
            Assert.IsFalse(_roots.Select(r => r.Integer_ValueBetween).Where(i => i < MinInteger || i > MaxInteger).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithAllExpectedDoubles()
        {
            Assert.IsFalse(_roots.Select(r => r.Double_ValueIn).Except(PossibleDoubles).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithOnlyDoublesInRange()
        {
            Assert.IsFalse(_roots.Select(r => r.Double_ValueBetween).Where(i => i < MinDouble || i > MaxDouble).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithAllExpectedDecimals()
        {
            Assert.IsFalse(_roots.Select(r => r.Decimal_ValueIn).Except(PossibleDecimals).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithOnlyDecimalsInRange()
        {
            Assert.IsFalse(_roots.Select(r => r.Decimal_ValueBetween).Where(i => i < MinDecimal || i > MaxDecimal).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithNonEmptyIds()
        {
            Assert.IsTrue(_roots.All(i => i.Id != Guid.Empty));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithNullableDateTimeSometimesNull()
        {
            Assert.IsTrue(_roots.Any(i => i.Nullable_DateTime == null));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithNullableDateTimeSometimesNonNull()
        {
            Assert.IsTrue(_roots.Any(i => i.Nullable_DateTime != null));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithIngoreByteArrayAlwaysNull()
        {
            Assert.IsTrue(_roots.All(i => i.Ignore_ByteArray == null));
        }

        [TestMethod]
        public void Build_ProducesRootChild_NonNull()
        {
            Assert.IsTrue(_roots.All(i => i.RootChild != null));
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithAllExpectedStrings()
        {
            Assert.IsTrue(_roots.All(i => i.RootChild != null));
            Assert.IsFalse(_roots.Select(r => r.RootChild.String_ValueIn).Except(PossibleStrings).Any());
        }
        
        [TestMethod]
        public void Build_ProducesRootChild_WithAllExpectedIntegers()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Integer_ValueIn).Except(PossibleIntegers).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithOnlyIntegersInRange()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Integer_ValueBetween).Where(i => i < MinInteger || i > MaxInteger).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithAllExpectedDoubles()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Double_ValueIn).Except(PossibleDoubles).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithOnlyDoublesInRange()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Double_ValueBetween).Where(i => i < MinDouble || i > MaxDouble).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithAllExpectedDecimals()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Decimal_ValueIn).Except(PossibleDecimals).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithOnlyDecimalsInRange()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Decimal_ValueBetween).Where(i => i < MinDecimal || i > MaxDecimal).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChildren_ContainsOneElement()
        {
            Assert.IsTrue(_roots.All(i => i.RootChildren != null));
            Assert.IsTrue(_roots.All(i => i.RootChildren.Any()));
        }

        [TestMethod]
        public void Build_ProducesNestedChild_NonNull()
        {
            Assert.IsTrue(_roots.Select(i => i.RootChild).All(i => i.NestedChild != null));
        }

        [TestMethod]
        public void Build_ProducesNestedChild_WithAllExpectedStrings()
        {
            Assert.IsTrue(_roots.All(i => i.RootChild != null));
            Assert.IsFalse(_roots.Select(r => r.RootChild.NestedChild.String_ValueIn).Except(PossibleStrings).Any());
        }
    }

    class Root
    {
        public Guid Id { get; set; }
        public DateTime? Nullable_DateTime { get; set; }
        public byte[] Ignore_ByteArray { get; set; }

        public int Integer_ValueIn { get; set; }
        public int Integer_ValueBetween { get; set; }

        public double Double_ValueIn { get; set; }
        public double Double_ValueBetween { get; set; }

        public decimal Decimal_ValueIn { get; set; }
        public decimal Decimal_ValueBetween { get; set; }

        public string String_ValueIn { get; set; }

        public RootChild RootChild { get; set; }

        public List<RootChild> RootChildren { get; set; }
    }

    class RootChild
    {
        public int Integer_ValueIn { get; set; }
        public int Integer_ValueBetween { get; set; }

        public double Double_ValueIn { get; set; }
        public double Double_ValueBetween { get; set; }

        public decimal Decimal_ValueIn { get; set; }
        public decimal Decimal_ValueBetween { get; set; }

        public string String_ValueIn { get; set; }

        public NestedChild NestedChild { get; set; }
    }

    class NestedChild
    {
        public string String_ValueIn { get; set; }
    }
}
