using System;

namespace EntroBuilder
{
    public class DateTimeGenerator : ScalarGenerator<DateTime>
    {
        protected override DateTime NextImpl(Random random)
        {
            return new DateTime((long)(random.NextDouble() * DateTime.MaxValue.Ticks));
        }
    }
}