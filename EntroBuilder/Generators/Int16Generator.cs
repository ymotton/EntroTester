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
}