using System;
using System.Collections.Generic;
using EntroBuilder.Generators;

namespace EntroBuilder
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
        
        public static IGenerator<T> Or<T>(this IGenerator<T> generator, IGenerator<T> otherGenerator, double ratio = 1.0)
        {
            var compoundGenerator = generator as CompoundGenerator<T>;
            if (compoundGenerator != null)
            {
                return compoundGenerator.Or(otherGenerator, ratio);
            }
            return new CompoundGenerator<T>(new[] {Tuple.Create(generator, 1.0), Tuple.Create(otherGenerator, ratio)});
        }
        public static IGenerator<T> Or<T>(this CompoundGenerator<T> compoundGenerator, IGenerator<T> otherGenerator, double ratio = 1.0)
        {
            return new CompoundGenerator<T>(compoundGenerator, Tuple.Create(otherGenerator, ratio));
        }
    }
}
