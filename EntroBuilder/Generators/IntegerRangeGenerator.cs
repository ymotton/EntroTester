using System;

namespace EntroBuilder
{
    public class SByteRangeGenerator : RangeGenerator<sbyte>
    {
        public SByteRangeGenerator(sbyte minValue, sbyte maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static sbyte GenerateRandomBetween(sbyte minValue, sbyte maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
    public class ByteRangeGenerator : RangeGenerator<byte>
    {
        public ByteRangeGenerator(byte minValue, byte maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static byte GenerateRandomBetween(byte minValue, byte maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
    public class Int16RangeGenerator : RangeGenerator<short>
    {
        public Int16RangeGenerator(short minValue, short maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static short GenerateRandomBetween(short minValue, short maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
    public class UInt16RangeGenerator : RangeGenerator<ushort>
    {
        public UInt16RangeGenerator(ushort minValue, ushort maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static ushort GenerateRandomBetween(ushort minValue, ushort maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
    public class Int32RangeGenerator : RangeGenerator<int>
    {
        public Int32RangeGenerator(int minValue, int maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static int GenerateRandomBetween(int minValue, int maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
    public class UInt32RangeGenerator : RangeGenerator<uint>
    {
        public UInt32RangeGenerator(uint minValue, uint maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static uint GenerateRandomBetween(uint minValue, uint maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
    public class Int64RangeGenerator : RangeGenerator<long>
    {
        public Int64RangeGenerator(long minValue, long maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static long GenerateRandomBetween(long minValue, long maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
    public class UInt64RangeGenerator : RangeGenerator<ulong>
    {
        public UInt64RangeGenerator(ulong minValue, ulong maxValue) : base(minValue, maxValue, GenerateRandomBetween)
        {
        }
        static ulong GenerateRandomBetween(ulong minValue, ulong maxValue, Random random)
        {
            return random.NextBetween(minValue, maxValue);
        }
    }
}