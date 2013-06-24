using System;

namespace EntroBuilder
{
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
}