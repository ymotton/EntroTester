using System;
using System.Collections;
using System.Collections.Generic;

namespace EntroBuilder
{
    public class CustomGenerator
    {
        public static CustomGenerator<T> Create<T>(Func<T> generatorDelegate, DelegateGeneratorOptions options = DelegateGeneratorOptions.None)
        {
            return new CustomGenerator<T>(generatorDelegate, options);
        }
    }

    public class CustomGenerator<T> : IGenerator<T>
    {
        readonly Func<T> _generatorDelegate;
        Maybe<T> _cachedSequence;
        public CustomGenerator(Func<T> generatorDelegate, DelegateGeneratorOptions options = DelegateGeneratorOptions.None)
        {
            _generatorDelegate = () =>
            {
                // If we're not supposed to cache, just clear it every time, so it gets recreated.
                if (_cachedSequence == null || options == DelegateGeneratorOptions.None)
                {
                    _cachedSequence = new Maybe<T>();
                }
                if (!_cachedSequence.HasValue)
                {
                    _cachedSequence = new Maybe<T>(generatorDelegate());
                }
                return _cachedSequence.Value;
            };
        }

        public T Next(Random random)
        {
            return _generatorDelegate();
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
