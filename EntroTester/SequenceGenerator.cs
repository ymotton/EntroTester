using System;
using System.Collections.Generic;
using System.Linq;

namespace EntroTester
{
    public class SequenceGenerator<T> : IGenerator<T>
    {
        readonly int _count;
        readonly IEnumerable<T> _enumerable;

        public SequenceGenerator(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;

            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                _count = collection.Count;
            }
            else
            {
                _count = -1;
            }
        }

        public T Next(Random random)
        {
            T value;
            if (_count > -1)
            {
                value = _enumerable.Skip(random.Next(0, _count)).First();
            }
            else
            {
                value = _enumerable.Skip(random.Next(0, 10)).First();
            }
            return value;
        }

        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }
}
