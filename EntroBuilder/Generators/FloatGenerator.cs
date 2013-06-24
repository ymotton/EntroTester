using System;

namespace EntroBuilder
{
    public class FloatGenerator : ScalarGenerator<float>
    {
        protected override float NextImpl(Random random)
        {
            return (float)(random.NextDouble() * short.MaxValue);
        }
    }
}