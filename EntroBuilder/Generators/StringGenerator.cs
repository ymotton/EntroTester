using System;

namespace EntroBuilder
{
    public class StringGenerator : ScalarGenerator<string>
    {
        protected override string NextImpl(Random random)
        {
            return Guid.NewGuid().ToString().Split('-')[0];
        }
    }
}