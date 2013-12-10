using System;

namespace EntroBuilder
{
    public class CharGenerator : ScalarGenerator<char>
    {
        protected override char NextImpl(Random random)
        {
            return (char)(random.NextDouble() * char.MaxValue);
        }
    }
}