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
}