using System;

namespace EntroBuilder
{
    public class GuidGenerator : ScalarGenerator<Guid>
    {
        protected override Guid NextImpl(Random random)
        {
            return Guid.NewGuid();
        }
    }
}