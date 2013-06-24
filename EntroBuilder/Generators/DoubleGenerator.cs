using System;

namespace EntroBuilder
{
    public class DoubleGenerator : ScalarGenerator<double>
    {
        protected override double NextImpl(Random random)
        {
            return random.NextDouble() * int.MaxValue;
        }
    }
}