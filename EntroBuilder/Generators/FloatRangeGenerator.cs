using System;

namespace EntroBuilder
{
    public class FloatRangeGenerator : RangeGenerator<float>
    {
        public FloatRangeGenerator(float minValue, float maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static float GenerateRandomBetween(float minValue, float maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);

        }
    } 
    public class DoubleRangeGenerator : RangeGenerator<double>
    {
        public DoubleRangeGenerator(double minValue, double maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static double GenerateRandomBetween(double minValue, double maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
}