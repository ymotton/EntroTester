using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests
{
    [TestClass]
    public class EntroBuilderTests
    {
        static List<Root> _roots;
        const byte ByteValue = 127;
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
        const string IBANPattern = "([A-Z]{2}[0-9]{2})( [0-9]{4}){3,6}";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var builder = EntroBuilder
                .Create<Root>()
                .Property(a => a.Byte_Value, Is.Value(ByteValue))
                .Property(a => a.String_ValueIn, Any.ValueIn(PossibleStrings))
                .Property(a => a.Integer_ValueIn, Any.ValueIn(PossibleIntegers))
                .Property(a => a.Integer_ValueBetween, Any.ValueBetween(MinInteger, MaxInteger))
                .Property(a => a.Double_ValueIn, Any.ValueIn(PossibleDoubles))
                .Property(a => a.Double_ValueBetween, Any.ValueBetween(MinDouble, MaxDouble))
                .Property(a => a.Decimal_ValueIn, Any.ValueIn(PossibleDecimals))
                .Property(a => a.Decimal_ValueBetween, Any.ValueBetween(MinDecimal, MaxDecimal))
                .Property(a => a.String_FromPattern, Any.ValueLike(IBANPattern))
                .Property(a => a.RootChild.String_ValueIn, Any.ValueIn(PossibleStrings))
                .Property(a => a.RootChild.Integer_ValueIn, Any.ValueIn(PossibleIntegers))
                .Property(a => a.RootChild.Integer_ValueBetween, Any.ValueBetween(MinInteger, MaxInteger))
                .Property(a => a.RootChild.Double_ValueIn, Any.ValueIn(PossibleDoubles))
                .Property(a => a.RootChild.Double_ValueBetween, Any.ValueBetween(MinDouble, MaxDouble))
                .Property(a => a.RootChild.Decimal_ValueIn, Any.ValueIn(PossibleDecimals))
                .Property(a => a.RootChild.Decimal_ValueBetween, Any.ValueBetween(MinDecimal, MaxDecimal))
                .Property(a => a.RootChild.NestedChild.String_ValueIn, Any.ValueIn(PossibleStrings));

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
        public void Build_ProducesRoot_WithRandomString()
        {
            Assert.IsTrue(_roots.Select(r => r.String_RandomValue).All(s => !string.IsNullOrEmpty(s)));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithAllExpectedIntegers()
        {
            Assert.IsFalse(_roots.Select(r => r.Integer_ValueIn).Except(PossibleIntegers).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithOnlyIntegersInRange()
        {
            Assert.IsTrue(_roots.Select(r => r.Integer_ValueBetween).All(i => i >= MinInteger && i <= MaxInteger));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithAllExpectedDoubles()
        {
            Assert.IsFalse(_roots.Select(r => r.Double_ValueIn).Except(PossibleDoubles).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithOnlyDoublesInRange()
        {
            Assert.IsTrue(_roots.Select(r => r.Double_ValueBetween).All(i => i >= MinDouble && i <= MaxDouble));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithAllExpectedDecimals()
        {
            Assert.IsFalse(_roots.Select(r => r.Decimal_ValueIn).Except(PossibleDecimals).Any());
        }

        [TestMethod]
        public void Build_ProducesRoot_WithOnlyDecimalsInRange()
        {
            Assert.IsTrue(_roots.Select(r => r.Decimal_ValueBetween).All(i => i >= MinDecimal && i <= MaxDecimal));
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
            Assert.IsTrue(_roots.Select(r => r.RootChild.Integer_ValueBetween).All(i => i >= MinInteger && i <= MaxInteger));
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithAllExpectedDoubles()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Double_ValueIn).Except(PossibleDoubles).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithOnlyDoublesInRange()
        {
            Assert.IsTrue(_roots.Select(r => r.RootChild.Double_ValueBetween).All(i => i >= MinDouble && i <= MaxDouble));
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithAllExpectedDecimals()
        {
            Assert.IsFalse(_roots.Select(r => r.RootChild.Decimal_ValueIn).Except(PossibleDecimals).Any());
        }

        [TestMethod]
        public void Build_ProducesRootChild_WithOnlyDecimalsInRange()
        {
            Assert.IsTrue(_roots.Select(r => r.RootChild.Decimal_ValueBetween).All(i => i >= MinDecimal && i <= MaxDecimal));
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

        [TestMethod]
        public void Build_ProducesRoot_WithExpectedByte()
        {
            Assert.IsTrue(_roots.All(i => i.Byte_Value == ByteValue));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithStringLikeRegex()
        {
            var regex = new Regex(IBANPattern);
            Assert.IsTrue(_roots.All(r => regex.IsMatch(r.String_FromPattern)));
        }
    }

    class Root
    {
        // Scalars are currently given a default random value, if no generation strategy is specified
        public Guid Id { get; set; }
        public string String_RandomValue { get; set; }

        // Nullable properties have 50% chance to be null, 50% a random value
        public DateTime? Nullable_DateTime { get; set; }

        // Currently unsupported - ignored
        public byte[] Ignore_ByteArray { get; set; }

        // Is.Value returns a generator that ensures the property always has the same value
        public byte Byte_Value { get; set; }

        // Any.ValueIn returns a Generator that randomly picks a value in a given enumeration
        public int Integer_ValueIn { get; set; }
        public double Double_ValueIn { get; set; }
        public decimal Decimal_ValueIn { get; set; }
        public string String_ValueIn { get; set; }
        
        // Any.ValueBetween returns a Generator that randomly picks a value in a given range
        // Only int, double, and decimal are currently supported, but it's easy enough to create your own...
        public int Integer_ValueBetween { get; set; }
        public double Double_ValueBetween { get; set; }
        public decimal Decimal_ValueBetween { get; set; }

        // Any.ValueLike returns a Generator that randomly produces values given a Regex Pattern
        // A sort of reverse Regex, if you will. 
        // Note: there are limitations to the supported patterns.
        public string String_FromPattern { get; set; }

        // Drill into Non-Scalars, to recursively generate
        public RootChild RootChild { get; set; }

        // Drill into collections, and add one element by default
        public List<RootChild> RootChildren { get; set; }
    }

    class RootChild
    {
        // Any.ValueIn returns a Generator that randomly picks a value in a given enumeration
        public int Integer_ValueIn { get; set; }
        public double Double_ValueIn { get; set; }
        public decimal Decimal_ValueIn { get; set; }
        public string String_ValueIn { get; set; }

        // Any.ValueBetween returns a Generator that randomly picks a value in a given range
        // Only int, double, and decimal are currently supported, but it's easy enough to create your own...
        public int Integer_ValueBetween { get; set; }
        public double Double_ValueBetween { get; set; }
        public decimal Decimal_ValueBetween { get; set; }

        // Same logic applies to properties in nested objects
        public NestedChild NestedChild { get; set; }
    }

    class NestedChild
    {
        public string String_ValueIn { get; set; }
    }
}
