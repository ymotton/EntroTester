using System;

namespace EntroBuilder
{
    public class SByteGenerator : ScalarGenerator<sbyte>
    {
        protected override sbyte NextImpl(Random random)
        {
            return (sbyte)((random.NextDouble() * byte.MaxValue) + sbyte.MinValue);
        }
    }
}