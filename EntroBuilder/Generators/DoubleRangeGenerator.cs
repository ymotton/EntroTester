using System;

namespace EntroBuilder
{
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
}