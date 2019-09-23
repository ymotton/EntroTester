using System;
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
            public int[] Ints { get; set; }
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

        [TestMethod]
        public void Property_WithSelectManySelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.SelectMany(c => c.Ints), Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        [TestMethod]
        public void Property_WithFirstSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.First().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        
        [TestMethod]
        public void Property_WithFirstOrDefaultSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.FirstOrDefault().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        [TestMethod]
        public void Property_WithSingleSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.Single().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }
        [TestMethod]
        public void Property_WithSingleOrDefaultSelectorOfArray_Works()
        {
            var instance = Builder.Create<Parent>()
                .Property(x => x.Children.SingleOrDefault().Ints, Is.Value(1))
                .Build();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Children);
            Assert.IsNotNull(instance.Children.First());
            Assert.IsNotNull(instance.Children.First().Ints);
            Assert.AreEqual(1, instance.Children.First().Ints.First());
        }

        class NullableContainer
        {
            public short? NullableShort { get; set; }
            public int? NullableInt { get; set; }
            public long? NullableLong { get; set; }
            public float? NullableFloat { get; set; }
            public double? NullableDouble { get; set; }
            public decimal? NullableDecimal { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public Guid? NullableGuid { get; set; }
        }
        [TestMethod]
        public void Property_WithNullableType_TakesNonNullableGenerator()
        {
            var now = DateTime.UtcNow;
            var guid = Guid.NewGuid();
            var instance = Builder.Create<NullableContainer>()
                .Property(x => x.NullableInt, Is.Value(1))
                .Property(x => x.NullableLong, Is.Value(1L))
                .Property(x => x.NullableFloat, Is.Value(1.0f))
                .Property(x => x.NullableDouble, Is.Value(1.0))
                .Property(x => x.NullableDecimal, Is.Value(1.0M))
                .Property(x => x.NullableDateTime, Is.Value(now))
                .Property(x => x.NullableGuid, Is.Value(guid))
                .Build();

            Assert.IsNotNull(instance);
            Assert.AreEqual(1, instance.NullableInt);
            Assert.AreEqual(1L, instance.NullableLong);
            Assert.AreEqual(1.0f, instance.NullableFloat);
            Assert.AreEqual(1.0, instance.NullableDouble);
            Assert.AreEqual(1.0M, instance.NullableDecimal);
            Assert.AreEqual(now, instance.NullableDateTime);
            Assert.AreEqual(guid, instance.NullableGuid);
        }
        [TestMethod]
        public void Property_WithNullableFloat_CanHandleNonNullableDoubleGenerator()
        {
            var instance = Builder.Create<NullableContainer>()
                .Property(x => x.NullableShort, Is.Value(1))
                .Property(x => x.NullableFloat, Is.Value(1.0))
                .Build();

            Assert.IsNotNull(instance);
            Assert.AreEqual((short)1, instance.NullableShort);
            Assert.AreEqual(1.0f, instance.NullableFloat);
        }
    }
}
