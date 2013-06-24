using System;

namespace EntroBuilder
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
}