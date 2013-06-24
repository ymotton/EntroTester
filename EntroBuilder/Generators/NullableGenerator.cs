using System;

namespace EntroBuilder
{
    public class NullableGenerator<T> : IGenerator<T?>
        where T : struct
    {
        readonly ScalarGenerator<T> _scalarGenerator;
        public NullableGenerator(ScalarGenerator<T> scalarGenerator)
        {
            _scalarGenerator = scalarGenerator;
        }
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
}
