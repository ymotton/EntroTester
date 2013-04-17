using System;

namespace EntroTester
{
    public class IntegerRangeGenerator : RangeGenerator<int>
    {
        public IntegerRangeGenerator(int minValue, int maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static int GenerateRandomBetween(int minValue, int maxValue, Random random)
        {
            var result = random.Next(minValue, maxValue);
            return result;
        }
    }
    public class DoubleRangeGenerator : RangeGenerator<double>
    {
        public DoubleRangeGenerator(double minValue, double maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static double GenerateRandomBetween(double minValue, double maxValue, Random random)
        {
            if (minValue < 0)
            {
                // Not uniformely random, but who cares
                var result = (random.NextDouble() * minValue) + (random.NextDouble() * maxValue);
                return result;
            }
            else
            {
                double range = maxValue - minValue;
                var result = random.NextDouble() * range + minValue;
                return result;
            }
        }
    }
    public class DecimalRangeGenerator : RangeGenerator<decimal>
    {
        public DecimalRangeGenerator(decimal minValue, decimal maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static decimal GenerateRandomBetween(decimal minValue, decimal maxValue, Random random)
        {
            if (minValue < 0)
            {
                // Not uniformely random, but who cares
                var result = ((decimal)random.NextDouble() * minValue) + ((decimal)random.NextDouble() * maxValue);
                return result;
            }
            else
            {
                var range = maxValue - minValue;
                var result = (decimal)random.NextDouble() * range + minValue;
                return result;
            }
        }
    }

    public class RangeGenerator<T> : IGenerator<T>
    {
        private readonly T _minValue;
        private readonly T _maxValue;
        private Func<T, T, Random, T> _generateRandomBetween;

        protected RangeGenerator(T minValue, T maxValue, Func<T, T, Random, T> generateRandomBetween)
        {
            AssertOrder(ref minValue, ref maxValue);
            _minValue = minValue;
            _maxValue = maxValue;
            _generateRandomBetween = generateRandomBetween;
        }
        static void AssertOrder(ref T min, ref T max)
        {
            var comparable = (IComparable)min;
            if (comparable.CompareTo(max) > 0)
            {
                T tmp = max;
                max = min;
                min = tmp;
            }
        }

        public T Next(Random random)
        {
            var result = _generateRandomBetween(_minValue, _maxValue, random);
            return result;
        }
        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }
}
