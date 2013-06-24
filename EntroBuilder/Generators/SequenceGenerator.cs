using System;
using System.Collections.Generic;
using System.Linq;

namespace EntroBuilder
{
    public class SequenceGenerator<T> : IGenerator<T>
    {
        int? _count;
        readonly IEnumerable<T> _enumerable;
        readonly int _finiteSequenceCount;

        public SequenceGenerator(IEnumerable<T> enumerable, int finiteSequenceCount = 1000000)
        {
            _enumerable = enumerable;
            _finiteSequenceCount = finiteSequenceCount;

            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                _count = collection.Count;
            }
            else
            {
                SetCountForFiniteSequence();
            }
        }

        // Finite sequences can be hard to detect
        // If a sequence has a count <= finiteSequenceCount it's considered finite, otherwise treat it as infinite
        void SetCountForFiniteSequence()
        {
            var enumerator = _enumerable.GetEnumerator();
            for (int i = 0; i <= _finiteSequenceCount; i++)
            {
                if (!enumerator.MoveNext())
                {
                    _count = i;
                    break;
                }
            }
        }

        public T Next(Random random)
        {
            T value;
            if (_count.HasValue)
            {
                value = _enumerable.Skip(random.Next(0, _count.Value)).First();
            }
            // For infinite sequences skip randomly
            else
            {
                value = _enumerable.Skip(random.Next(0, _finiteSequenceCount)).First();
            }
            return value;
        }

        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }
}
