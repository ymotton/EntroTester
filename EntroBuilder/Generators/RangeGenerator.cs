using System;

namespace EntroBuilder
{
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
