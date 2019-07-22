using System;

namespace EntroBuilder.FallbackGenerators
{
    public interface IFallbackGenerator
    {
        bool TryNext(Type type, Random random, out object instance);
    }
}
