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
}