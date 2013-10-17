using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests
{
    [TestClass]
    public class PossibleValueDistributionTests
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
        const string StudentName = "Oliver Twist";

        static IEnumerable<Person> CreatePeople()
        {
            var people = Builder.Create<Person>()
                                        .Property(p => p.Country, Is.Value("Belgium"))
                                        .Take(2)
                .Concat(Builder.Create<Person>()
                                        .Property(p => p.Country, Is.Value("France"))
                                        .Take(2))
                .Concat(Builder.Create<Person>()
                                        .Property(p => p.Country, Is.Value("Germany"))
                                        .Take(2))
                .Concat(Builder.Create<Person>()
                                        .Property(p => p.Country, Is.Value("United Kingdom"))
                                        .Take(2));
            return people;
        }
        static int _cachedGeneratorCallCount;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var builder = Builder
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
                .Property(a => a.OtherPerson.Integer_ValueIn, Is.Value(1))
                .Property(a => a.OtherPerson.Integer_ValueBetween, Is.Value(1))
                .Property(a => a.OtherPerson.String_ValueIn, Is.Value("Fixed"))
                .Property(a => a.OtherPerson.Decimal_ValueIn, Is.Value(1M))
                .Property(a => a.OtherPerson.Decimal_ValueBetween, Is.Value(1M))
                .Property(a => a.OtherPerson.Double_ValueIn, Is.Value(1D))
                .Property(a => a.OtherPerson.Double_ValueBetween, Is.Value(1D))
                .Property(a => a.People, CustomGenerator.Create(() => CreatePeople().ToList()))
                .Property(a => a.Array, CustomGenerator.Create(() => CreatePeople().ToArray()))
                .Property(a => a.IList, CustomGenerator.Create(() => CreatePeople().ToList()))
                .Property(a => a.ICollection, CustomGenerator.Create(() => CreatePeople().ToList()))
                .Property(a => a.IEnumerable, CustomGenerator.Create(() => CreatePeople().ToList()))
                .Property(a => a.BindingList, CustomGenerator.Create(() => new BindingList<Person>(CreatePeople().ToList())))
                .Property(a => a.CachedPerson, CustomGenerator.Create(() =>
                    {
                        _cachedGeneratorCallCount++;
                        return new Person();
                    }, 
                    DelegateGeneratorOptions.Cached))
                .Property(a => a.Schools.SelectMany(s => s.Classes).SelectMany(c => c.Students).Select(s => s.Name), Is.Value(StudentName));

            _roots = builder.Take(10000).ToList();
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
        public void Build_ProducesRoot_WithNullableComplexTypeSometimesNull()
        {
            Assert.IsTrue(_roots.Any(i => i.Nullable_DateTime == null));
        }

        [TestMethod]
        public void Build_ProducesRoot_WithNullableComplexTypeSometimesNonNull()
        {
            Assert.IsTrue(_roots.Any(i => i.Nullable_DateTime != null));
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

        [TestMethod]
        public void Build_ProducesPeople_WithExpectedComposition()
        {
            Assert.IsTrue(_roots.All(r => r.People.Count(p => p.Country == "Belgium") == 2));
            Assert.IsTrue(_roots.All(r => r.People.Count(p => p.Country == "France") == 2));
            Assert.IsTrue(_roots.All(r => r.People.Count(p => p.Country == "Germany") == 2));
            Assert.IsTrue(_roots.All(r => r.People.Count(p => p.Country == "United Kingdom") == 2));
        }

        [TestMethod]
        public void Build_ProducesIList_WithExpectedComposition()
        {
            Assert.IsTrue(_roots.All(r => r.IList.Count(p => p.Country == "Belgium") == 2));
            Assert.IsTrue(_roots.All(r => r.IList.Count(p => p.Country == "France") == 2));
            Assert.IsTrue(_roots.All(r => r.IList.Count(p => p.Country == "Germany") == 2));
            Assert.IsTrue(_roots.All(r => r.IList.Count(p => p.Country == "United Kingdom") == 2));
        }

        [TestMethod]
        public void Build_ProducesIEnumerable_WithExpectedComposition()
        {
            Assert.IsTrue(_roots.All(r => r.IEnumerable.Count(p => p.Country == "Belgium") == 2));
            Assert.IsTrue(_roots.All(r => r.IEnumerable.Count(p => p.Country == "France") == 2));
            Assert.IsTrue(_roots.All(r => r.IEnumerable.Count(p => p.Country == "Germany") == 2));
            Assert.IsTrue(_roots.All(r => r.IEnumerable.Count(p => p.Country == "United Kingdom") == 2));
        }

        [TestMethod]
        public void Build_ProducesICollection_WithExpectedComposition()
        {
            Assert.IsTrue(_roots.All(r => r.ICollection.Count(p => p.Country == "Belgium") == 2));
            Assert.IsTrue(_roots.All(r => r.ICollection.Count(p => p.Country == "France") == 2));
            Assert.IsTrue(_roots.All(r => r.ICollection.Count(p => p.Country == "Germany") == 2));
            Assert.IsTrue(_roots.All(r => r.ICollection.Count(p => p.Country == "United Kingdom") == 2));
        }

        [TestMethod]
        public void Build_ProducesBindingList_WithExpectedComposition()
        {
            Assert.IsTrue(_roots.All(r => r.BindingList.Count(p => p.Country == "Belgium") == 2));
            Assert.IsTrue(_roots.All(r => r.BindingList.Count(p => p.Country == "France") == 2));
            Assert.IsTrue(_roots.All(r => r.BindingList.Count(p => p.Country == "Germany") == 2));
            Assert.IsTrue(_roots.All(r => r.BindingList.Count(p => p.Country == "United Kingdom") == 2));
        }

        [TestMethod]
        public void Build_ProducesArray_WithExpectedComposition()
        {
            Assert.IsTrue(_roots.All(r => r.Array.Count(p => p.Country == "Belgium") == 2));
            Assert.IsTrue(_roots.All(r => r.Array.Count(p => p.Country == "France") == 2));
            Assert.IsTrue(_roots.All(r => r.Array.Count(p => p.Country == "Germany") == 2));
            Assert.IsTrue(_roots.All(r => r.Array.Count(p => p.Country == "United Kingdom") == 2));
        }

        [TestMethod]
        public void Build_ProducesMyEnum_WithExpectedValue()
        {
            Assert.IsTrue(_roots.All(r => Enum.GetValues(typeof(MyEnum)).Cast<int>().Contains((int)r.MyEnum)));
        }

        [TestMethod]
        public void Build_CachedCustomGenerator_IsCalledOnce()
        {
            Assert.AreEqual(1, _cachedGeneratorCallCount);
        }

        [TestMethod]
        public void Build_ProducesStudents_WithExpectedName()
        {
            Assert.IsTrue(_roots.All(r => r.Schools.SelectMany(s => s.Classes).SelectMany(c => c.Students).All(p => p.Name == StudentName)));
        }

        [TestMethod]
        public void Build_ProducesInt_WithDifferentValues()
        {
            var values = Builder.Create<int>().Take(1000);

            bool producesDifferentValues = values.GroupBy(i => i).Count() > 100;

            Assert.IsTrue(producesDifferentValues);
        }

        [TestMethod]
        public void Build_ProducesString_WithDifferentValues()
        {
            var values = Builder.Create<string>().Take(1000);

            bool producesDifferentValues = values.GroupBy(i => i).Count() > 100;

            Assert.IsTrue(producesDifferentValues);
        }

        [TestMethod]
        public void Build_ProducesString_WithExpectedValues()
        {
            var values = Builder.Create<string>()
                                .For(Any.ValueIn(PossibleStrings))
                                .Take(1000);

            var distinctValues = values.Distinct().ToList();
            var distinctValueCount = distinctValues.Count();
            Assert.AreEqual(PossibleStrings.Length, distinctValueCount);
            Assert.IsTrue(PossibleStrings.Contains(distinctValues[0]));
            Assert.IsTrue(PossibleStrings.Contains(distinctValues[1]));
            Assert.IsTrue(PossibleStrings.Contains(distinctValues[2]));
        }

        [TestMethod]
        public void Build_ProducesInt_WithExpectedValues()
        {
            var values = Builder.Create<int>()
                                .For(Any.ValueIn(PossibleIntegers))
                                .Take(1000);

            var distinctValues = values.Distinct().ToList();
            var distinctValueCount = distinctValues.Count();
            Assert.AreEqual(PossibleIntegers.Length, distinctValueCount);
            Assert.IsTrue(PossibleIntegers.Contains(distinctValues[0]));
            Assert.IsTrue(PossibleIntegers.Contains(distinctValues[1]));
            Assert.IsTrue(PossibleIntegers.Contains(distinctValues[2]));
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

            // Drill into collections given a delegate generator to influence the composition of the collection
            public List<Person> People { get; set; }
            public Person[] Array { get; set; }
            public IList<Person> IList { get; set; }
            public BindingList<Person> BindingList { get; set; }
            public ICollection<Person> ICollection { get; set; }
            public IEnumerable<Person> IEnumerable { get; set; }

            // Drill into nested collections
            public List<School> Schools { get; set; }

            // Allows to cache a custom generator, so it doesn't need to be called each time
            public Person CachedPerson { get; set; }

            // Differentiating between properties of the same type
            public Person OtherPerson { get; set; }

            public MyEnum MyEnum { get; set; }
        }

        class School
        {
            public List<Class> Classes { get; set; }
        }

        class Class
        {
            public List<Person> Students { get; set; }
        }

        class Person
        {
            public string Name { get; set; }
            public string Country { get; set; }

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
        }

        enum MyEnum
        {
            One = 1,
            Two = 2,
            Max = int.MaxValue
        }
    }
}