using System;

namespace EntroBuilder
{
    public abstract class ScalarGenerator<T> : IGenerator<T>
    {
        public virtual T Next(Random random)
        {
            return NextImpl(random);
        }
        protected abstract T NextImpl(Random random);
        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }
}