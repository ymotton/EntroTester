using System;

namespace EntroBuilder
{
    public class Int64Generator : ScalarGenerator<long>
    {
        protected override long NextImpl(Random random)
        {
            return (long)(random.NextDouble() * long.MaxValue);
        }
    }
    public class UInt64Generator : ScalarGenerator<ulong>
    {
        protected override ulong NextImpl(Random random)
        {
            return (ulong)(random.NextDouble() * ulong.MaxValue);
        }
    }
}