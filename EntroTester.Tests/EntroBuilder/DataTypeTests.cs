using System.Collections.Generic;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace EntroTester.Tests
{
    [TestClass]
    public class DataTypeTests
    {
        [TestMethod]
        public void Build_WithReferenceType_ProducesPrimitives()
        {
            var instance = Builder.Create<ReferenceType>()
                                  .Take(10)
                                  .ToList();

            Assert.IsTrue(instance.Any(t => t.Bool));
            Assert.IsTrue(instance.Any(t => t.Decimal != 0.0M));
            Assert.IsTrue(instance.Any(t => t.Double != 0.0D));
            Assert.IsTrue(instance.Any(t => t.Enum != (MyEnum)0));
            Assert.IsTrue(instance.Any(t => t.Float != 0.0F));
            Assert.IsTrue(instance.Any(t => t.Integer != 0));
            Assert.IsTrue(instance.Any(t => t.Long != 0));
            Assert.IsTrue(instance.Any(t => t.Short != 0));
            Assert.IsTrue(instance.Any(t => t.UnsignedInteger != 0));
            Assert.IsTrue(instance.Any(t => t.UnsignedLong != 0));
            Assert.IsTrue(instance.Any(t => t.UnsignedShort != 0));
        }

        [TestMethod]
        public void Build_WithReferenceType_ProducesNullablePrimitives()
        {
            var instance = Builder.Create<ReferenceType>()
                                  .Take(10)
                                  .ToList();

            Assert.IsTrue(instance.Any(t => t.NullableBool.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableDecimal.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableDouble.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableEnum.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableFloat.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableInteger.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableLong.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableShort.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableUnsignedInteger.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableUnsignedLong.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableUnsignedShort.HasValue)); 
        }

        [TestMethod]
        public void Build_WithReferenceType_ProducesComplexTypes()
        {
            var instance = Builder.Create<ReferenceType>()
                                  .Take(10)
                                  .ToList();

            Assert.IsTrue(instance.Any(t => t.DateTime != DateTime.MinValue));
            Assert.IsTrue(instance.Any(t => t.Guid != Guid.Empty));
            Assert.IsTrue(instance.Any(t => t.NestedComplexType.DateTime != DateTime.MinValue));
        }

        [TestMethod]
        public void Build_WithReferenceType_ProducesNullableComplexTypes()
        {
            var instance = Builder.Create<ReferenceType>()
                .Take(10)
                .ToList();

            Assert.IsTrue(instance.Any(t => t.NullableDateTime.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableGuid.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableNestedComplexType.HasValue && t.NullableNestedComplexType.Value.DateTime != DateTime.MinValue));
        }

        [TestMethod]
        public void Build_WithReferenceType_ProducesReferenceTypes()
        {
            var instance = Builder.Create<ReferenceType>()
                .Take(10)
                .ToList();

            Assert.IsTrue(instance.Any(t => t.Class != null));
            Assert.IsTrue(instance.Any(t => t.String != null));
            Assert.IsTrue(instance.Any(t => t.NestedComplexType.String != null));
            Assert.IsTrue(instance.Any(t => t.NullableNestedComplexType.HasValue && t.NullableNestedComplexType.Value.String != null));
        }

        [TestMethod]
        public void Build_WithReferenceType_ProducesSequences()
        {
            var instance = Builder.Create<ReferenceType>()
                .Take(10)
                .ToList();

            Assert.IsTrue(instance.All(t => t.IEnumerable.Any()));
            Assert.IsTrue(instance.All(t => t.ICollection.Any()));
            Assert.IsTrue(instance.All(t => t.IList.Any()));
            Assert.IsTrue(instance.All(t => t.List.Any()));
            Assert.IsTrue(instance.All(t => t.Array == null));
        }        
        
        [TestMethod]
        public void Build_WithComplexType_ProducesPrimitives()
        {
            var instance = Builder.Create<ComplexType>()
                                  .Take(10)
                                  .ToList();

            Assert.IsTrue(instance.Any(t => t.Bool));
            Assert.IsTrue(instance.Any(t => t.Decimal != 0.0M));
            Assert.IsTrue(instance.Any(t => t.Double != 0.0D));
            Assert.IsTrue(instance.Any(t => t.Enum != (MyEnum)0));
            Assert.IsTrue(instance.Any(t => t.Float != 0.0F));
            Assert.IsTrue(instance.Any(t => t.Integer != 0));
            Assert.IsTrue(instance.Any(t => t.Long != 0));
            Assert.IsTrue(instance.Any(t => t.Short != 0));
            Assert.IsTrue(instance.Any(t => t.UnsignedInteger != 0));
            Assert.IsTrue(instance.Any(t => t.UnsignedLong != 0));
            Assert.IsTrue(instance.Any(t => t.UnsignedShort != 0));
        }

        [TestMethod]
        public void Build_WithComplexType_ProducesNullablePrimitives()
        {
            var instance = Builder.Create<ComplexType>()
                                  .Take(10)
                                  .ToList();

            Assert.IsTrue(instance.Any(t => t.NullableBool.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableDecimal.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableDouble.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableEnum.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableFloat.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableInteger.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableLong.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableShort.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableUnsignedInteger.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableUnsignedLong.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableUnsignedShort.HasValue)); 
        }

        [TestMethod]
        public void Build_WithComplexType_ProducesComplexTypes()
        {
            var instance = Builder.Create<ComplexType>()
                                  .Take(10)
                                  .ToList();

            Assert.IsTrue(instance.Any(t => t.DateTime != DateTime.MinValue));
            Assert.IsTrue(instance.Any(t => t.Guid != Guid.Empty));
            Assert.IsTrue(instance.Any(t => t.NestedComplexType.DateTime != DateTime.MinValue));
        }

        [TestMethod]
        public void Build_WithComplexType_ProducesNullableComplexTypes()
        {
            var instance = Builder.Create<ComplexType>()
                .Take(10)
                .ToList();

            Assert.IsTrue(instance.Any(t => t.NullableDateTime.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableGuid.HasValue));
            Assert.IsTrue(instance.Any(t => t.NullableNestedComplexType.HasValue && t.NullableNestedComplexType.Value.DateTime != DateTime.MinValue));
        }

        [TestMethod]
        public void Build_WithComplexType_ProducesReferenceTypes()
        {
            var instance = Builder.Create<ComplexType>()
                .Take(10)
                .ToList();

            Assert.IsTrue(instance.Any(t => t.Class != null));
            Assert.IsTrue(instance.Any(t => t.String != null));
            Assert.IsTrue(instance.Any(t => t.NestedComplexType.String != null));
            Assert.IsTrue(instance.Any(t => t.NullableNestedComplexType.HasValue && t.NullableNestedComplexType.Value.String != null));
        }

        [TestMethod]
        public void Build_WithComplexType_ProducesSequences()
        {
            var instance = Builder.Create<ComplexType>()
                .Take(10)
                .ToList();

            Assert.IsTrue(instance.All(t => t.IEnumerable.Any()));
            Assert.IsTrue(instance.All(t => t.ICollection.Any()));
            Assert.IsTrue(instance.All(t => t.IList.Any()));
            Assert.IsTrue(instance.All(t => t.List.Any()));
            Assert.IsTrue(instance.All(t => t.Array == null));
        }

        [TestMethod]
        public void Build_WithPrivateDefaultCtor_ProducesInstance()
        {
            var instance = Builder.Create<ClassWithPrivateDefaultCtor>()
                                  .Build();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassWithPrivateDefaultCtor));
        }

        [TestMethod]
        [ExpectedException(typeof(MissingMethodException))]
        public void Build_WithNoDefaultCtor_ThrowsException()
        {
            Builder.Create<ClassWithNoDefaultCtor>().Build();

            Assert.Fail("This should fail.");
        }

        [TestMethod]
        public void Build_WithComplexTypeWithPrivateMember_ProducesValue()
        {
            var instance = Builder.Create<ComplexTypeWithPrivateMember>()
                                  .Build();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ComplexTypeWithPrivateMember));
            Assert.IsNotNull(instance.ToString());
        }

        [TestMethod]
        public void Build_WithComplexTypeWithPrivateReadOnlyMember_ProducesValue()
        {
            var instance = Builder.Create<ComplexTypeWithPrivateReadOnlyMember>()
                                  .Build();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ComplexTypeWithPrivateReadOnlyMember));
            Assert.IsNotNull(instance.ToString());
        }
        
        [TestMethod]
        public void Build_WithPrivatePropertySetterOnBase_ProducesValue()
        {
            var instance = Builder.Create<DerivedClassForBaseWithPrivatePropertySetter>()
                                  .Build();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(DerivedClassForBaseWithPrivatePropertySetter));
            Assert.IsNotNull(instance.PropertyWithPrivateSetter);
        }

        class ReferenceType
        {
            public bool Bool { get; set; }
            public short Short { get; set; }
            public ushort UnsignedShort { get; set; }
            public int Integer { get; set; }
            public uint UnsignedInteger { get; set; }
            public long Long { get; set; }
            public ulong UnsignedLong { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public Guid Guid { get; set; }
            public DateTime DateTime { get; set; }
            public MyEnum Enum { get; set; }
            public NestedComplexType NestedComplexType { get; set; }

            public bool? NullableBool { get; set; }
            public short? NullableShort { get; set; }
            public ushort? NullableUnsignedShort { get; set; }
            public int? NullableInteger { get; set; }
            public uint? NullableUnsignedInteger { get; set; }
            public long? NullableLong { get; set; }
            public ulong? NullableUnsignedLong { get; set; }
            public float? NullableFloat { get; set; }
            public double? NullableDouble { get; set; }
            public decimal? NullableDecimal { get; set; }
            public Guid? NullableGuid { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public MyEnum? NullableEnum { get; set; }
            public NestedComplexType? NullableNestedComplexType { get; set; }

            public string String { get; set; }
            public ClassWithPrivateDefaultCtor Class { get; set; }

            public IEnumerable<Guid> IEnumerable { get; set; }
            public ICollection<Guid> ICollection { get; set; }
            public IList<Guid> IList { get; set; }
            public List<Guid> List { get; set; }
            public Guid[] Array { get; set; }
        }
        struct ComplexType
        {
            public bool Bool;
            public short Short;
            public ushort UnsignedShort;
            public int Integer;
            public uint UnsignedInteger;
            public long Long;
            public ulong UnsignedLong;
            public float Float;
            public double Double;
            public decimal Decimal;
            public Guid Guid;
            public DateTime DateTime;
            public MyEnum Enum;
            public NestedComplexType NestedComplexType;

            public bool? NullableBool;
            public short? NullableShort;
            public ushort? NullableUnsignedShort;
            public int? NullableInteger;
            public uint? NullableUnsignedInteger;
            public long? NullableLong;
            public ulong? NullableUnsignedLong;
            public float? NullableFloat;
            public double? NullableDouble;
            public decimal? NullableDecimal;
            public Guid? NullableGuid;
            public DateTime? NullableDateTime;
            public MyEnum? NullableEnum;
            public NestedComplexType? NullableNestedComplexType;
            
            public string String;
            public ClassWithPrivateDefaultCtor Class;

            public IEnumerable<Guid> IEnumerable;
            public ICollection<Guid> ICollection;
            public IList<Guid> IList;
            public List<Guid> List;
            public Guid[] Array;
        }
        struct NestedComplexType
        {
            public string String;
            public DateTime DateTime;
        }
        enum MyEnum
        {
            None = 0,
            One = 1,
            Two = 2,
            Max = int.MaxValue
        }
        struct ComplexTypeWithPrivateMember
        {
            private string String;
            public override string ToString()
            {
                return String;
            }
        }
        struct ComplexTypeWithPrivateReadOnlyMember
        {
            private readonly string String;
            public override string ToString()
            {
                return String;
            }
        }
        class ClassWithPrivateDefaultCtor
        {
            ClassWithPrivateDefaultCtor() { }
        }
        class ClassWithNoDefaultCtor
        {
            public ClassWithNoDefaultCtor(string required) { }
        }
        abstract class BaseClassWithPrivatePropertySetter
        {
            public string PropertyWithPrivateSetter { get; private set; }
        }
        class DerivedClassForBaseWithPrivatePropertySetter : BaseClassWithPrivatePropertySetter
        {
        }
    }
}
