using System;

namespace EntroBuilder
{
    public class Int16Generator : ScalarGenerator<short>
    {
        protected override short NextImpl(Random random)
        {
            return (short)(random.NextDouble() * short.MaxValue);
        }
    }
    public class UInt16Generator : ScalarGenerator<ushort>
    {
        protected override ushort NextImpl(Random random)
        {
            return (ushort)(random.NextDouble() * ushort.MaxValue);
        }
    }
}