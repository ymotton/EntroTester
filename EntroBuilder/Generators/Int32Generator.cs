using System;

namespace EntroBuilder
{
    public class Int32Generator : ScalarGenerator<int>
    {
        protected override int NextImpl(Random random)
        {
            return random.Next(int.MaxValue);
        }
    }
    public class UInt32Generator : ScalarGenerator<uint>
    {
        protected override uint NextImpl(Random random)
        {
            return (uint)(random.NextDouble() * uint.MaxValue);
        }
    }
}