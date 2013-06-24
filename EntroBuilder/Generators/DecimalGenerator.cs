using System;

namespace EntroBuilder
{
    public class DecimalGenerator : ScalarGenerator<decimal>
    {
        protected override decimal NextImpl(Random random)
        {
            return (decimal)(random.NextDouble() * int.MaxValue);
        }
    }
}