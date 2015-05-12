using System;

namespace EntroBuilder
{
    public class DateTimeRangeGenerator : RangeGenerator<DateTime>
    {
        public DateTimeRangeGenerator(DateTime minValue, DateTime maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static DateTime GenerateRandomBetween(DateTime minValue, DateTime maxValue, Random random)
        {
            long ticks = random.NextBetween(minValue.Ticks, maxValue.Ticks);
            return new DateTime(ticks);
        }
    }
}