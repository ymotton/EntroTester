using System;
using EntroBuilder.Generators;

namespace EntroBuilder
{
    public class DecimalRangeGenerator : RangeGenerator<decimal>
    {
        public DecimalRangeGenerator(decimal minValue, decimal maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static decimal GenerateRandomBetween(decimal minValue, decimal maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
}