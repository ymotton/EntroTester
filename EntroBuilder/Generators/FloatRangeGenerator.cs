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
            float range = Math.Abs(maxValue - minValue);
            float delta = (float)(random.NextDouble() * range);
            float result = minValue + delta;
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
            double range = Math.Abs(maxValue - minValue);
            double delta = random.NextDouble() * range;
            double result = minValue + delta;
            return result;
        }
    }
}