using System;

namespace EntroBuilder
{
    public static class CustomGenerator
    {
        public static CustomGenerator<T> Create<T>(Func<T> generatorDelegate, DelegateGeneratorOptions options = DelegateGeneratorOptions.None)
        {
            return new CustomGenerator<T>(generatorDelegate, options);
        }
        public static CustomGenerator<T> Create<T>(Func<Random, T> generatorDelegate, DelegateGeneratorOptions options = DelegateGeneratorOptions.None)
        {
            return new CustomGenerator<T>(generatorDelegate, options);
        }
    }

    public class CustomGenerator<T> : IGenerator<T>
    {
        readonly Func<Random, T> _generatorDelegate;
        Maybe<T> _cachedSequence;
        public CustomGenerator(Func<T> generatorDelegate, DelegateGeneratorOptions options = DelegateGeneratorOptions.None) : this(r => generatorDelegate(), options) { }
        public CustomGenerator(Func<Random, T> generatorDelegate, DelegateGeneratorOptions options = DelegateGeneratorOptions.None)
        {
            _generatorDelegate = random =>
            {
                // If we're not supposed to cache, just clear it every time, so it gets recreated.
                if (_cachedSequence == null || options == DelegateGeneratorOptions.None)
                {
                    _cachedSequence = new Maybe<T>();
                }
                if (!_cachedSequence.HasValue)
                {
                    _cachedSequence = new Maybe<T>(generatorDelegate(random));
                }
                return _cachedSequence.Value;
            };
        }

        public T Next(Random random)
        {
            return _generatorDelegate(random);
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
