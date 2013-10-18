using System;
using System.Collections.Generic;

namespace EntroBuilder.Generators
{
    public static class GeneratorExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this IGenerator<T> generator, Random random)
        {
            while (true)
            {
                yield return generator.Next(random);
            }
        }
    }
}
