using System;
using System.Collections.Generic;
using System.Linq;

namespace EntroTester
{
    public class CollectionGenerator<T> : IGenerator<List<T>>
        where T : class, new()
    {
        readonly Func<IEnumerable<T>> _generatorDelegate;
        IEnumerable<T> _cachedSequence;
        public CollectionGenerator(Func<IEnumerable<T>> generatorDelegate, DelegateGeneratorOptions options = DelegateGeneratorOptions.None)
        {
            _generatorDelegate = () =>
            {
                // If we're not supposed to cache, just clear it every time, so it gets recreated.
                if (options == DelegateGeneratorOptions.None)
                {
                    _cachedSequence = null;
                }
                if (_cachedSequence == null)
                {
                    _cachedSequence = generatorDelegate().ToList();
                }
                return _cachedSequence;
            };
        }

        public List<T> Next(Random random)
        {
            return _generatorDelegate().ToList();
        }

        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }

    public enum DelegateGeneratorOptions
    {
        None = 0,
        Cached
    }
}
