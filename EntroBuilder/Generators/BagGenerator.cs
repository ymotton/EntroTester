using System;
using System.Collections.Generic;
using System.Linq;

namespace EntroBuilder.Generators
{
    public class BagGenerator<T> : IGenerator<T>
    {
        readonly IReadOnlyCollection<T> _possibleValues;
        public BagGenerator(IReadOnlyCollection<T> possibleValues)
        {
            _possibleValues = possibleValues;
        }

        List<T> _bag;
        public T Next(Random random)
        {
            if (_bag == null || _bag.Count == 0)
            {
                _bag = _possibleValues.ToList();
            }

            int index = random.Next(_bag.Count);
            T value = _bag[index];
            _bag.RemoveAt(index);
            return value;
        }
        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }
}
