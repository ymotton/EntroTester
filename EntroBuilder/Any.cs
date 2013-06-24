namespace EntroBuilder
{
    public static class Any
    {
        public static IGenerator<T> ValueIn<T>(params T[] possibleValues)
        {
            var generator = new SequenceGenerator<T>(possibleValues);
            return generator;
        }
        public static IGenerator<string> ValueLike(string regularExpression)
        {
            var generator = new RegexGenerator(regularExpression);
            return generator;
        }
        public static IGenerator<int> ValueBetween(int minValue, int maxValue)
        {
            var generator = new IntegerRangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<double> ValueBetween(double minValue, double maxValue)
        {
            var generator = new DoubleRangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<decimal> ValueBetween(decimal minValue, decimal maxValue)
        {
            var generator = new DecimalRangeGenerator(minValue, maxValue);
            return generator;
        }
    }
}