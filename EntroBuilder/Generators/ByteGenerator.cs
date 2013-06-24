using System;

namespace EntroBuilder
{
    public class ByteGenerator : ScalarGenerator<byte>
    {
        protected override byte NextImpl(Random random)
        {
            return (byte)(random.NextDouble() * byte.MaxValue);
        }
    }
}