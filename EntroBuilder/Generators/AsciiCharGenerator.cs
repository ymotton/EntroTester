using System;

namespace EntroBuilder
{
    public class AsciiCharGenerator : ScalarGenerator<char>
    {
        protected override char NextImpl(Random random)
        {
            return (char)(0x20 + random.Next(0x7f - 0x20));
        }
    }
}