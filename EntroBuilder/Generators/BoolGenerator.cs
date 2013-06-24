using System;

namespace EntroBuilder
{
    public class BoolGenerator : ScalarGenerator<bool>
    {
        protected override bool NextImpl(Random random)
        {
            return random.Next(0, 2) == 1;
        }
    }
}