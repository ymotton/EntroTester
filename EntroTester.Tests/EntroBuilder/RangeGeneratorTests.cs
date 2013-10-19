using System;
using System.Linq;
using EntroBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntroTester.Tests.EntroBuilder
{
    [TestClass]
    public class RangeGeneratorTests
    {
        Random _rnd = new Random(123456);
        
        [TestMethod]
        public void Next_SByteRange_ProducesOnlyValuesInRange()
        {
            var generator = new SByteRangeGenerator(-5, 5);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i < -5));
            Assert.IsFalse(distinctItems.Any(i => i > 5));
        }    
    
        [TestMethod]
        public void Next_ByteRange_ProducesOnlyValuesInRange()
        {
            var generator = new ByteRangeGenerator(0, 10);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i > 10));
        } 
       
        [TestMethod]
        public void Next_Int16Range_ProducesOnlyValuesInRange()
        {
            var generator = new Int16RangeGenerator(-5, 5);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i < -5));
            Assert.IsFalse(distinctItems.Any(i => i > 5));
        }    
    
        [TestMethod]
        public void Next_UInt16Range_ProducesOnlyValuesInRange()
        {
            var generator = new UInt16RangeGenerator(0, 10);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i > 10));
        }       

        [TestMethod]
        public void Next_Int32Range_ProducesOnlyValuesInRange()
        {
            var generator = new Int32RangeGenerator(-5, 5);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i < -5));
            Assert.IsFalse(distinctItems.Any(i => i > 5));
        }    
    
        [TestMethod]
        public void Next_UInt32Range_ProducesOnlyValuesInRange()
        {
            var generator = new UInt32RangeGenerator(0, 10);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i > 10));
        }

        [TestMethod]
        public void Next_Int64Range_ProducesOnlyValuesInRange()
        {
            var generator = new Int64RangeGenerator(-5, 5);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i < -5));
            Assert.IsFalse(distinctItems.Any(i => i > 5));
        }    
    
        [TestMethod]
        public void Next_UInt64Range_ProducesOnlyValuesInRange()
        {
            var generator = new UInt64RangeGenerator(0, 10);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i > 10));
        }

        [TestMethod]
        public void Next_FloatRange_ProducesOnlyValuesInRange()
        {
            var generator = new FloatRangeGenerator(-5, 5);

            var distinctItems = generator.AsEnumerable(_rnd).Take(1000).Select(f => (int)Math.Round(f, 0, MidpointRounding.ToEven)).Distinct().ToList();

            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i < -5));
            Assert.IsFalse(distinctItems.Any(i => i > 5));
        }

        [TestMethod]
        public void Next_FloatRange_DoesntOverflowForLargeValues()
        {
            var generator = new FloatRangeGenerator(float.MinValue, float.MaxValue);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();

            Assert.IsTrue(items.All(i => !float.IsInfinity(i)));
        }

        [TestMethod]
        public void Next_FloatRange_ProducesValuesBetweenMinValueAndZero()
        {
            var generator = new FloatRangeGenerator(float.MinValue, 0);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();
            Assert.IsTrue(items.All(i => i <= 0));
        }

        [TestMethod]
        public void Next_FloatRange_ProducesValuesBetweenZeroAndMaxValue()
        {
            var generator = new FloatRangeGenerator(0, float.MaxValue);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();
            Assert.IsTrue(items.All(i => i >= 0));
        }
    
        [TestMethod]
        public void Next_DoubleRange_ProducesOnlyValuesInRange()
        {
            var generator = new DoubleRangeGenerator(-5, 5);

            var distinctItems = generator.AsEnumerable(_rnd).Take(1000).Select(f => Math.Round(f, 0, MidpointRounding.ToEven)).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i < -5));
            Assert.IsFalse(distinctItems.Any(i => i > 5));
        }

        [TestMethod]
        public void Next_DoubleRange_OverflowsForLargeValues()
        {
            var generator = new DoubleRangeGenerator(double.MinValue, double.MaxValue);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();

            Assert.IsTrue(items.All(i => double.IsInfinity(i)));
        }

        [TestMethod]
        public void Next_DoubleRange_ProducesValuesBetweenMinValueAndZero()
        {
            var generator = new DoubleRangeGenerator(double.MinValue, 0);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();
            Assert.IsTrue(items.All(i => i <= 0));
        }

        [TestMethod]
        public void Next_DoubleRange_ProducesValuesBetweenZeroAndMaxValue()
        {
            var generator = new DoubleRangeGenerator(0, double.MaxValue);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();
            Assert.IsTrue(items.All(i => i >= 0));
        }
        
        [TestMethod]
        public void Next_DecimalRange_ProducesOnlyValuesInRange()
        {
            var generator = new DecimalRangeGenerator(-5, 5);

            var distinctItems = generator.AsEnumerable(_rnd).Take(100).Select(f => Math.Round(f, 0, MidpointRounding.ToEven)).Distinct().ToList();
            
            Assert.AreEqual(11, distinctItems.Count);
            Assert.IsFalse(distinctItems.Any(i => i < -5));
            Assert.IsFalse(distinctItems.Any(i => i > 5));
        }

        [TestMethod]
        public void Next_DecimalRange_DoesntOverflowForLargeValues()
        {
            var generator = new DecimalRangeGenerator(decimal.MinValue, decimal.MaxValue);

            generator.AsEnumerable(_rnd).Take(100).ToList();
        }

        [TestMethod]
        public void Next_DecimalRange_ProducesValuesBetweenMinValueAndZero()
        {
            var generator = new DecimalRangeGenerator(decimal.MinValue, 0);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();
            Assert.IsTrue(items.All(i => i <= 0));
        }

        [TestMethod]
        public void Next_DecimalRange_ProducesValuesBetweenZeroAndMaxValue()
        {
            var generator = new DecimalRangeGenerator(0, decimal.MaxValue);

            var items = generator.AsEnumerable(_rnd).Take(100).ToList();
            Assert.IsTrue(items.All(i => i >= 0));
        }
    }
}
