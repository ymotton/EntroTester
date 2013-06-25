using System;

namespace EntroBuilder
{
    public class NullableGenerator<T> : IGenerator<T?>
        where T : struct
    {
        readonly IGenerator<T> _scalarGenerator;
        public NullableGenerator(IGenerator<T> scalarGenerator)
        {
            _scalarGenerator = scalarGenerator;
        }

        /// <summary>
        /// A Nullable generator has 50% chance to generate a null, and 50% chance to generate a primitive value
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public T? Next(Random random)
        {
            if (random.Next(0, 2) == 1)
            {
                return null;
            }

            return _scalarGenerator.Next(random);
        }
        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }

    public class NullableGenerator
    {
        /// <summary>
        /// A Nullable generator has 50% chance to generate a null, and 50% chance to generate a primitive value
        /// </summary>
        /// <param name="scalarGenerator"></param>
        /// <returns></returns>
        public static NullableGenerator<T> Create<T>(IGenerator<T> scalarGenerator)
            where T : struct 
        {
            return new NullableGenerator<T>(scalarGenerator);
        }
    }
}
