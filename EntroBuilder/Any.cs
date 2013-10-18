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

        public static IGenerator<sbyte> ValueBetween(sbyte minValue, sbyte maxValue)
        {
            var generator = new SByteRangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<byte> ValueBetween(byte minValue, byte maxValue)
        {
            var generator = new ByteRangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<short> ValueBetween(short minValue, short maxValue)
        {
            var generator = new Int16RangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<ushort> ValueBetween(ushort minValue, ushort maxValue)
        {
            var generator = new UInt16RangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<int> ValueBetween(int minValue, int maxValue)
        {
            var generator = new Int32RangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<uint> ValueBetween(uint minValue, uint maxValue)
        {
            var generator = new UInt32RangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<long> ValueBetween(long minValue, long maxValue)
        {
            var generator = new Int64RangeGenerator(minValue, maxValue);
            return generator;
        }
        public static IGenerator<ulong> ValueBetween(ulong minValue, ulong maxValue)
        {
            var generator = new UInt64RangeGenerator(minValue, maxValue);
            return generator;
        }

        public static IGenerator<float> ValueBetween(float minValue, float maxValue)
        {
            var generator = new FloatRangeGenerator(minValue, maxValue);
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